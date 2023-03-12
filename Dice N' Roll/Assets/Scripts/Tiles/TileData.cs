using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Tile")]
public class TileData : ScriptableObject
{
    [Header("Tile Variables")]
    public string Name;

    [Header("References")]
    public GameObject Tile;
    public GameObject Preview;

    [Header("Gizmos Variables")]
    public Vector3 GizmoSize;
    public Color32 GizmoColor;
}

public enum TileTypes
{
    None,
    Block,
    Dice,
    Flag,
    Power,
    Teleport,
    Button
}