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
    }

    public void ResetData()
    {
        GridRows = 0;
        GridColumns = 0;

        DiceCoordinates = new Vector2(0, 0);
        DiceValue = 0;
        FlagCoordinates = new Vector2(0, 0);
        TilesList.BlockTiles = new List<Tile>();
        TilesList.PowerTiles = new List<PowerTile>();
        TilesList.TeleportTiles = new List<TeleportTile>();
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