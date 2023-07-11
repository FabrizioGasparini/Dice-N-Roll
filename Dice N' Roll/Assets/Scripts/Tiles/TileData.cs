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
    public float GizmoVerticalPosition;
    public Color GizmoColor;
}

public enum TileType
{
    None,
    Block,
    Dice,
    Flag,
    Power,
    Teleport,
    Button,
    GhostBlock
}
