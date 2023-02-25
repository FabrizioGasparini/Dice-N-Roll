using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorPlacementScript : MonoBehaviour
{
    [Header("Tiles")]
    public TileTypes selectedTileType;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private LayerMask tilesLayer;

    [Header("Data")]
    public TileData diceData;
    public TileData flagData;
    public TileData blockData;
    public TileData powerData;
    public TileData teleportData;

    // References
    private GameObject spawnedTile;
    private Transform previewsHolder;
    private Camera mainCam;

    // Bools
    private bool deleteModeEnabled;
    private bool testModeEnabled;

    // Grid Scripts
    private Grid grid;
    private Grid tilesPlacer;

    // Tiles Settings
    private TileTypes currentTileType;

    private bool hasDice;
    private bool hasFlag;


    private List<PowerTile> powerTiles = new List<PowerTile>();

    private int powerValue = 1;
    private PowerTypeTest powerType = PowerTypeTest.Add;

    private List<TeleportTile> teleportTiles = new List<TeleportTile>();
    [HideInInspector] public bool firstTeleportPlaced;
    private Vector2 firstTeleportCoordinates;
    private GameObject firstTeleportObject;

    // MAIN
    void Awake() 
    {
        mainCam = Camera.main;
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        tilesPlacer = grid.GetComponent<Grid>();
        previewsHolder = transform.GetChild(0);
        Init();
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.D)) deleteModeEnabled = !deleteModeEnabled;

        if(!deleteModeEnabled) PlacingModeLogic();
        else DeletingModeLogic();

    }

    public void Init()
    {
        ResetTiles();

        foreach (Vector2 block in grid.LevelData.Blocks)
        {
            Vector3 position = grid.startPosition + new Vector3(block.x, 1, block.y);
            var newBlock = Instantiate(blockData.Preview, position, Quaternion.identity, previewsHolder);
            newBlock.name = "Block";
        }
        foreach (PowerTile power in grid.LevelData.Powers)
        {
            Vector3 position = grid.startPosition + new Vector3(power.Coordinates.x, 1, power.Coordinates.y);
            var newPower = Instantiate(powerData.Preview, position, Quaternion.identity, previewsHolder);
            newPower.name = "Power";
        }
        foreach (TeleportTile teleport in grid.LevelData.Teleports)
        {
            Vector3 position = grid.startPosition + new Vector3(teleport.Coordinates.x, 1, teleport.Coordinates.y);
            var newTeleport = Instantiate(teleportData.Preview, position, Quaternion.identity, previewsHolder);
            newTeleport.name = "Teleport";
        
            teleport.PreviewObject = newTeleport;
        }

        if(grid.LevelData.DiceCoordinates != new Vector2(-1, -1))
        {
            Vector3 position = grid.startPosition + new Vector3(grid.LevelData.DiceCoordinates.x, 1, grid.LevelData.DiceCoordinates.y);
            var newDice = Instantiate(diceData.Preview, position, Quaternion.identity, previewsHolder);
            newDice.name = "Dice";
            hasDice = true;
        }
        if (grid.LevelData.FlagCoordinates != new Vector2(-1, -1))
        {
            Vector3 position = grid.startPosition + new Vector3(grid.LevelData.FlagCoordinates.x, 1, grid.LevelData.FlagCoordinates.y);
            var newFlag = Instantiate(flagData.Preview, position, Quaternion.identity, previewsHolder);
            newFlag.name = "Flag";
            hasFlag = true;
        }
    }

    public void ResetTiles()
    {
        if(previewsHolder.gameObject) foreach (Transform child in previewsHolder) Destroy(child.gameObject);
    }

    public void SelectTileType(TileTypes tileType)
    {
        if(!firstTeleportPlaced)
        {
            if(deleteModeEnabled) SetDeletingMode();
            selectedTileType = tileType;
        }
        else ErrorsPanelScript.SendError.Invoke("Place the destination tile of the <color=purple>TELEPORT<color=red> tile");
    }

    private void SelectTile(TileTypes tileType)
    {
        selectedTileType = tileType;

        if(deleteModeEnabled)
        {
            deleteModeEnabled = false;
        }

        DeleteObjectPreview();

        GameObject newTile = null;

        if(selectedTileType != TileTypes.None)
        {
            switch(selectedTileType)
            {
                case TileTypes.Block:
                    newTile = Instantiate(blockData.Preview);
                    break;

                case TileTypes.Dice:
                    newTile = Instantiate(diceData.Preview);
                    break;

                case TileTypes.Flag:
                    newTile = Instantiate(flagData.Preview);
                    break;

                case TileTypes.Power:
                    newTile = Instantiate(powerData.Preview);
                    break;

                case TileTypes.Teleport:
                    newTile = Instantiate(teleportData.Preview);
                    break;

            }

            newTile.name = selectedTileType.ToString() + "Preview";
            newTile.transform.parent = previewsHolder;
        }

        spawnedTile = newTile;
    }
    private bool IsRayHittingSomething(LayerMask layerMask, out RaycastHit hitInfo)
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hitInfo, maxDistance: 30, layerMask);
    }


    // PLACING MODE
    private void PlacingModeLogic()
    {
        if(testModeEnabled) return;
        if(IsPointerOverUI()) return;

        if(!deleteModeEnabled && spawnedTile == null) SelectTile(selectedTileType);
        if(spawnedTile == null) return;

        if(selectedTileType == TileTypes.None) 
        {
            DeleteObjectPreview(); 
            return;
        }

        PositionTilePreview();

        if(!deleteModeEnabled && currentTileType != selectedTileType && !firstTeleportPlaced)
        {
            currentTileType = selectedTileType;
            SelectTile(selectedTileType);
        }
    }
    
    private void PositionTilePreview()
    {
        if(IsRayHittingSomething(gridLayer, out RaycastHit hitInfo))
        {
            bool canPlace = true;

            int x = (int)hitInfo.transform.position.x;
            int z = (int)hitInfo.transform.position.z;
            
            if(x < 0 || z < 0 || x >= grid.LevelData.GridRows || z >= grid.LevelData.GridColumns) canPlace = false;

        
            if(tilesPlacer.TileIsBlock(x, z) || tilesPlacer.TileIsDice(x, z) || tilesPlacer.TileIsFlag(x, z) || tilesPlacer.TileIsPower(x, z) || tilesPlacer.TileIsTeleport(x, z)) canPlace = false;

            spawnedTile.transform.position = new Vector3(x, 0.5f, z);

            if(Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceTile();

                DeleteObjectPreview();
                SelectTile(selectedTileType);
            }
        } 
    }

    private void PlaceTile()
    {
        switch(selectedTileType)
        {
            case TileTypes.Block:
                GameObject block = Instantiate(blockData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                block.name = "Block";

                tilesPlacer.LevelData.Blocks.Add(new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z));
                break;

            case TileTypes.Dice:
                if(!hasDice)
                {
                    GameObject dice = Instantiate(diceData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    dice.name = "Dice";

                    hasDice = true;
                }
                else
                {
                    GameObject dice = previewsHolder.Find("Dice").gameObject;
                    dice.transform.position = spawnedTile.transform.position;
                }

                tilesPlacer.LevelData.DiceCoordinates = new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z);
                break;

            case TileTypes.Flag:
                if(!hasFlag)
                {
                    GameObject flag = Instantiate(flagData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    flag.name = "Flag";

                    hasFlag = true;
                }
                else
                {
                    GameObject flag = previewsHolder.Find("Flag").gameObject;
                    flag.transform.position = spawnedTile.transform.position;
                }

                tilesPlacer.LevelData.FlagCoordinates = new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z);
                break;

            case TileTypes.Power:
                GameObject power = Instantiate(powerData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                power.name = "Power";

                PowerTile newPowerTile = new PowerTile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z, powerType, powerValue);

                tilesPlacer.LevelData.Powers.Add(newPowerTile);
                powerTiles.Add(newPowerTile);
                break;

            case TileTypes.Teleport:

                if(firstTeleportPlaced)
                {
                    GameObject teleport = Instantiate(teleportData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    teleport.name = "Teleport";

                    TeleportTile firstTeleport =new TeleportTile((int)firstTeleportCoordinates.x, (int)firstTeleportCoordinates.y, (int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z);
                    TeleportTile secondTeleport = new TeleportTile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z, (int)firstTeleportCoordinates.x, (int)firstTeleportCoordinates.y);
                    
                    firstTeleport.PreviewObject = firstTeleportObject;
                    secondTeleport.PreviewObject = teleport;

                    tilesPlacer.LevelData.Teleports.Add(firstTeleport);
                    tilesPlacer.LevelData.Teleports.Add(secondTeleport);

                    teleportTiles.Add(firstTeleport);
                    teleportTiles.Add(secondTeleport);

                    firstTeleportPlaced = false;
                }
                else
                {
                    GameObject teleport = Instantiate(teleportData.Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    teleport.name = "Teleport";

                    firstTeleportObject = teleport;
                    firstTeleportCoordinates = new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z);
                    firstTeleportPlaced = true;
                }

                break;
        }
    }


    // DELETING MODE
    private void DeletingModeLogic()
    {
        if(testModeEnabled) return;
        if(IsPointerOverUI()) return;
        if(firstTeleportPlaced) return;

        if (spawnedTile != null) Destroy(spawnedTile.gameObject);
        
        if(IsRayHittingSomething(tilesLayer, out RaycastHit hitInfo))
        {
            if(hitInfo.collider.gameObject == null || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("GridTile")) return;
            
            TileTypes tileTypeToDestroy = tilesPlacer.FindTileType((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);

            if(Input.GetMouseButtonDown(0)) 
            {
                bool canDestroy = false;
                switch (tileTypeToDestroy)
                {
                    case TileTypes.Block:
                        tilesPlacer.LevelData.Blocks.Remove(new Vector2(hitInfo.transform.position.x, hitInfo.transform.position.z));
                        
                        canDestroy = true;

                        break;

                    case TileTypes.Dice:
                        tilesPlacer.LevelData.DiceCoordinates = new Vector2(-1, -1);
                        hasDice = false;

                        canDestroy = true;

                        break;

                    case TileTypes.Flag:
                        tilesPlacer.LevelData.FlagCoordinates = new Vector2(-1, -1);
                        hasFlag = false;

                        canDestroy = true;

                        break;

                    case TileTypes.Power:
                        PowerTile powerTile = tilesPlacer.GetPowerTile((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);
                        
                        tilesPlacer.LevelData.Powers.Remove(powerTile);
                        
                        powerTiles.Remove(powerTile);

                        canDestroy = true;

                        break;

                    case TileTypes.Teleport:
                        TeleportTile thisTile = tilesPlacer.GetTeleportTile((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);
                        TeleportTile otherTile = tilesPlacer.GetTeleportTile((int)thisTile.DestinationCoordinates.x, (int)thisTile.DestinationCoordinates.y);
                        
                        Destroy(otherTile.PreviewObject);

                        tilesPlacer.LevelData.Teleports.Remove(thisTile);
                        tilesPlacer.LevelData.Teleports.Remove(otherTile);

                        teleportTiles.Remove(thisTile);
                        teleportTiles.Remove(otherTile);

                        canDestroy = true;

                        break;

                    case TileTypes.None:
                        break;
                }
                if(canDestroy) Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    private void DeleteObjectPreview()
    {
        if (spawnedTile != null)
        {
            Destroy(spawnedTile);
            spawnedTile = null;
        }
    }

    public void SetDeletingMode()
    {
        if(!firstTeleportPlaced)
        {
            deleteModeEnabled = !deleteModeEnabled;

            UnityEngine.UI.Image image = GameObject.FindGameObjectWithTag("EditorUI").GetComponent<LevelEditorUI>().deleteTileButton.GetComponent<UnityEngine.UI.Image>();

            if(deleteModeEnabled) image.color = new Color32(32, 32, 32, 150);
            else image.color = new Color32(32, 32, 32, 75);
        }
        else ErrorsPanelScript.SendError.Invoke("Place the destination tile of the <color=purple>TELEPORT<color=red> tile");
    }

    // TESTING MODE
    public void TestEditor()
    {
        if(!testModeEnabled)
        {
            bool hasAll = true;
            if(!hasDice)
            {
                ErrorsPanelScript.SendError.Invoke("There is no <color=white>DICE<color=red> setted in the grid");
                hasAll = false;
            }
            if(!hasFlag)
            {
                ErrorsPanelScript.SendError.Invoke("There is no <color=white>FLAG<color=red> setted in the grid");
                hasAll = false;
            }
            
            if(hasAll)
            {
                testModeEnabled = true;
                
                previewsHolder.gameObject.SetActive(false);
                
                tilesPlacer.Init();
            }
        }
        else
        {
            tilesPlacer.DeleteTiles();
            //tilesPlacer.LevelData.Powers = powerTiles;
            //tilesPlacer.LevelData.Teleports = teleportTiles;

            previewsHolder.gameObject.SetActive(true);
            testModeEnabled = false;
        }
    }

    // UI
    public static bool IsPointerOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    // Tiles Settings Setters

    public void SetPowerValue(int newValue) 
    {
        powerValue = newValue;
    }

    public void SetPowerType(PowerTypeTest newType) 
    {
        powerType = newType;
    }
}
