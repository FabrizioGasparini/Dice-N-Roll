using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OnlineGameManager : MonoBehaviour
{
    private int readyPlayers;
    
    public static OnlineGameManager Instance;

    public static OnlineDice P1Dice;
    public static OnlineDice P2Dice;

    [SerializeField] GameObject waitingPanel;
    [SerializeField] TMPro.TextMeshProUGUI levelLabel;
    [SerializeField] TMPro.TextMeshProUGUI waitingLabel;

    public static OnlineDice GetOtherDice(OnlineDice dice)
    {
        if(dice == P1Dice) return P2Dice;
        else return P1Dice;
    }

    void Awake()
    {
        Instance = this;
        levelLabel.text = SceneManager.GetActiveScene().name;
        waitingPanel.SetActive(true);
    }

    public void StartGame()
    {
        waitingPanel.SetActive(false);
        GameObject.FindObjectOfType<COOPGrid>().Init();
    }

    public void Ready(GameObject readyButton)
    {
        readyButton.SetActive(false);
        readyPlayers += 1;

        waitingLabel.text = "WAITING FOR PLAYERS " + readyPlayers + "/2";
        UpdateReadyPlayers();

        if(readyPlayers == 2) StartGame();
    }



    private void UpdateReadyPlayers()
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("ReciveReadyPlayers", RpcTarget.Others, readyPlayers);
    }

    [PunRPC]
    public void ReciveReadyPlayers(int readyCount)
    {
        readyPlayers = readyCount;
        waitingLabel.text = "WAITING FOR PLAYERS " + readyPlayers + "/2";
        if (readyPlayers == 2) StartGame();
    }



    public void MoveDice(int diceNumber, int x, int y, Vector3 direction)
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("ReciveMoveDice", RpcTarget.Others, diceNumber, x, y, direction);
    }

    [PunRPC]
    public void ReciveMoveDice(int diceNumber, int x, int y, Vector3 direction)
    {
        if(diceNumber == 1) P1Dice.Move(x, y, direction);
        else P2Dice.Move(x, y, direction);
    }



    public void FailLevel()
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("ReciveFailLevel", RpcTarget.All);
    }

    [PunRPC]
    public void ReciveFailLevel()
    {
        if(P1Dice.inGame) P1Dice.Fail();
        if(P2Dice.inGame) P2Dice.Fail();

        if(!P1Dice.hasFailed && !P2Dice.hasFailed)
        {
            P1Dice.hasFailed = true;
            P2Dice.hasFailed = true;

            GameObject.FindObjectOfType<OnlineLevelUI>().FailLevel();
        }
    }


    public void CompleteLevel()
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("ReciveCompleteLevel", RpcTarget.All);
    }

    [PunRPC]
    public void ReciveCompleteLevel()
    {
        if(P1Dice.inGame || P2Dice.inGame) return;

        if(P1Dice.hasArrived && P2Dice.hasArrived)
        {
            P1Dice.hasArrived = false;
            P2Dice.hasArrived = false;

            GameObject.FindObjectOfType<OnlineLevelUI>().CompleteLevel();
        }
    }



    public void Pause(bool pause)
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("RecivePause", RpcTarget.All, pause);
    }

    [PunRPC]
    public void RecivePause(bool pause)
    {
        if(pause) GameObject.FindObjectOfType<OnlineLevelUI>().Pause();
        else GameObject.FindObjectOfType<OnlineLevelUI>().UnPause();
    }



    public void Retry()
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("ReciveRetry", RpcTarget.All);
    }

    [PunRPC]
    public void ReciveRetry()
    {
        GameObject.FindObjectOfType<OnlineLevelUI>().CloseEndScreen();
        GameObject.FindObjectOfType<OnlineLevelUI>().UnPause();

        GameObject.FindObjectOfType<COOPGrid>().ResetTiles();
        GameObject.FindObjectOfType<COOPGrid>().Init();
    }
}
