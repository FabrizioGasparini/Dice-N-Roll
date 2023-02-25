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
    public List<Vector2> Blocks;
    public List<PowerTile> Powers;
    public List<TeleportTile> Teleports;

    public void Override(LevelData levelData)
    {
        GridRows = levelData.GridRows;
        GridColumns = levelData.GridColumns;

        DiceCoordinates = levelData.DiceCoordinates;
        DiceValue = levelData.DiceValue;
        FlagCoordinates = levelData.FlagCoordinates;
        Blocks = levelData.Blocks;
        Powers = levelData.Powers;
        Teleports = levelData.Teleports;
    }

    public void Reset()
    {
        GridRows = 0;
        GridColumns = 0;

        DiceCoordinates = new Vector2(0, 0);
        DiceValue = 0;
        FlagCoordinates = new Vector2(0, 0);
        Blocks = new List<Vector2>();
        Powers = new List<PowerTile>();
        Teleports = new List<TeleportTile>();
    }
}
