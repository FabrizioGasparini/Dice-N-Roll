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
                    GetComponent<MeshRenderer>().material.color = Color.green;
                    
                    inGame = false;
                    DiceValue = 0;

                    if (grid.InMenu || grid.InEditor)
                    {
                        StartCoroutine(ResetDice());
                        return;
                    }
                    
                    GameObject.FindGameObjectWithTag("LevelUI").GetComponent<UIScript>().CompleteLevel();
                }
            }
        }
        else
        {
            if(Row != grid.LevelData.FlagCoordinates.x || Column != grid.LevelData.FlagCoordinates.y)
            {
                if(inGame)
                {
                    GetComponent<MeshRenderer>().material.color = Color.red;
                    
                    inGame = false;
                    DiceValue = 0;

                    if(grid.InMenu || grid.InEditor)
                    {
                        StartCoroutine(ResetDice());
                        return;
                    }

                    GameObject.FindGameObjectWithTag("LevelUI").GetComponent<UIScript>().FailLevel();
                }
            } 
        } 
    }

    private void Move(int x, int y, Vector3 direction){
        if(!isMoving)
        {
            if(grid.GetTile(x, y))
            {
                TileTypes tileType = grid.GetTile(x, y).TileType;

                switch (tileType)
                {
                    case TileTypes.Block:
                        if(GodMode)
                        {
                            ResetTile(Row, Column);
                            RollDice(direction);

                            Row = x;
                            Column = y;
                        } 
                        
                        break;

                    case TileTypes.Power:
                        PowerTile powerTile = grid.GetPowerTile(x, y);

                        if(powerTile != null)
                        {
                            if(!GodMode)
                            {
                                grid.GetTile(x, y).TileType = TileTypes.Dice;
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

                    case TileTypes.Flag:
                        if(!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);

                        if(grid.InMenu || grid.InEditor) break; 

                        GameObject.FindGameObjectWithTag("LevelUI").GetComponent<UIScript>().CompleteLevel();

                        break;

                    case TileTypes.Teleport:
                        if(!GodMode) DiceValue -= 1;
                        
                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);
                        
                        TeleportTile teleportTile = grid.GetTeleportTile(x, y);
                        Teleport((int)teleportTile.DestinationCoordinates.x, (int)teleportTile.DestinationCoordinates.y);
                        
                        break;

                    case TileTypes.None:
                        if(!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);
                        grid.GetTile(x, y).TileType = TileTypes.Dice;

                        Row = x;
                        Column = y;

                        RollDice(direction);
                        break;
                  
                }
            }
        }
    }

    private void ResetTile(int x, int y)
    {
        if(grid.TileIsBlock(x, y)) return;
        if(grid.TileIsFlag(x, y)) return;
        if(grid.TileIsTeleport(x, y)) return;
        if(grid.TileIsPower(x, y) && GodMode) return;

        grid.GetTile(x, y).TileType = TileTypes.None;
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
