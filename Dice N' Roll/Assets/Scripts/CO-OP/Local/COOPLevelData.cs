using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/CO-OP Level"), System.Serializable]
public class COOPLevelData : ScriptableObject
{
    [Header("Grid Settings")]
    public int GridRows;
    public int GridColumns;

    [Header("Tiles Settings")]
    public Vector2 P1DiceCoordinates;
    public Vector2 P2DiceCoordinates;
    public int P1DiceValue;
    public int P2DiceValue;
    public Vector2 P1FlagCoordinates;
    public Vector2 P2FlagCoordinates;
    public TilesList TilesList;
    public bool OnlineLevel;

    [Header("Menu Settings")]
    public Sprite LevelSprite;

    public void Override(COOPLevelData levelData)
    {
        GridRows = levelData.GridRows;
        GridColumns = levelData.GridColumns;

        P1DiceCoordinates = levelData.P1DiceCoordinates;
        P2DiceCoordinates = levelData.P2DiceCoordinates;

        P1DiceValue = levelData.P1DiceValue;
        P2DiceValue = levelData.P2DiceValue;

        P1FlagCoordinates = levelData.P1FlagCoordinates;
        P2FlagCoordinates = levelData.P2FlagCoordinates;

        TilesList.BlockTiles = levelData.TilesList.BlockTiles;
        TilesList.PowerTiles = levelData.TilesList.PowerTiles;
        TilesList.TeleportTiles = levelData.TilesList.TeleportTiles;
        TilesList.ButtonTiles = levelData.TilesList.ButtonTiles;
        TilesList.GhostBlockTiles = levelData.TilesList.GhostBlockTiles;
    }

    public void Override(int gridRows, int gridColumns, Vector2 p1DiceCoordinates, Vector2 p2DiceCoordinates, int p1DiceValue, int p2DiceValue, Vector2 p1FlagCoordinates, Vector2 p2FlagCoordinates, List<Tile> blockTiles, List<PowerTile> powerTiles, List<TeleportTile> teleportTiles, List<ButtonTile> buttonTiles, List<GhostBlockTile> ghostBlockTiles)
    {
        GridRows = gridRows;
        GridColumns = gridColumns;

        P1DiceCoordinates = p1DiceCoordinates;
        P2DiceCoordinates = p2DiceCoordinates;
        
        P1DiceValue = p1DiceValue;
        P2DiceValue = p2DiceValue;
        P1FlagCoordinates = p1FlagCoordinates;
        P1FlagCoordinates = p2FlagCoordinates;
        TilesList.BlockTiles = blockTiles;
        TilesList.PowerTiles = powerTiles;
        TilesList.TeleportTiles = teleportTiles;
        TilesList.ButtonTiles = buttonTiles;
        TilesList.GhostBlockTiles = ghostBlockTiles;
    }
}
