using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Level"), System.Serializable]
public class LevelData : ScriptableObject
{
    [Header("Grid Settings")]
    public int GridRows;
    public int GridColumns;

    [Header("Tiles Settings")]
    public Vector2 DiceCoordinates;
    public int DiceValue;
    public Vector2 FlagCoordinates;
    public TilesList TilesList;

    public void Override(LevelData levelData)
    {
        GridRows = levelData.GridRows;
        GridColumns = levelData.GridColumns;

        DiceCoordinates = levelData.DiceCoordinates;
        DiceValue = levelData.DiceValue;
        FlagCoordinates = levelData.FlagCoordinates;
        TilesList.BlockTiles = levelData.TilesList.BlockTiles;
        TilesList.PowerTiles = levelData.TilesList.PowerTiles;
        TilesList.TeleportTiles = levelData.TilesList.TeleportTiles;
        TilesList.ButtonTiles = levelData.TilesList.ButtonTiles;
        TilesList.GhostBlockTiles = levelData.TilesList.GhostBlockTiles;
    }

    public void Override(int gridRows, int gridColumns, Vector2 diceCoordinates, int diceValue, Vector2 flagCoordinates, List<Tile> blockTiles, List<PowerTile> powerTiles, List<TeleportTile> teleportTiles, List<ButtonTile> buttonTiles, List<GhostBlockTile> ghostBlockTiles)
    {
        GridRows = gridRows;
        GridColumns = gridColumns;

        DiceCoordinates = diceCoordinates;
        DiceValue = diceValue;
        FlagCoordinates = flagCoordinates;
        TilesList.BlockTiles = blockTiles;
        TilesList.PowerTiles = powerTiles;
        TilesList.TeleportTiles = teleportTiles;
        TilesList.ButtonTiles = buttonTiles;
        TilesList.GhostBlockTiles = ghostBlockTiles;
    }
}

[System.Serializable]
public class TilesList
{
    public List<Tile> BlockTiles;
    public List<PowerTile> PowerTiles;
    public List<TeleportTile> TeleportTiles;
    public List<ButtonTile> ButtonTiles;
    public List<GhostBlockTile> GhostBlockTiles;
}