using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI hostNameLabel;
    [SerializeField] TMPro.TextMeshProUGUI roomCodeLabel;
    LobbyManager manager;

    void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void UpdateRoomItem(string hostName, string roomCode)
    {
        this.hostNameLabel.text = hostName;
        this.roomCodeLabel.text = roomCode;
    }

    public void JoinRoom()
    {
        manager.JoinRoom(hostNameLabel.text + "-" + roomCodeLabel.text);
    }
}
