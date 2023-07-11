using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] TMPro.TMP_InputField usernameInput;

    public void JoinLobby()
    {
        if (usernameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = usernameInput.text;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("OnlineLobby");
    }
}
