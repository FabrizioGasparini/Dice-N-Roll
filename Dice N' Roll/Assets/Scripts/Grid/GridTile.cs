using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public TileTypes TileType;
}

[System.Serializable]
public class TeleportTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    public Vector2 DestinationCoordinates = new Vector2(-1, -1);
    [HideInInspector] public GameObject Object;
    [HideInInspector] public GameObject PreviewObject;

    public TeleportTile(int x, int y, int xD, int yD)
    {
        this.Coordinates = new Vector2(x, y);
        this.DestinationCoordinates = new Vector2(xD, yD);
    }
}

[System.Serializable]
public class PowerTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    public PowerType PowerType;
    [Range(0, 6)] public int Value;
    [HideInInspector] public GameObject Object;

    public PowerTile(int x, int y, PowerType powerType, int value)
    {
        this.Coordinates = new Vector2(x, y);
        this.PowerType = powerType;
        this.Value = value;
    }
}

[System.Serializable]
public class ButtonTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    //public ButtonType ButtonType;
    public bool Activated;
    [HideInInspector] public GameObject Object;

    /*public ButtonTile(int x, int y, ButtonType buttonType, bool activated)
    {
        this.Coordinates = new Vector2(x, y);
        this.ButtonType = buttonType;
        this.Activated = activated;
    }*/
}