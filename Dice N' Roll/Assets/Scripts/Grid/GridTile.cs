using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypes
{
    None,
    Block,
    Dice,
    Flag,
    Power,
    Teleport
}

public class GridTile : MonoBehaviour
{
    public TileTypes TileType;
}
