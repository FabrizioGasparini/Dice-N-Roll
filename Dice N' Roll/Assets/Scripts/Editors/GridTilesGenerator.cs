#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GridTilesGenerator : EditorWindow
{
    
    private Vector2 scrollPos;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle titleStyle = new GUIStyle();


    private TileDataList tileDataList;
    private Grid grid;

    private Object level;
    
    private TileType selectedTile;
    private string[] tilesTypes = System.Enum.GetNames(typeof(TileType));

    private int powerValue = -1;

    private PowerType powerType;

    private bool teleportSelected = false;
    private int teleportX = -1;
    private int teleportY = -1;

    private bool buttonSelected = false;
    private int buttonX = -1;
    private int buttonY = -1;
    private ButtonType buttonType;

    private int selectedButton = -1;

    private int gridRows = 5;
    private int gridColumns = 5;

    private Vector2 diceCoordinates;
    private int diceValue = 6;
    private Vector2 flagCoordinates;
    private List<Tile> blockTiles = new List<Tile>();
    private List<PowerTile> powerTiles = new List<PowerTile>();
    private List<TeleportTile> teleportTiles = new List<TeleportTile>();
    private List<ButtonTile> buttonTiles = new List<ButtonTile>();
    private List<GhostBlockTile> ghostBlockTiles = new List<GhostBlockTile>();


    [MenuItem("Dice N' Roll/Grid Tiles Generator")]
    public static void ShowWindow()
    {
        GetWindow<GridTilesGenerator>("Grid Tiles Generator");
    }

    private void OnGUI()
    {
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;

        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 25;
        titleStyle.normal.textColor = Color.white;

        if(grid == null) grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        if(tileDataList == null) tileDataList = Resources.Load<TileDataList>("Tiles/Data/TileDataList");

        scrollPos =
            GUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label(" GRID TILES GENERATOR", titleStyle);
        GUILayout.Label("");

        GUILayout.BeginVertical();
        gridRows = EditorGUILayout.IntSlider(" Grid Rows: ", gridRows, 3, 15);
        GUILayout.Label("");

        gridColumns = EditorGUILayout.IntSlider(" Grid Columns: ", gridColumns, 3, 15);
        GUILayout.Label("");

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(30),GUILayout.Height(5));
        for (int a = 0; a < gridColumns; a++)
        {
            GUILayout.Label(a.ToString(), GUILayout.Width(50),GUILayout.Height(15));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(30),GUILayout.Height(5));
        for (int a = 0; a < gridColumns; a++)
        {
            GUILayout.Label("", GUILayout.Width(50),GUILayout.Height(5));
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.HSVToRGB(0, 0, .45f);

        for (int a = 0; a < gridRows; a++)
        {    
            GUILayout.BeginHorizontal();
            GUILayout.Label(a.ToString(), GUILayout.Width(10),GUILayout.Height(50));
            GUILayout.Label("", GUILayout.Width(1),GUILayout.Height(50));

            for (int b = 0; b < gridColumns; b++)
            {   

                string textButton = "";

                if(GetType(a, b) == TileType.Dice) GUI.backgroundColor = GetTileData("Dice").GizmoColor;
                if(GetType(a, b) == TileType.Flag) GUI.backgroundColor = GetTileData("Flag").GizmoColor;
                if(GetType(a, b) == TileType.Block) GUI.backgroundColor = GetTileData("Block").GizmoColor;
                if(GetType(a, b) == TileType.Button) GUI.backgroundColor = GetTileData("Button").GizmoColor;
                if(GetType(a, b) == TileType.GhostBlock) GUI.backgroundColor = GetTileData("Ghost Block").GizmoColor;

                if(GetType(a, b) == TileType.Teleport)
                {
                    GUI.backgroundColor = GetTileData("Teleport").GizmoColor;
                    textButton = GetTeleportTile(a,b).DestinationCoordinates.x.ToString() + ", " + GetTeleportTile(a,b).DestinationCoordinates.y.ToString();
                }

                if(GetType(a, b) == TileType.Button)
                {
                    GUI.backgroundColor = GetTileData("Button").GizmoColor;
                    textButton = GetButtonTile(a,b).DestinationCoordinates.x.ToString() + ", " + GetButtonTile(a, b).DestinationCoordinates.y.ToString();
                }

                if (GetType(a, b) == TileType.Power)
                {
                    GUI.backgroundColor = GetTileData("Power").GizmoColor;
                    
                    switch(powerType)
                    {
                        case PowerType.Add:
                            textButton = "+" + GetPowerTile(a,b).Value.ToString();
                            break;

                        case PowerType.Remove:
                            textButton = "-" + GetPowerTile(a,b).Value.ToString();
                            break;

                        case PowerType.Double:
                            textButton = "x2";
                            break;

                        case PowerType.Split:
                            textButton = "/2";
                            break;
                    }
                } 
                
                if (GUILayout.Button(textButton, GUILayout.Width(50),GUILayout.Height(50)))
                {
                    switch (selectedTile)
                    {
                        case TileType.None:
                            if (GetType(a, b) == TileType.Dice) diceCoordinates = new Vector2(-1, -1);
                            if (GetType(a, b) == TileType.Flag) flagCoordinates = new Vector2(-1, -1);

                            if (GetType(a, b) == TileType.Block) blockTiles.Remove(GetBlockTile(a, b));

                            if (GetType(a, b) == TileType.GhostBlock)
                            {
                                ghostBlockTiles.Remove(GetGhostBlockTile(a, b));
                                buttonTiles.Remove(GetButtonTileByDestination(a, b));
                            } 

                            if (GetType(a, b) == TileType.Power)
                            {
                                powerTiles.Remove(GetPowerTile(a, b));
                            } 

                            if(GetType(a, b) == TileType.Teleport)
                            {
                                int destX = (int)GetTeleportTile(a, b).DestinationCoordinates.x;
                                int destY = (int)GetTeleportTile(a, b).DestinationCoordinates.y;

                                teleportTiles.Remove(GetTeleportTile(a, b));
                                teleportTiles.Remove(GetTeleportTile(destX, destY));
                            }
                            
                            if(GetType(a, b) == TileType.Button)
                            {
                                int destX = (int)GetButtonTile(a, b).DestinationCoordinates.x;
                                int destY = (int)GetButtonTile(a, b).DestinationCoordinates.y;

                                buttonTiles.Remove(GetButtonTile(a, b));
                                ghostBlockTiles.Remove(GetGhostBlockTile(destX, destY));
                            } 
                            
                            break;

                        case TileType.Block:
                            if (GetType(a, b) != TileType.None) break;
                            blockTiles.Add(new Tile(a, b));
                    
                            break;

                        case TileType.Dice:
                            if (GetType(a, b) != TileType.None) break;
                            diceCoordinates = (new Vector2(a, b));
                            
                            break;
                            
                        case TileType.Flag:
                            if (GetType(a, b) != TileType.None) break;
                            flagCoordinates = (new Vector2(a, b));
                    
                            break;

                        case TileType.Power:
                            if (GetType(a, b) != TileType.None) break;
                            powerTiles.Add(new PowerTile(a, b, powerType, powerValue));

                            break;

                        case TileType.Teleport:
                            if (GetType(a, b) != TileType.None) break;

                            if(teleportSelected)
                            {
                                teleportSelected = false;

                                teleportTiles.Add(new TeleportTile(a, b, teleportX, teleportY));
                                teleportTiles.Add(new TeleportTile(teleportX, teleportY, a, b));

                                teleportX = -1;
                                teleportY = -1;
                            }
                            else
                            {
                                teleportSelected = true;
                                teleportX = a;
                                teleportY = b;
                            }

                            break;

                        case TileType.Button:
                            if(GetType(a, b) != TileType.None) break;

                            if (buttonSelected)
                            {
                                buttonSelected = false;

                                buttonTiles.Add(new ButtonTile(buttonX, buttonY, a, b, buttonType));
                                ghostBlockTiles.Add(new GhostBlockTile(a, b));

                                buttonX = -1;
                                buttonY = -1;
                            }
                            else
                            {
                                buttonSelected = true;
                                buttonX = a;
                                buttonY = b;
                            }

                            break;
                    }
                }
                GUI.backgroundColor = Color.HSVToRGB(0, 0, .45f);
            }
            GUILayout.EndHorizontal();
        }
        GUI.backgroundColor = Color.HSVToRGB(0, 0, .45f);

        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.BeginHorizontal();
    
        for(int i = 0; i < tilesTypes.Length; i++)
        {
            if(tilesTypes[i] == "GhostBlock") break;
            if(selectedButton == i) GUI.backgroundColor = Color.HSVToRGB(0, 0, .65f);
            if (GUILayout.Button(tilesTypes[i], GUILayout.Height(30)))
            {
                if(!teleportSelected && !buttonSelected)
                {
                    selectedTile = (TileType)System.Enum.Parse(typeof(TileType), tilesTypes[i]);
                    selectedButton = i;
                }
            }
            GUI.backgroundColor = Color.HSVToRGB(0, 0, .45f);
        }

        GUILayout.EndHorizontal();
        
        if(selectedButton == 2)
        {
            GUILayout.Label("");
            GUILayout.Label(" Dice Variables", guiStyle);
            GUILayout.Label("");

            diceValue = EditorGUILayout.IntSlider(" Power Value: ", diceValue, 1, 6);
        }

        if(selectedButton == 4)
        {
            GUILayout.Label("");
            GUILayout.Label(" Power Variables", guiStyle);
            GUILayout.Label("");

            powerValue = EditorGUILayout.IntSlider(" Power Value: ", powerValue, 0, 6);
            powerType = (PowerType)EditorGUILayout.EnumPopup(" Power Type: ", powerType);
        }

        if(selectedButton == 6)
        {
            GUILayout.Label("");
            GUILayout.Label(" Button Variables", guiStyle);
            GUILayout.Label("");

            buttonType = (ButtonType)EditorGUILayout.EnumPopup(" Button Type: ", buttonType);
        }

        GUILayout.Label("");
        GUILayout.Label("");
        if (GUILayout.Button("RESET TILES", GUILayout.Height(50)))
        {
            diceCoordinates = new Vector2(-1, -1);
            flagCoordinates = new Vector2(-1, -1);
            blockTiles = new List<Tile>();
            powerTiles = new List<PowerTile>();
            teleportTiles = new List<TeleportTile>();
            buttonTiles = new List<ButtonTile>();
            ghostBlockTiles = new List<GhostBlockTile>();
        }
        GUILayout.Label("");

        level = EditorGUILayout.ObjectField(" Level To Override: ", level, typeof(LevelData), true);
        
        GUILayout.Label("");
        if (GUILayout.Button("GENERATE LEVEL DATA", GUILayout.Height(50)))
        {
            var newLevel = GenerateLevelData();

            if(newLevel.name == SceneManager.GetActiveScene().name) grid.LevelData = newLevel;
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }


    private TileData GetTileData(string tileName)
    {
        foreach (var tile in tileDataList.TilesList) if (tile.Name == tileName) return tile;

        return null;
    }

    public TileType GetType(int x, int y)
    {
        foreach (Tile tile in blockTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Block;

        if (diceCoordinates == new Vector2(x, y)) return TileType.Dice;

        if (flagCoordinates == new Vector2(x, y)) return TileType.Flag;

        foreach (PowerTile tile in powerTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Power;

        foreach (TeleportTile tile in teleportTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Teleport;

        foreach (ButtonTile tile in buttonTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.Button;

        foreach (GhostBlockTile tile in ghostBlockTiles) if (tile.Coordinates == new Vector2(x, y)) return TileType.GhostBlock;

        return TileType.None;
    }

    public Tile GetBlockTile(int x, int y)
    {
        foreach (Tile tile in blockTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }
    public PowerTile GetPowerTile(int x, int y)
    {
        foreach (PowerTile tile in powerTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public TeleportTile GetTeleportTile(int x, int y)
    {
        foreach (TeleportTile tile in teleportTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public ButtonTile GetButtonTile(int x, int y)
    {
        foreach (ButtonTile tile in buttonTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public ButtonTile GetButtonTileByDestination(int x, int y)
    {
        foreach (ButtonTile tile in buttonTiles) if (tile.DestinationCoordinates == new Vector2(x, y)) return tile;

        return null;
    }

    public GhostBlockTile GetGhostBlockTile(int x, int y)
    {
        foreach (GhostBlockTile tile in ghostBlockTiles) if (tile.Coordinates == new Vector2(x, y)) return tile;

        return null;
    }


    public LevelData GenerateLevelData()
    {
        var levelData = (LevelData)level;
        //levelData.Override(gridRows, gridColumns, diceCoordinates, diceValue, flagCoordinates, blockTiles, powerTiles, teleportTiles, buttonTiles, ghostBlockTiles);
        
        return levelData;
    }
}
#endif