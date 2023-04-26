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
    [SerializeField] private TileDataList tilesList;

    [Header("References")]
    [SerializeField] private Material flagMaterial;
    private Transform tilesParent;

    [Header("Gizmos Settings")]
    [SerializeField] private bool showGrid;
    [SerializeField] private bool showTiles;
    [SerializeField] private bool drawSolid;

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
        if(!LevelData) return;

        if(showGrid)
        {
            for(int a = 0; a < LevelData.GridColumns; a++)
            {
                for(int b = 0; b < LevelData.GridRows; b++)
                {
                    Gizmos.DrawWireCube(new Vector3(startPosition.x + b, 0, startPosition.z + a), new Vector3(1, 0.01f, 1));
                }
            }
        }

        if(showTiles)
        {
            if (LevelData.FlagCoordinates != LevelData.DiceCoordinates)
            {
                var tile = GetTileData("Dice");

                Vector3 position = startPosition + new Vector3(LevelData.DiceCoordinates.x, 1, LevelData.DiceCoordinates.y);

                DrawGizmo(position, GetTileData("Dice").GizmoSize, GetTileData("Dice").GizmoColor);


                tile = GetTileData("Flag");
                position = startPosition + new Vector3(LevelData.FlagCoordinates.x, 1, LevelData.FlagCoordinates.y);

                DrawGizmo(position, GetTileData("Flag").GizmoSize, GetTileData("Flag").GizmoColor);
            }

            if(LevelData.TilesList.BlockTiles.Count > 0)
            {
                foreach (Tile block in LevelData.TilesList.BlockTiles)
                {
                    var tile = GetTileData("Block");

                    Vector3 position = startPosition + new Vector3(block.Coordinates.x, 1, block.Coordinates.y);

                    DrawGizmo(position, GetTileData("Block").GizmoSize, GetTileData("Block").GizmoColor);
                }
            }

            if (LevelData.TilesList.PowerTiles.Count > 0)
            {
                foreach (PowerTile power in LevelData.TilesList.PowerTiles)
                {
                    var tile = GetTileData("Power");
                    
                    Vector3 position = startPosition + new Vector3(power.Coordinates.x, 1, power.Coordinates.y);

                    DrawGizmo(position, GetTileData("Power").GizmoSize, GetTileData("Power").GizmoColor);
                }
            }

            if (LevelData.TilesList.TeleportTiles.Count > 0)
            {
                foreach (TeleportTile teleport in LevelData.TilesList.TeleportTiles)
                {
                    var tile = GetTileData("Teleport");

                    Vector3 position = startPosition + new Vector3(teleport.Coordinates.x, .625f, teleport.Coordinates.y);

                    DrawGizmo(position, GetTileData("Teleport").GizmoSize, GetTileData("Teleport").GizmoColor);
                }
            }

            if (LevelData.TilesList.ButtonTiles.Count > 0)
            {
                foreach (ButtonTile button in LevelData.TilesList.ButtonTiles)
                {
                    var tile = GetTileData("Button");

                    Vector3 position = startPosition + new Vector3(button.Coordinates.x, .625f, button.Coordinates.y);

                    DrawGizmo(position, GetTileData("Button").GizmoSize, GetTileData("Button").GizmoColor);
                }
            }

            if (LevelData.TilesList.GhostBlockTiles.Count > 0)
            {
                foreach (GhostBlockTile ghostBlock in LevelData.TilesList.GhostBlockTiles)
                {
                    var tile = GetTileData("Ghost Block");

                    Vector3 position = startPosition + new Vector3(ghostBlock.Coordinates.x, 1, ghostBlock.Coordinates.y);

                    DrawGizmo(position, GetTileData("Ghost Block").GizmoSize, GetTileData("Ghost Block").GizmoColor);
                }
            }
        }
    }

    private void DrawGizmo(Vector3 position, Vector3 size, Color color)
    {
        Gizmos.color = color;
        if(drawSolid) Gizmos.DrawCube(position, size);
        Gizmos.DrawWireCube(position, size);
    
    }

    private TileData GetTileData(string tileName)
    {
        foreach (var tile in tilesList.TilesList)
        {
            if (tile.Name == tileName)
            {
                return tile;
            }
        }

        return null;
    }

    void Awake()
    {
        tilePrefab = GetComponentInChildren<GridTile>();

        currentColumns = LevelData.GridColumns;
        currentRows = LevelData.GridRows;

        tilesParent = transform.Find("TilesParent");

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
        if(GameObject.FindObjectOfType<CameraScript>()) GameObject.FindObjectOfType<CameraScript>().ResetCamera();
    }

    public bool TileHasFlag(int x, int y, TileType flag)
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
    public TileType GetTileType(int x, int y)
    {
        return GetTile(x, y).TileType;
    }

    public TileType GetType(int x, int y)
    {
        foreach (Tile tile in LevelData.TilesList.BlockTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Block;

        if (LevelData.DiceCoordinates == new Vector2(x, y)) return TileType.Dice;

        if (LevelData.FlagCoordinates == new Vector2(x, y)) return TileType.Flag;

        foreach (PowerTile tile in LevelData.TilesList.PowerTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Power;

        foreach (TeleportTile tile in LevelData.TilesList.TeleportTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Teleport;

        foreach (ButtonTile tile in LevelData.TilesList.ButtonTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Button;

        foreach (GhostBlockTile tile in LevelData.TilesList.GhostBlockTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.GhostBlock;

        return TileType.None;
    }


    // TILES PLACER

    public void Init()
    {
        if (LevelData.FlagCoordinates != LevelData.DiceCoordinates)
        {
            Vector3 position = new Vector3(LevelData.DiceCoordinates.x, .46f, LevelData.DiceCoordinates.y);
            GameObject newDice = Instantiate(GetTileData("Dice").Tile, position, Quaternion.identity, transform.parent);
            newDice.name = "Dice";
            dice = newDice;

            dice.GetComponent<DiceScript>().DiceValue = LevelData.DiceValue;
            dice.GetComponent<DiceScript>().Row = (int)LevelData.DiceCoordinates.x;
            dice.GetComponent<DiceScript>().Column = (int)LevelData.DiceCoordinates.y;
            dice.GetComponent<DiceScript>().GodMode = godMode;
            GetTile((int)LevelData.DiceCoordinates.x, (int)LevelData.DiceCoordinates.y).TileType = TileType.Dice;


            position = new Vector3(LevelData.FlagCoordinates.x, 1, LevelData.FlagCoordinates.y);
            GameObject newFlag = Instantiate(GetTileData("Flag").Tile, position, Quaternion.identity, transform.parent);
            newFlag.name = "Flag";
            flag = newFlag;

            GetTile((int)LevelData.FlagCoordinates.x, (int)LevelData.FlagCoordinates.y).TileType = TileType.Flag;
            GetTile((int)LevelData.FlagCoordinates.x, (int)LevelData.FlagCoordinates.y).GetComponent<MeshRenderer>().material = flagMaterial;
        }

        foreach (Tile block in LevelData.TilesList.BlockTiles)
        {
            Vector3 position = new Vector3(block.Coordinates.x, .5f, block.Coordinates.y);
            GameObject newBlock = Instantiate(GetTileData("Block").Tile, position, Quaternion.identity, tilesParent);
            newBlock.name = "Block";

            GetTile((int)block.Coordinates.x, (int)block.Coordinates.y).TileType = TileType.Block;
        }

        foreach (PowerTile power in LevelData.TilesList.PowerTiles)
        {
            Vector3 position = new Vector3(power.Coordinates.x, .5f, power.Coordinates.y);
            GameObject newPower = Instantiate(GetTileData("Power").Tile, position, Quaternion.identity, tilesParent);
            newPower.name = "Power";
            power.Object = newPower;

            switch (power.PowerType)
            {
                case PowerType.Add:
                    power.Object.GetComponent<PowerScript>().SetValue("+" + power.Value.ToString());
                    break;

                case PowerType.Remove:
                    power.Object.GetComponent<PowerScript>().SetValue("-" + power.Value.ToString());
                    break;

                case PowerType.Double:
                    power.Object.GetComponent<PowerScript>().SetValue("x2");
                    break;

                case PowerType.Split:
                    power.Object.GetComponent<PowerScript>().SetValue("/2");
                    break;
            }

            GetTile((int)power.Coordinates.x, (int)power.Coordinates.y).TileType = TileType.Power;
        }

        foreach (TeleportTile teleport in LevelData.TilesList.TeleportTiles)
        {
            Vector3 position = new Vector3(teleport.Coordinates.x, .125f, teleport.Coordinates.y);
            GameObject newTeleport = Instantiate(GetTileData("Teleport").Tile, position, Quaternion.identity, tilesParent);
            newTeleport.name = "Teleport";
            teleport.Object = newTeleport;

            GetTile((int)teleport.Coordinates.x, (int)teleport.Coordinates.y).TileType = TileType.Teleport;
        }

        foreach (ButtonTile button in LevelData.TilesList.ButtonTiles)
        {
            Vector3 position = new Vector3(button.Coordinates.x, .125f, button.Coordinates.y);
            GameObject newButton = Instantiate(GetTileData("Button").Tile, position, Quaternion.identity, tilesParent);
            newButton.name = "Button";
            button.Object = newButton;

            GetTile((int)button.Coordinates.x, (int)button.Coordinates.y).TileType = TileType.Button;
        }

        foreach (GhostBlockTile ghostBlock in LevelData.TilesList.GhostBlockTiles)
        {
            Vector3 position = new Vector3(ghostBlock.Coordinates.x, .5f, ghostBlock.Coordinates.y);
            GameObject newBlock = Instantiate(GetTileData("Ghost Block").Tile, position, Quaternion.identity, tilesParent);
            newBlock.name = "Ghost Block";
            ghostBlock.Object = newBlock;

            GetTile((int)ghostBlock.Coordinates.x, (int)ghostBlock.Coordinates.y).TileType = TileType.GhostBlock;
        }
    }

    public PowerTile GetPowerTile(int x, int y)
    {
        foreach (PowerTile tile in LevelData.TilesList.PowerTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public TeleportTile GetTeleportTile(int x, int y)
    {
        foreach (TeleportTile tile in LevelData.TilesList.TeleportTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }
    public ButtonTile GetButtonTile(int x, int y)
    {
        foreach (ButtonTile tile in LevelData.TilesList.ButtonTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }
    public GhostBlockTile GetGhostBlockTile(int x, int y)
    {
        foreach (GhostBlockTile tile in LevelData.TilesList.GhostBlockTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public ButtonTile GetButtonTileByGhostBlockTile(int x, int y)
    {
        foreach (ButtonTile tile in LevelData.TilesList.ButtonTiles) if (tile.DestinationCoordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public int GetLastTileRow()
    {
        int lastTileRow = 0;

        if((int)LevelData.DiceCoordinates.x > lastTileRow) lastTileRow = (int)LevelData.DiceCoordinates.x;
        if((int)LevelData.FlagCoordinates.x > lastTileRow) lastTileRow = (int)LevelData.FlagCoordinates.x;

        foreach (Tile block in LevelData.TilesList.BlockTiles) if ((int)block.Coordinates.x > lastTileRow) lastTileRow = (int)block.Coordinates.x;
        foreach (PowerTile powerTile in LevelData.TilesList.PowerTiles) if ((int)powerTile.Coordinates.x > lastTileRow) lastTileRow = (int)powerTile.Coordinates.x;
        foreach (TeleportTile teleportTile in LevelData.TilesList.TeleportTiles) if ((int)teleportTile.Coordinates.x > lastTileRow) lastTileRow = (int)teleportTile.Coordinates.x;
    
        return lastTileRow;
    }
    public int GetLastTileColumn()
    {
        int lastTileColumn = 0;

        if((int)LevelData.DiceCoordinates.y > lastTileColumn) lastTileColumn = (int)LevelData.DiceCoordinates.y;
        if((int)LevelData.FlagCoordinates.y > lastTileColumn) lastTileColumn = (int)LevelData.FlagCoordinates.y;

        foreach (Tile block in LevelData.TilesList.BlockTiles) if ((int)block.Coordinates.y > lastTileColumn) lastTileColumn = (int)block.Coordinates.y;
        foreach (PowerTile powerTile in LevelData.TilesList.PowerTiles) if ((int)powerTile.Coordinates.y > lastTileColumn) lastTileColumn = (int)powerTile.Coordinates.y;
        foreach (TeleportTile teleportTile in LevelData.TilesList.TeleportTiles) if ((int)teleportTile.Coordinates.y > lastTileColumn) lastTileColumn = (int)teleportTile.Coordinates.y;
    
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
        LevelData.TilesList.BlockTiles = new List<Tile>();
        LevelData.TilesList.PowerTiles = new List<PowerTile>();
        LevelData.TilesList.TeleportTiles = new List<TeleportTile>();
    }
}