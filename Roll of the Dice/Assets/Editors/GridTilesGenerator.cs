#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class GridTilesGenerator : EditorWindow
{
    
    private Vector2 scrollPos;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle titleStyle = new GUIStyle();


    private Grid grid;


    private TileTypes selectedTile;
    private string[] tilesTypes = System.Enum.GetNames(typeof(TileTypes));

    private string levelName;

    private int powerValue = -1;

    private PowerTypeTest powerType;

    private bool teleportSelected = false;
    private int teleportX = -1;
    private int teleportY = -1;

    private int selectedButton = -1;

    private int gridRows = 5;
    private int gridColumns = 5;

    private Vector2 diceCoordinates;
    private int diceValue = 6;
    private Vector2 flagCoordinates;
    private List<Vector2> blockTiles = new List<Vector2>();
    private List<PowerTile> powerTiles = new List<PowerTile>();
    private List<TeleportTile> teleportTiles = new List<TeleportTile>();


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

        scrollPos =
            GUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("");
        GUILayout.Label(" GRID TILES GENERATOR", titleStyle);
        GUILayout.Label("");

        GUILayout.BeginVertical();
        levelName = EditorGUILayout.TextField(" Level Name", levelName);
        GUILayout.Label("");

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

                if(TileIsDice(a,b)) GUI.backgroundColor = Color.white;
                if(TileIsFlag(a,b)) GUI.backgroundColor = Color.black;
                if(TileIsBlock(a,b)) GUI.backgroundColor = Color.red;
                if(TileIsTeleport(a,b))
                {
                    GUI.backgroundColor = Color.magenta;
                    textButton = GetTeleportTile(a,b).DestinationCoordinates.x.ToString() + ", " + GetTeleportTile(a,b).DestinationCoordinates.y.ToString();
                } 

                if(TileIsPower(a,b))
                {
                    GUI.backgroundColor = Color.blue;
                    
                    switch(powerType)
                    {
                        case PowerTypeTest.Add:
                            textButton = "+" + GetPowerTile(a,b).Value.ToString();
                            break;

                        case PowerTypeTest.Remove:
                            textButton = "-" + GetPowerTile(a,b).Value.ToString();
                            break;

                        case PowerTypeTest.Double:
                            textButton = "x2";
                            break;

                        case PowerTypeTest.Split:
                            textButton = "/2";
                            break;
                    }
                } 
                
                if (GUILayout.Button(textButton, GUILayout.Width(50),GUILayout.Height(50)))
                {
                    switch (selectedTile)
                    {
                        case TileTypes.None:
                            if(TileIsDice(a, b)) diceCoordinates = new Vector2(-1, -1);
                            if(TileIsFlag(a, b)) flagCoordinates = new Vector2(-1, -1);
                            if(TileIsBlock(a, b)) blockTiles.Remove(new Vector2(a, b));
                            
                            if(TileIsPower(a, b))
                            {
                                powerTiles.Remove(GetPowerTile(a, b));
                            } 

                            if(TileIsTeleport(a, b))
                            {
                                int destX = (int)GetTeleportTile(a, b).DestinationCoordinates.x;
                                int destY = (int)GetTeleportTile(a, b).DestinationCoordinates.y;

                                teleportTiles.Remove(GetTeleportTile(a, b));
                                teleportTiles.Remove(GetTeleportTile(destX, destY));
                            } 
                            
                            break;

                        case TileTypes.Block:
                            if(TileIsDice(a, b) || TileIsFlag(a, b) || TileIsBlock(a, b) || TileIsPower(a, b) || TileIsTeleport(a, b)) break;
                            blockTiles.Add(new Vector2(a, b));
                    
                            break;

                        case TileTypes.Dice:
                            if(TileIsDice(a, b) || TileIsFlag(a, b) || TileIsBlock(a, b) || TileIsPower(a, b) || TileIsTeleport(a, b)) break;
                            diceCoordinates = (new Vector2(a, b));
                            
                            break;
                            
                        case TileTypes.Flag:
                            if(TileIsDice(a, b) || TileIsFlag(a, b) || TileIsBlock(a, b) || TileIsPower(a, b) || TileIsTeleport(a, b)) break;
                            flagCoordinates = (new Vector2(a, b));
                    
                            break;

                        case TileTypes.Power:
                            if(TileIsDice(a, b) || TileIsFlag(a, b) || TileIsBlock(a, b) || TileIsPower(a, b) || TileIsTeleport(a, b)) break;
                            powerTiles.Add(new PowerTile(a, b, powerType, powerValue));

                            break;

                        case TileTypes.Teleport:
                            if(TileIsDice(a, b) || TileIsFlag(a, b) || TileIsBlock(a, b) || TileIsPower(a, b) || TileIsTeleport(a, b)) break;

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
            if(selectedButton == i) GUI.backgroundColor = Color.HSVToRGB(0, 0, .65f);
            if (GUILayout.Button(tilesTypes[i], GUILayout.Height(30)))
            {
                if(!teleportSelected)
                {
                    selectedTile = (TileTypes)System.Enum.Parse(typeof(TileTypes), tilesTypes[i]);
                    selectedButton = i;
                }
            }
            GUI.backgroundColor =Color.HSVToRGB(0, 0, .45f);
        }

        GUILayout.EndHorizontal();
        
        if(selectedButton == 4)
        {
            GUILayout.Label("");
            GUILayout.Label(" Power Variables", guiStyle);
            GUILayout.Label("");

            powerValue = EditorGUILayout.IntSlider(" Power Value: ", powerValue, 0, 6);
            powerType = (PowerTypeTest)EditorGUILayout.EnumPopup(" Power Type: ", powerType);
        }

        if(selectedButton == 2)
        {
            GUILayout.Label("");
            GUILayout.Label(" Dice Variables", guiStyle);
            GUILayout.Label("");

            diceValue = EditorGUILayout.IntSlider(" Power Value: ", diceValue, 1, 6);
        }


        GUILayout.Label("");
        GUILayout.Label("");
        if (GUILayout.Button("RESET TILES", GUILayout.Height(50)))
        {
            diceCoordinates = new Vector2(-1, -1);
            flagCoordinates = new Vector2(-1, -1);
            blockTiles = new List<Vector2>();
            powerTiles = new List<PowerTile>();
            teleportTiles = new List<TeleportTile>();
        }
        GUILayout.Label("");
        if (GUILayout.Button("GENERATE LEVEL DATA", GUILayout.Height(50)))
        {
            grid.LevelData = GenerateLevelData();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }



    public bool TileIsBlock(int x, int y)
    {
        foreach (Vector2 tile in blockTiles) if (tile == new Vector2(x, y)) return true;

        return false;
    }

    public bool TileIsDice(int x, int y)
    {
        if (diceCoordinates == new Vector2(x, y)) return true;

        return false;
    }

    public bool TileIsFlag(int x, int y)
    {
        if (flagCoordinates == new Vector2(x, y)) return true;

        return false;
    }
    
    public bool TileIsPower(int x, int y)
    {
        foreach (PowerTile tile in powerTiles) if (tile.Coordinates == new Vector2(x, y)) return true;

        return false;
    }

    public bool TileIsTeleport(int x, int y)
    {
        foreach (TeleportTile tile in teleportTiles) if (tile.Coordinates == new Vector2(x, y)) return true;

        return false;
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


    public LevelData GenerateLevelData()
    {
        if(levelName == "") levelName = SceneManager.GetActiveScene().name;
        string path = "Levels/" + levelName;

        if (Resources.Load<LevelData>(path) != null)
        {
            LevelData data = Resources.Load<LevelData>(path);

            data.DiceCoordinates = diceCoordinates;
            data.DiceValue = diceValue;
            data.FlagCoordinates = flagCoordinates;
            data.Blocks = blockTiles;
            data.Powers = powerTiles;
            data.Teleports = teleportTiles;
            data.GridRows = gridRows;
            data.GridColumns = gridColumns;
        }
        else
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

            levelData.DiceCoordinates = diceCoordinates;
            levelData.DiceValue = diceValue;
            levelData.FlagCoordinates = flagCoordinates;
            levelData.Blocks = blockTiles;
            levelData.Powers = powerTiles;
            levelData.Teleports = teleportTiles;
            levelData.GridRows = gridRows;
            levelData.GridColumns = gridColumns;

            AssetDatabase.CreateAsset(levelData, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        var level = Resources.Load<LevelData>(path);
        
        return level;
    }
}
#endif