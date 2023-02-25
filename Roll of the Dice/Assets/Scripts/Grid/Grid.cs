using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public LevelData LevelData;

    [Header("Tiles Data")]
    [SerializeField] private TileData diceData;
    [SerializeField] private TileData flagData;
    [SerializeField] private TileData blockData;
    [SerializeField] private TileData powerData;
    [SerializeField] private TileData teleportData;

    [Header("References")]
    [SerializeField] private Material flagMaterial;
    private Transform blocksParent;
    private Transform powersParent;
    private Transform teleportsParent;

    [Header("Editor")]
    public bool InEditor;
    public bool InMenu;
    public bool godMode;

    private GameObject dice;
    private GameObject flag;


    [HideInInspector] public Vector3 startPosition = new Vector3(0,-0.5f,0);
    private int currentRows;
    private int currentColumns;

    private GridTile tilePrefab;

    public List<List<GridTile>> grid = new List<List<GridTile>>();

    void OnDrawGizmos()
    {
        if(!LevelData || InEditor) return;

        for(int a = 0; a < LevelData.GridColumns; a++)
        {
            for(int b = 0; b < LevelData.GridRows; b++)
            {
                Gizmos.DrawWireCube(new Vector3(startPosition.x + b, 0, startPosition.z + a), new Vector3(1, 0.01f, 1));
            }
        }

        if (LevelData == null) return;

        foreach (Vector2 block in LevelData.Blocks)
        {
            Vector3 position = startPosition + new Vector3(block.x, 1, block.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position, new Vector3(1, 1, 1));
        }

        foreach (PowerTile power in LevelData.Powers)
        {
            Vector3 position = startPosition + new Vector3(power.Coordinates.x, 1, power.Coordinates.y);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(position, new Vector3(.5f, .5f, .5f));
        }

        foreach (TeleportTile teleport in LevelData.Teleports)
        {
            Vector3 position = startPosition + new Vector3(teleport.Coordinates.x, .625f, teleport.Coordinates.y);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(position, new Vector3(1, .25f, 1f));
        }

        if (LevelData.FlagCoordinates != LevelData.DiceCoordinates)
        {
            Vector3 position = startPosition + new Vector3(LevelData.DiceCoordinates.x, 1, LevelData.DiceCoordinates.y);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(position, new Vector3(.8f, .8f, .8f));


            position = startPosition + new Vector3(LevelData.FlagCoordinates.x, 1, LevelData.FlagCoordinates.y);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(position, new Vector3(.25f, .25f, .25f));
        }
    }

    void Awake()
    {
        tilePrefab = GetComponentInChildren<GridTile>();

        currentColumns = LevelData.GridColumns;
        currentRows = LevelData.GridRows;

        blocksParent = transform.Find("Blocks");
        powersParent = transform.Find("Powers");
        teleportsParent = transform.Find("Teleports");

        if(tilePrefab) GenerateGrid();
        
        if(!InEditor) Init();
    }


    void Update()
    {
        if(currentRows != LevelData.GridRows) SetRows(LevelData.GridRows);

        if(currentColumns != LevelData.GridColumns) SetColumns(LevelData.GridColumns);

        currentRows = LevelData.GridRows;
        currentColumns = LevelData.GridColumns;
    }
    public void GenerateGrid()
    {
        for(int a = 0; a < LevelData.GridRows; a++)
        {

            grid.Add(new List<GridTile>());

            for(int b = 0; b < LevelData.GridColumns; b++)
            {
                GridTile tile = Instantiate(tilePrefab, new Vector3(startPosition.x + a, startPosition.y, startPosition.z + b), Quaternion.identity, transform);
                tile.name = "Tile";

                grid[a].Add(tile);
            }
        }
    }

    public void SetRows(int value)
    {
        LevelData.GridRows = value;
        ResetGrid();
    }

    public void SetColumns(int value)
    {
        LevelData.GridColumns = value;
        ResetGrid();
    }

    public void ResetGrid()
    {
        foreach (Transform child in transform)
        {
            if(child.name == "Tile")
            {
                Destroy(child.gameObject);
            }
        }

        grid.Clear();
        GenerateGrid();
        Camera.main.GetComponent<CameraScript>().ResetCamera();
    }

    public bool TileHasFlag(int x, int y, TileTypes flag)
    {
        return grid[x][y].TileType.HasFlag(flag);
    }

    public GridTile GetTile(int x, int y)
    {
        if((0 <= x && x < LevelData.GridRows) && (0 <= y && y < LevelData.GridColumns))
        {
            return grid[x][y];
        } 
        else 
        {
            return null;
        }
    }

    // TILES PLACER

    public void Init()
    {
        if (LevelData.FlagCoordinates != LevelData.DiceCoordinates)
        {
            Vector3 position = new Vector3(LevelData.DiceCoordinates.x, .46f, LevelData.DiceCoordinates.y);
            GameObject newDice = Instantiate(diceData.Tile, position, Quaternion.identity, transform.parent);
            newDice.name = "Dice";
            dice = newDice;

            dice.GetComponent<Dice>().DiceValue = LevelData.DiceValue;
            dice.GetComponent<Dice>().Row = (int)LevelData.DiceCoordinates.x;
            dice.GetComponent<Dice>().Column = (int)LevelData.DiceCoordinates.y;
            dice.GetComponent<Dice>().GodMode = godMode;
            GetTile((int)LevelData.DiceCoordinates.x, (int)LevelData.DiceCoordinates.y).TileType = TileTypes.Dice;


            position = new Vector3(LevelData.FlagCoordinates.x, 1, LevelData.FlagCoordinates.y);
            GameObject newFlag = Instantiate(flagData.Tile, position, Quaternion.identity, transform.parent);
            newFlag.name = "Flag";
            flag = newFlag;

            GetTile((int)LevelData.FlagCoordinates.x, (int)LevelData.FlagCoordinates.y).TileType = TileTypes.Flag;
            GetTile((int)LevelData.FlagCoordinates.x, (int)LevelData.FlagCoordinates.y).GetComponent<MeshRenderer>().material = flagMaterial;
        }

        foreach (Vector2 blockCoord in LevelData.Blocks)
        {
            Vector3 position = new Vector3(blockCoord.x, .5f, blockCoord.y);
            GameObject newBlock = Instantiate(blockData.Tile, position, Quaternion.identity, blocksParent);
            newBlock.name = "Block";

            GetTile((int)blockCoord.x, (int)blockCoord.y).TileType = TileTypes.Block;
        }

        foreach (PowerTile power in LevelData.Powers)
        {
            Vector3 position = new Vector3(power.Coordinates.x, .5f, power.Coordinates.y);
            GameObject newPower = Instantiate(powerData.Tile, position, Quaternion.identity, powersParent);
            newPower.name = "Power";
            power.Object = newPower;

            switch (power.PowerType)
            {
                case PowerTypeTest.Add:
                    power.Object.GetComponent<PowerScript>().SetValue("+" + power.Value.ToString());
                    break;

                case PowerTypeTest.Remove:
                    power.Object.GetComponent<PowerScript>().SetValue("-" + power.Value.ToString());
                    break;

                case PowerTypeTest.Double:
                    power.Object.GetComponent<PowerScript>().SetValue("x2");
                    break;

                case PowerTypeTest.Split:
                    power.Object.GetComponent<PowerScript>().SetValue("/2");
                    break;
            }

            GetTile((int)power.Coordinates.x, (int)power.Coordinates.y).TileType = TileTypes.Power;
        }

        foreach (TeleportTile teleport in LevelData.Teleports)
        {
            Vector3 position = new Vector3(teleport.Coordinates.x, .125f, teleport.Coordinates.y);
            GameObject newTeleport = Instantiate(teleportData.Tile, position, Quaternion.identity, teleportsParent);
            newTeleport.name = "Teleport";
            teleport.Object = newTeleport;

            GetTile((int)teleport.Coordinates.x, (int)teleport.Coordinates.y).TileType = TileTypes.Teleport;
        }

    }

    public TileTypes FindTileType(int x, int y)
    {
        foreach (Vector2 tile in LevelData.Blocks) if (tile == new Vector2(x, y)) return TileTypes.Block;

        if (LevelData.DiceCoordinates == new Vector2(x, y)) return TileTypes.Dice;

        if (LevelData.FlagCoordinates == new Vector2(x, y)) return TileTypes.Flag;

        foreach (PowerTile tile in LevelData.Powers) if (tile.Coordinates == new Vector2(x, y)) return TileTypes.Power;

        foreach (TeleportTile tile in LevelData.Teleports) if (tile.Coordinates == new Vector2(x, y)) return TileTypes.Teleport;

        return TileTypes.None;
    }

    public bool TileIsBlock(int x, int y)
    {
        foreach (Vector2 tile in LevelData.Blocks) if (tile == new Vector2(x, y)) return true;

        return false;
    }
    public bool TileIsDice(int x, int y)
    {
        if (LevelData.DiceCoordinates == new Vector2(x, y)) return true;

        return false;
    }
    public bool TileIsFlag(int x, int y)
    {
        if (LevelData.FlagCoordinates == new Vector2(x, y)) return true;

        return false;
    }
    public bool TileIsPower(int x, int y)
    {
        foreach (PowerTile tile in LevelData.Powers) if (tile.Coordinates == new Vector2(x, y)) return true;

        return false;
    }
    public bool TileIsTeleport(int x, int y)
    {
        foreach (TeleportTile tile in LevelData.Teleports) if (tile.Coordinates == new Vector2(x, y)) return true;

        return false;
    }

    public PowerTile GetPowerTile(int x, int y)
    {
        foreach (PowerTile tile in LevelData.Powers) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public TeleportTile GetTeleportTile(int x, int y)
    {
        foreach (TeleportTile tile in LevelData.Teleports) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public int GetLastTileRow()
    {
        int lastTileRow = 0;

        if((int)LevelData.DiceCoordinates.x > lastTileRow) lastTileRow = (int)LevelData.DiceCoordinates.x;
        if((int)LevelData.FlagCoordinates.x > lastTileRow) lastTileRow = (int)LevelData.FlagCoordinates.x;

        foreach (Vector2 block in LevelData.Blocks) if ((int)block.x > lastTileRow) lastTileRow = (int)block.x;
        foreach (PowerTile powerTile in LevelData.Powers) if ((int)powerTile.Coordinates.x > lastTileRow) lastTileRow = (int)powerTile.Coordinates.x;
        foreach (TeleportTile teleportTile in LevelData.Teleports) if ((int)teleportTile.Coordinates.x > lastTileRow) lastTileRow = (int)teleportTile.Coordinates.x;
    
        return lastTileRow;
    }
    public int GetLastTileColumn()
    {
        int lastTileColumn = 0;

        if((int)LevelData.DiceCoordinates.y > lastTileColumn) lastTileColumn = (int)LevelData.DiceCoordinates.y;
        if((int)LevelData.FlagCoordinates.y > lastTileColumn) lastTileColumn = (int)LevelData.FlagCoordinates.y;

        foreach (Vector2 block in LevelData.Blocks) if ((int)block.y > lastTileColumn) lastTileColumn = (int)block.y;
        foreach (PowerTile powerTile in LevelData.Powers) if ((int)powerTile.Coordinates.y > lastTileColumn) lastTileColumn = (int)powerTile.Coordinates.y;
        foreach (TeleportTile teleportTile in LevelData.Teleports) if ((int)teleportTile.Coordinates.y > lastTileColumn) lastTileColumn = (int)teleportTile.Coordinates.y;
    
        return lastTileColumn;
    }


    public void DeleteTiles()
    {
        foreach (Transform group in transform) foreach (Transform child in group) Destroy(child.gameObject);

        Destroy(transform.parent.Find("Dice").gameObject);
        Destroy(transform.parent.Find("Flag").gameObject);

        ResetGrid();
    }

    public void ResetTiles()
    {
        LevelData.DiceCoordinates = new Vector2(-1, -1);
        LevelData.FlagCoordinates = new Vector2(-1, -1);
        LevelData.Blocks = new List<Vector2>();
        LevelData.Powers = new List<PowerTile>();
        LevelData.Teleports = new List<TeleportTile>();
    }
}

[System.Serializable]
public class TeleportTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    public Vector2 DestinationCoordinates = new Vector2(-1, -1);
    [HideInInspector] public GameObject Object;
    [HideInInspector] public GameObject PreviewObject;

    public TeleportTile(int x, int y, int xD, int yD)
    {
        this.Coordinates = new Vector2(x, y);
        this.DestinationCoordinates = new Vector2(xD, yD);
    }
}

[System.Serializable]
public class PowerTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    public PowerTypeTest PowerType;
    [Range(0, 6)] public int Value;
    [HideInInspector] public GameObject Object;

    public PowerTile(int x, int y, PowerTypeTest powerType, int value)
    {
        this.Coordinates = new Vector2(x, y);
        this.PowerType = powerType;
        this.Value = value;
    }
}