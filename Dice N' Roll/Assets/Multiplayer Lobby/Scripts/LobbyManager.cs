using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room References")]
    [SerializeField] TMPro.TMP_InputField roomCodeInput;

    [Header("")]
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;
    [SerializeField] GameObject levelSelectorPanel;

    [Header("")]
    [SerializeField] TMPro.TextMeshProUGUI roomCodeLabel;

    [Header("")]
    [SerializeField] GameObject createRoomsButton;
    [SerializeField] GameObject refreshRoomsButton;
    [SerializeField] GameObject playButton;


    [Header("Room Item References")]
    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] Transform roomItemParent;
    private List<RoomItem> roomItemsList = new List<RoomItem>();


    [Header("Player Item References")]
    [SerializeField] PlayerItem playerItemPrefab;
    [SerializeField] Transform playerItemParents;
    private List<PlayerItem> playerItemsList = new List<PlayerItem>();

    [Header("Settings")]
    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;


    // Player Connected
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // Create Room
    public void CreateRoom()
    {
        if (roomCodeInput.text.Length >= 1)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName + "-" + roomCodeInput.text, roomOptions);
        }
    }

    // Join Room
    public void JoinRoom(string roomCode)
    {
        PhotonNetwork.JoinRoom(roomCode);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.Name == "Refresh")
        {
            LeaveRoom();
            return;
        }

        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomCodeLabel.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name.Split("-")[1];

        UpdatePlayerList();
    }

    // Update Room List
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            if (!room.Name.Contains("Refresh"))
            {
                RoomItem newRoom = Instantiate(roomItemPrefab, roomItemParent);
                newRoom.UpdateRoomItem(room.Name.Split("-")[0], room.Name.Split("-")[1]);
                roomItemsList.Add(newRoom);
            }
        }
    }

    // Leave Room
    public void LeaveRoom()
    {
        roomCodeInput.text = "";
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        refreshRoomsButton.SetActive(true);
        createRoomsButton.SetActive(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }


    // Player Enter/Leave Room 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.NickName != "") UpdatePlayerList();
        else PhotonNetwork.LeaveRoom();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom != null)
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerItem newPlayer = Instantiate(playerItemPrefab, playerItemParents);

                if(player.Value.IsMasterClient) 
                {
                    newPlayer.gameObject.name = "Player 1";
                    newPlayer.SetPlayerInfo(player.Value.NickName, 1);

                    newPlayer.transform.SetAsFirstSibling();
                }
                else
                {
                    newPlayer.gameObject.name = "Player 2";
                    newPlayer.SetPlayerInfo(player.Value.NickName, 2);
                }

                playerItemsList.Add(newPlayer);
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1) playButton.SetActive(true);
        else
        {
            playButton.SetActive(false);
            levelSelectorPanel.SetActive(false);
        } 
    }

    public void OpenLevelSelector(bool value)
    {
        levelSelectorPanel.SetActive(value);
        roomPanel.SetActive(!value);
    }

    public void PlayLevel(string levelName)
    {
        PhotonNetwork.LoadLevel(levelName);
    }

    public void RefreshRooms()
    {
        createRoomsButton.SetActive(false);
        refreshRoomsButton.SetActive(false);
        PhotonNetwork.CreateRoom("Refresh");
    }

    public void Exit()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
