using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public TileType TileType;
}

[System.Serializable]
public class Tile
{
    public Vector2 Coordinates = new Vector2(-1, -1);

    public Tile(int x, int y)
    {
        this.Coordinates = new Vector2(x, y);
    }
    public Tile(Vector2 Coordinates)
    {
        this.Coordinates = Coordinates;
    }
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
    public TeleportTile(Vector2 coordinates, Vector2 destinationCoordinates)
    {
        this.Coordinates = coordinates;
        this.DestinationCoordinates = destinationCoordinates;
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

    public PowerTile(Vector2 coordinates, PowerType powerType, int value)
    {
        this.Coordinates = coordinates;
        this.PowerType = powerType;
        this.Value = value;
    }
}

[System.Serializable]
public class ButtonTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    public Vector2 DestinationCoordinates = new Vector2(-1, -1);
    public ButtonType ButtonType;
    public bool Activated;
    [HideInInspector] public GameObject Object;

    public ButtonTile(int x, int y, int dX, int dY, ButtonType buttonType)
    {
        this.Coordinates = new Vector2(x, y);
        this.DestinationCoordinates = new Vector2(dX, dY);
        this.ButtonType = buttonType;
    }
    public ButtonTile(Vector2 coordinates, Vector2 destinationCoordinates, ButtonType buttonType)
    {
        this.Coordinates = coordinates;
        this.DestinationCoordinates = destinationCoordinates;
        this.ButtonType = buttonType;
    }
}

[System.Serializable]
public class GhostBlockTile
{
    public Vector2 Coordinates = new Vector2(-1, -1);
    [HideInInspector] public GameObject Object;

    public GhostBlockTile(int x, int y)
    {
        this.Coordinates = new Vector2(x, y);
    }
    public GhostBlockTile(Vector2 Coordinates)
    {
        this.Coordinates = Coordinates;
    }
}