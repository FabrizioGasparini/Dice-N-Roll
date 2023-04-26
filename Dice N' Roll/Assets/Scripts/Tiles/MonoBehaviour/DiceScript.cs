using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour
{
    [Header("Variables")]
    public int Row;
    public int Column;
    public int DiceValue;
    public bool GodMode;

    [Header("")]
    private bool isMoving = false;
    private bool inGame = true;

    [Header("References")]
    [SerializeField] private List<Sprite> diceSprites = new List<Sprite>();

    private Grid grid;
    private Transform faces;


    public void RollDice(Vector3 dir) 
    {
        Vector3 anchor = transform.position + (Vector3.down + dir) * 0.5f;
        Vector3 axis = Vector3.Cross(Vector3.up, dir);
        StartCoroutine(Roll(anchor, axis));
    }
    IEnumerator Roll(Vector3 anchor, Vector3 axis)
    {
        isMoving = true;

        for (int i = 0; i < (90 / 3); i++)
        {
            transform.RotateAround(anchor, axis, 3);
            yield return new WaitForSeconds(.01f);
        }

        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/DiceSound"));

        isMoving = false;
        yield return new WaitForSeconds(1);
        DestroyImmediate(audio, true);
    }
    
    void Start()
    {
        faces = transform.GetChild(0);
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        grid = grid.GetComponent<Grid>();
    }
    void Update()
    {
        ChangeNumber();

        if (DiceValue > 0){
            if(inGame)
            {
                KeyCode up = KeyCode.W;
                KeyCode down = KeyCode.S;
                KeyCode left = KeyCode.A;
                KeyCode right = KeyCode.D;

                if(PlayerPrefs.GetString("Up") != null && PlayerPrefs.GetString("Down") != null && PlayerPrefs.GetString("Left") != null && PlayerPrefs.GetString("Right") != null)
                {
                    up = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
                    down = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
                    left = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
                    right = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
                }

                if (Input.GetKey(left) || Input.GetKey(KeyCode.LeftArrow)) Move(Row, Column - 1, Vector3.back);
                
                else if (Input.GetKey(right) || Input.GetKey(KeyCode.RightArrow)) Move(Row, Column + 1, Vector3.forward);
                
                else if (Input.GetKey(up) || Input.GetKey(KeyCode.UpArrow)) Move(Row - 1, Column, Vector3.left);
                
                else if (Input.GetKey(down) || Input.GetKey(KeyCode.DownArrow))  Move(Row + 1, Column, Vector3.right);
                

                if(Row == grid.LevelData.FlagCoordinates.x && Column == grid.LevelData.FlagCoordinates.y) 
                {
                    GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                    
                    inGame = false;
                    DiceValue = 0;

                    if (grid.InMenu || grid.InEditor)
                    {
                        StartCoroutine(ResetDice());
                        return;
                    }
                    
                    GameObject.FindObjectOfType<UIScript>().CompleteLevel();
                }
            }
        }
        else
        {
            if(Row != grid.LevelData.FlagCoordinates.x || Column != grid.LevelData.FlagCoordinates.y)
            {
                if(inGame)
                {
                    GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                    
                    inGame = false;
                    DiceValue = 0;

                    if(grid.InMenu || grid.InEditor)
                    {
                        StartCoroutine(ResetDice());
                        return;
                    }

                    GameObject.FindObjectOfType<UIScript>().FailLevel();
                }
            } 
        } 
    }

    private void Move(int x, int y, Vector3 direction){
        if(!isMoving)
        {
            if(grid.GetTile(x, y))
            {
                TileType tileType = grid.GetTile(x, y).TileType;

                switch (tileType)
                {
                    case TileType.Block:
                        if(GodMode)
                        {
                            ResetTile(Row, Column);
                            RollDice(direction);

                            Row = x;
                            Column = y;
                        } 
                        
                        break;

                    case TileType.Power:
                        PowerTile powerTile = grid.GetPowerTile(x, y);

                        if(powerTile != null)
                        {
                            if(!GodMode)
                            {
                                grid.GetTile(x, y).TileType = TileType.Dice;
                                DiceValue -= 1;
                            }

                            ResetTile(Row, Column);

                            Row = x;
                            Column = y;

                            RollDice(direction);

                            switch (powerTile.PowerType)
                            {
                                case PowerType.Add:
                                    DiceValue += powerTile.Value;
                                    break;

                                case PowerType.Remove:
                                    DiceValue -= powerTile.Value;
                                    break;

                                case PowerType.Double:
                                    DiceValue *= 2;
                                    break;

                                case PowerType.Split:
                                    DiceValue = Mathf.RoundToInt(DiceValue / 2);
                                    break;
                            }

                            if(!GodMode) Destroy(powerTile.Object);
                        }
                        break;

                    case TileType.Flag:
                        if(!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);

                        if(grid.InMenu || grid.InEditor) break; 

                        GameObject.FindObjectOfType<UIScript>().CompleteLevel();

                        break;

                    case TileType.Teleport:
                        if(!GodMode) DiceValue -= 1;
                        
                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);
                        
                        TeleportTile teleportTile = grid.GetTeleportTile(x, y);
                        Teleport((int)teleportTile.DestinationCoordinates.x, (int)teleportTile.DestinationCoordinates.y);
                        
                        break;

                    case TileType.None:
                        if(!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);
                        grid.GetTile(x, y).TileType = TileType.Dice;

                        Row = x;
                        Column = y;

                        RollDice(direction);
                        break;

                    case TileType.GhostBlock:
                        if (GodMode)
                        {
                            ResetTile(Row, Column);
                            RollDice(direction);

                            Row = x;
                            Column = y;
                        }

                        break;

                    case TileType.Button:
                        if (!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);

                        ButtonTile buttonTile = grid.GetButtonTile(x, y);
                        if(buttonTile.ButtonType == ButtonType.Press)
                        {
                            GhostBlockTile ghostBlockTile = grid.GetGhostBlockTile((int)buttonTile.DestinationCoordinates.x, (int)buttonTile.Coordinates.y);
                            Destroy(buttonTile.Object);
                            Destroy(ghostBlockTile.Object);

                            grid.GetTile((int)buttonTile.DestinationCoordinates.x, (int)buttonTile.Coordinates.y).TileType = TileType.None;
                            grid.GetTile(x, y).TileType = TileType.None;
                        }

                        break;
                }
            }
        }
    }

    private void ResetTile(int x, int y)
    {
        if(grid.GetType(x, y) == TileType.Block) return;
        if(grid.GetType(x, y) == TileType.Flag) return;
        if(grid.GetType(x, y) == TileType.Teleport) return;
        if(grid.GetType(x, y) == TileType.GhostBlock) return;
        if(grid.GetType(x, y) == TileType.Button) return;
        if(grid.GetType(x, y) == TileType.Power && GodMode) return;

        grid.GetTile(x, y).TileType = TileType.None;
    }


    public void Teleport(int row, int column)
    {
        StartCoroutine(StartTeleport(row, column));
    }

    IEnumerator StartTeleport(int row, int column)
    {
        inGame = false;
        yield return new WaitForSeconds(1);

        transform.position = new Vector3(row, transform.position.y, column);

        Row = row;
        Column = column;

        inGame = true;
    }
    IEnumerator ResetDice()
    {
        yield return new WaitForSeconds(1);

        grid.DeleteTiles();
        grid.Init();
    }

    public void ChangeNumber()
    {
        switch(DiceValue)
        {
            default:
                SetFaceNumber(null);
                break;
            case 1:
                SetFaceNumber(diceSprites[0]);
                break;
            case 2:
                SetFaceNumber(diceSprites[1]);
                break;
            case 3:
                SetFaceNumber(diceSprites[2]);
                break;
            case 4:
                SetFaceNumber(diceSprites[3]);
                break;
            case 5:
                SetFaceNumber(diceSprites[4]);
                break;
            case 6:
                SetFaceNumber(diceSprites[5]);
                break;
        }
    }

    private void SetFaceNumber(Sprite sprite)
    {
        foreach (Transform face in faces)
        {
            face.GetComponent<Image>().sprite = sprite;

            if(sprite == null) face.GetComponent<Image>().color = Color.clear;
            else face.GetComponent<Image>().color = Color.white;
        }

    }

    public void SetGameStatus(bool value)
    { 
        inGame = value;
    }

    public bool GetGameStatus()
    {
        return inGame;
    }
}
