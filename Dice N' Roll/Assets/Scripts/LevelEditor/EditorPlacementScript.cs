using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorPlacementScript : MonoBehaviour
{
    [Header("Tiles Data")]
    [SerializeField] private TileDataList tilesList;
    [HideInInspector] public TileType selectedTileType;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private LayerMask tilesLayer;

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
    private TileType currentTileType;

    private bool hasDice;
    private bool hasFlag;


    private List<PowerTile> powerTiles = new List<PowerTile>();

    private int powerValue = 1;
    private PowerType powerType = PowerType.Add;

    private List<TeleportTile> teleportTiles = new List<TeleportTile>();

    [HideInInspector] public bool firstTeleportPlaced;
    private Vector2 firstTeleportCoordinates;
    private GameObject firstTeleportObject;

    [HideInInspector] public bool buttonPlaced;
    private Vector2 buttonCoordinates;
    private GameObject buttonObject;
    private ButtonType buttonType;

    // MAIN
    void Awake() 
    {
        mainCam = Camera.main;
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        tilesPlacer = grid.GetComponent<Grid>();
        previewsHolder = transform.GetChild(0);
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

        foreach (Tile block in grid.LevelData.TilesList.BlockTiles)
        {
            Vector3 position = grid.startPosition + new Vector3(block.Coordinates.x, 1, block.Coordinates.y);
            var newBlock = Instantiate(GetTileData("Block").Preview, position, Quaternion.identity, previewsHolder);
            newBlock.name = "Block";
        }
        foreach (PowerTile power in grid.LevelData.TilesList.PowerTiles)
        {
            Vector3 position = grid.startPosition + new Vector3(power.Coordinates.x, 1, power.Coordinates.y);
            var newPower = Instantiate(GetTileData("Power").Preview, position, Quaternion.identity, previewsHolder);
            newPower.name = "Power";
        }
        foreach (TeleportTile teleport in grid.LevelData.TilesList.TeleportTiles)
        {
            Vector3 position = grid.startPosition + new Vector3(teleport.Coordinates.x, 1, teleport.Coordinates.y);
            var newTeleport = Instantiate(GetTileData("Teleport").Preview, position, Quaternion.identity, previewsHolder);
            newTeleport.name = "Teleport";
        
            teleport.PreviewObject = newTeleport;
        }

        foreach (ButtonTile button in grid.LevelData.TilesList.ButtonTiles)
        {
            Vector3 position = new Vector3(button.Coordinates.x, .125f, button.Coordinates.y);
            GameObject newButton = Instantiate(GetTileData("Button").Preview, position, Quaternion.identity, previewsHolder);
            newButton.name = "Button";

            button.Object = newButton;
        }

        foreach (GhostBlockTile ghostBlock in grid.LevelData.TilesList.GhostBlockTiles)
        {
            Vector3 position = new Vector3(ghostBlock.Coordinates.x, .5f, ghostBlock.Coordinates.y);
            GameObject newBlock = Instantiate(GetTileData("Ghost Block").Preview, position, Quaternion.identity, previewsHolder);
            newBlock.name = "Ghost Block";

            ghostBlock.Object = newBlock;
        }

        if(grid.LevelData.DiceCoordinates != new Vector2(-1, -1))
        {
            Vector3 position = grid.startPosition + new Vector3(grid.LevelData.DiceCoordinates.x, 1, grid.LevelData.DiceCoordinates.y);
            var newDice = Instantiate(GetTileData("Dice").Preview, position, Quaternion.identity, previewsHolder);
            newDice.name = "Dice";
            hasDice = true;
        }
        if (grid.LevelData.FlagCoordinates != new Vector2(-1, -1))
        {
            Vector3 position = grid.startPosition + new Vector3(grid.LevelData.FlagCoordinates.x, 1, grid.LevelData.FlagCoordinates.y);
            var newFlag = Instantiate(GetTileData("Flag").Preview, position, Quaternion.identity, previewsHolder);
            newFlag.name = "Flag";
            hasFlag = true;
        }
    }

    public void ResetTiles()
    {
        if(!previewsHolder) return; 
        if(previewsHolder.childCount > 0) foreach (Transform child in previewsHolder) if(child.gameObject) Destroy(child.gameObject);
    }

    public void SelectTileType(TileType tileType)
    {
        if(!firstTeleportPlaced)
        {
            if(deleteModeEnabled) SetDeletingMode();
            selectedTileType = tileType;
        }
        else ErrorsPanelScript.SendError.Invoke("Place the destination tile of the <color=purple>TELEPORT<color=red> tile");
    }

    private void SelectTile(TileType tileType)
    {
        selectedTileType = tileType;

        if(deleteModeEnabled)
        {
            deleteModeEnabled = false;
        }

        DeleteObjectPreview();

        GameObject newTile = null;

        if(selectedTileType != TileType.None)
        {
            switch(selectedTileType)
            {
                case TileType.Block:
                    newTile = Instantiate(GetTileData("Block").Preview);
                    break;

                case TileType.Dice:
                    newTile = Instantiate(GetTileData("Dice").Preview);
                    break;

                case TileType.Flag:
                    newTile = Instantiate(GetTileData("Flag").Preview);
                    break;

                case TileType.Power:
                    newTile = Instantiate(GetTileData("Power").Preview);
                    break;

                case TileType.Teleport:
                    newTile = Instantiate(GetTileData("Teleport").Preview);
                    break;

                case TileType.Button:
                    newTile = Instantiate(GetTileData("Button").Preview);
                    break;

                case TileType.GhostBlock:
                    newTile = Instantiate(GetTileData("Ghost").Preview);
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

        if(selectedTileType == TileType.None) 
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

        
            if(tilesPlacer.GetType(x, z) != TileType.None) canPlace = false;

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
            case TileType.Block:
                GameObject block = Instantiate(GetTileData("Block").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                block.name = "Block";

                tilesPlacer.LevelData.TilesList.BlockTiles.Add(new Tile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z));
                break;

            case TileType.Dice:
                if(!hasDice)
                {
                    GameObject dice = Instantiate(GetTileData("Dice").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
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

            case TileType.Flag:
                if(!hasFlag)
                {
                    GameObject flag = Instantiate(GetTileData("Flag").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
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

            case TileType.Power:
                GameObject power = Instantiate(GetTileData("Power").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                power.name = "Power";

                PowerTile newPowerTile = new PowerTile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z, powerType, powerValue);

                tilesPlacer.LevelData.TilesList.PowerTiles.Add(newPowerTile);
                powerTiles.Add(newPowerTile);
                break;

            case TileType.Teleport:

                if(firstTeleportPlaced)
                {
                    GameObject teleport = Instantiate(GetTileData("Teleport").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    teleport.name = "Teleport";

                    TeleportTile firstTeleport =new TeleportTile((int)firstTeleportCoordinates.x, (int)firstTeleportCoordinates.y, (int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z);
                    TeleportTile secondTeleport = new TeleportTile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z, (int)firstTeleportCoordinates.x, (int)firstTeleportCoordinates.y);
                    
                    firstTeleport.PreviewObject = firstTeleportObject;
                    secondTeleport.PreviewObject = teleport;

                    tilesPlacer.LevelData.TilesList.TeleportTiles.Add(firstTeleport);
                    tilesPlacer.LevelData.TilesList.TeleportTiles.Add(secondTeleport);

                    teleportTiles.Add(firstTeleport);
                    teleportTiles.Add(secondTeleport);

                    firstTeleportPlaced = false;
                }
                else
                {
                    GameObject teleport = Instantiate(GetTileData("Teleport").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    teleport.name = "Teleport";

                    firstTeleportObject = teleport;
                    firstTeleportCoordinates = new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z);
                    firstTeleportPlaced = true;
                }

                break;

            case TileType.Button:

                if(buttonPlaced)
                {
                    GameObject ghostBlock = Instantiate(GetTileData("Ghost Block").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    ghostBlock.name = "Ghost Block";

                    ButtonTile buttonTile = new ButtonTile((int)buttonCoordinates.x, (int)buttonCoordinates.y, (int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z, buttonType);
                    GhostBlockTile ghostBlockTile = new GhostBlockTile((int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.z);
                    
                    buttonTile.Object = buttonObject;
                    ghostBlockTile.Object = ghostBlock;

                    tilesPlacer.LevelData.TilesList.ButtonTiles.Add(buttonTile);
                    tilesPlacer.LevelData.TilesList.GhostBlockTiles.Add(ghostBlockTile);

                    buttonPlaced = false;
                }
                else
                {
                    GameObject button = Instantiate(GetTileData("Button").Preview, spawnedTile.transform.position, Quaternion.identity, previewsHolder);
                    button.name = "Button";

                    buttonObject = button;
                    buttonCoordinates = new Vector2(spawnedTile.transform.position.x, spawnedTile.transform.position.z);
                    buttonPlaced = true;
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
            
            TileType tileTypeToDestroy = tilesPlacer.GetType((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);

            if(Input.GetMouseButtonDown(0)) 
            {
                bool canDestroy = false;
                switch (tileTypeToDestroy)
                {
                    case TileType.Block:
                        tilesPlacer.LevelData.TilesList.BlockTiles.Remove(new Tile((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z));
                        
                        canDestroy = true;

                        break;

                    case TileType.Dice:
                        tilesPlacer.LevelData.DiceCoordinates = new Vector2(-1, -1);
                        hasDice = false;

                        canDestroy = true;

                        break;

                    case TileType.Flag:
                        tilesPlacer.LevelData.FlagCoordinates = new Vector2(-1, -1);
                        hasFlag = false;

                        canDestroy = true;

                        break;

                    case TileType.Power:
                        PowerTile powerTile = tilesPlacer.GetPowerTile((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);
                        
                        tilesPlacer.LevelData.TilesList.PowerTiles.Remove(powerTile);
                        
                        powerTiles.Remove(powerTile);

                        canDestroy = true;

                        break;

                    case TileType.Teleport:
                        TeleportTile thisTile = tilesPlacer.GetTeleportTile((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z);
                        TeleportTile otherTile = tilesPlacer.GetTeleportTile((int)thisTile.DestinationCoordinates.x, (int)thisTile.DestinationCoordinates.y);
                        
                        Destroy(otherTile.PreviewObject);

                        tilesPlacer.LevelData.TilesList.TeleportTiles.Remove(thisTile);
                        tilesPlacer.LevelData.TilesList.TeleportTiles.Remove(otherTile);

                        teleportTiles.Remove(thisTile);
                        teleportTiles.Remove(otherTile);

                        canDestroy = true;

                        break;

                    case TileType.None:
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

    private TileData GetTileData(string tileName)
    {
        if(!tilesList) return null;
        
        foreach (var tile in tilesList.TilesList)
        {
            if (tile.Name == tileName)
            {
                return tile;
            }
        }

        return null;
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

    public void SetPowerType(PowerType newType) 
    {
        powerType = newType;
    }
    public void SetButtonType(ButtonType newType) 
    {
        buttonType = newType;
    }
}
