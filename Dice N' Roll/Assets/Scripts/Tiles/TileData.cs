using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Tile")]
public class TileData : ScriptableObject
{
    public string Name;
    public TileTypes TileType;
    public GameObject Tile;
    public GameObject Preview;
}
