using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OnlineDice : MonoBehaviour
{
    [Header("Grid Variables")]
    public int Row;
    public int Column;
    public int DiceValue;
    [Header("")]
    public int Player;
    [Header("")]
    public bool GodMode;

    [Header("")]
    private bool isMoving = false;
    [HideInInspector] public bool inGame = true;
    [HideInInspector] public bool hasArrived = false;
    [HideInInspector] public bool hasFailed = false;

    [Header("References")]
    [SerializeField] private List<Sprite> diceSprites = new List<Sprite>();

    [Header("Materials")]
    [SerializeField] Material winMaterial;
    [SerializeField] Material loseMaterial;
    [SerializeField] Material p2Material;

    private COOPGrid grid;
    private Transform faces;

    public void RollDice(Vector3 dir)
    {
        if(isMoving) return;

        StartCoroutine(Roll(dir));
    }

    IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;

        float remainingAngle = 90;
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while(remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * 250, remainingAngle);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);

            remainingAngle -= rotationAngle;
            yield return null;
        }

        isMoving = false;

        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/DiceSound"));
        
        yield return new WaitForSeconds(1);
        DestroyImmediate(audio, true);
    }

    void Start()
    {
        //SendPlayer();

        faces = transform.GetChild(0);
        grid = GameObject.FindObjectOfType<COOPGrid>();

        if ((PhotonNetwork.LocalPlayer.IsMasterClient && Player != 1) || (!PhotonNetwork.LocalPlayer.IsMasterClient && Player == 2)) GetComponentInChildren<MeshRenderer>().material = p2Material;
    }

    void Update()
    {
        ChangeNumber();

        if (DiceValue > 0)
        {
            if (inGame)
            {
                if ((PhotonNetwork.LocalPlayer.IsMasterClient && Player == 1) || (!PhotonNetwork.LocalPlayer.IsMasterClient && Player == 2))
                {
                    KeyCode up = KeyCode.W;
                    KeyCode down = KeyCode.S;
                    KeyCode left = KeyCode.A;
                    KeyCode right = KeyCode.D;

                    if (PlayerPrefs.GetString("Up") != null && PlayerPrefs.GetString("Down") != null && PlayerPrefs.GetString("Left") != null && PlayerPrefs.GetString("Right") != null)
                    {
                        up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
                        down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
                        left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
                        right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
                    }

                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(left)) Move(Row, Column - 1, Vector3.back);

                    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(right)) Move(Row, Column + 1, Vector3.forward);

                    else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(up)) Move(Row - 1, Column, Vector3.left);

                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(down)) Move(Row + 1, Column, Vector3.right);
                }

                if (Player == 1)
                {
                    if (Row == grid.LevelData.P1FlagCoordinates.x && Column == grid.LevelData.P1FlagCoordinates.y)
                    {
                        CompleteLevel();
                    }
                }
                else
                {
                    if (Row == grid.LevelData.P2FlagCoordinates.x && Column == grid.LevelData.P2FlagCoordinates.y)
                    {
                        CompleteLevel();
                    }
                }
            }
        }
        else
        {
            if (inGame)
            {
                if (Player == 1)
                {
                    if (Row == grid.LevelData.P1FlagCoordinates.x && Column == grid.LevelData.P1FlagCoordinates.y)
                    {
                        CompleteLevel();
                    }
                    else
                    {
                        Fail();
                        OnlineGameManager.Instance.FailLevel();
                    }
                }
                else
                {
                    if (Row == grid.LevelData.P2FlagCoordinates.x && Column == grid.LevelData.P2FlagCoordinates.y)
                    {
                        CompleteLevel();
                    }
                    else
                    {
                        Fail();
                        OnlineGameManager.Instance.FailLevel();
                    }
                }
            }
        }
    }

    public void Move(int x, int y, Vector3 direction)
    {
        bool hasMoved = false;

        if(OnlineGameManager.GetOtherDice(this).Row == x && OnlineGameManager.GetOtherDice(this).Column == y) return;

        if (!isMoving)
        {
            if (grid.GetTile(x, y))
            {
                TileType tileType = grid.GetTile(x, y).TileType;

                switch (tileType)
                {
                    case TileType.Dice:
                        if (GodMode)
                        {
                            ResetTile(Row, Column);

                            Row = x;
                            Column = y;
                            hasMoved = true;

                            RollDice(direction);
                        }

                        break;

                    case TileType.Block:
                        if (GodMode)
                        {
                            ResetTile(Row, Column);

                            Row = x;
                            Column = y;
                            hasMoved = true;

                            RollDice(direction);
                        }

                        break;

                    case TileType.Power:
                        PowerTile powerTile = grid.GetPowerTile(x, y);

                        if (powerTile != null)
                        {
                            if (!GodMode)
                            {
                                grid.GetTile(x, y).TileType = TileType.Dice;
                                DiceValue -= 1;
                            }

                            ResetTile(Row, Column);

                            Row = x;
                            Column = y;

                            hasMoved = true;
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

                            if (!GodMode) Destroy(powerTile.Object);
                        }
                        
                        break;

                    case TileType.Flag:
                        if (!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        RollDice(direction);

                        hasMoved = true;
                        break;

                    case TileType.Teleport:
                        if (!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;

                        hasMoved = true;
                        RollDice(direction);

                        TeleportTile teleportTile = grid.GetTeleportTile(x, y);

                        if(teleportTile.DestinationCoordinates != new Vector2(OnlineGameManager.GetOtherDice(this).Row, OnlineGameManager.GetOtherDice(this).Column)) Teleport((int)teleportTile.DestinationCoordinates.x, (int)teleportTile.DestinationCoordinates.y);


                        break;

                    case TileType.None:
                        if (!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);
                        grid.GetTile(x, y).TileType = TileType.Dice;

                        Row = x;
                        Column = y;
                        hasMoved = true;

                        RollDice(direction);

                        break;

                    case TileType.GhostBlock:
                        if (GodMode)
                        {
                            ResetTile(Row, Column);

                            Row = x;
                            Column = y;
                            hasMoved = true;                            
                            
                            RollDice(direction);
                        }

                        break;

                    case TileType.Button:
                        if (!GodMode) DiceValue -= 1;

                        ResetTile(Row, Column);

                        Row = x;
                        Column = y;
                        hasMoved = true;

                        RollDice(direction);

                        ButtonTile buttonTile = grid.GetButtonTile(x, y);
                        if (buttonTile.ButtonType == ButtonType.Press)
                        {
                            GhostBlockTile ghostBlockTile = grid.GetGhostBlockTile((int)buttonTile.DestinationCoordinates.x, (int)buttonTile.DestinationCoordinates.y);
                            Destroy(buttonTile.Object);

                            Destroy(ghostBlockTile.Object);

                            grid.GetTile((int)buttonTile.DestinationCoordinates.x, (int)buttonTile.DestinationCoordinates.y).TileType = TileType.None;
                            grid.GetTile(x, y).TileType = TileType.None;
                        }
                        
                        break;
                }
            }
        }

        if(hasMoved) OnlineGameManager.Instance.MoveDice(Player, x, y, direction);
    }

    private void ResetTile(int x, int y)
    {
        if (grid.GetType(x, y) == TileType.Block) return;
        if (grid.GetType(x, y) == TileType.Flag) return;
        if (grid.GetType(x, y) == TileType.Teleport) return;
        if (grid.GetType(x, y) == TileType.GhostBlock) return;
        if (grid.GetType(x, y) == TileType.Button) return;
        if (grid.GetType(x, y) == TileType.Power && GodMode) return;

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
        switch (DiceValue)
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

            if (sprite == null) face.GetComponent<Image>().color = Color.clear;
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

    public void CompleteLevel()
    {
        UpdateMaterial(winMaterial);

        hasArrived = true;
        inGame = false;
        DiceValue = 0;

        OnlineGameManager.Instance.CompleteLevel();
    }

    public void Fail()
    {
        UpdateMaterial(loseMaterial);
        OnlineGameManager.GetOtherDice(this).UpdateMaterial(loseMaterial);

        inGame = false;
        DiceValue = 0;
    }


    private void SendMove(int x, int y, Vector3 direction)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReciveMove", RpcTarget.Others, x, y, direction);
    }

    [PunRPC]
    public void ReciveMove(int x, int y, Vector3 direction)
    {
        Move(x, y, direction);
    }


    public void UpdateMaterial(Material material)
    {
        GetComponentInChildren<MeshRenderer>().material = material;
    }
}
