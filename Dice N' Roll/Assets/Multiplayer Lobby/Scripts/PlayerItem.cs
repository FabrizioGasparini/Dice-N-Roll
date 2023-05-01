using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI playerNameLabel;
    [SerializeField] TMPro.TextMeshProUGUI playerNumberLabel;
    
    public void SetPlayerInfo(string playerName, int playerNumber)
    {
        playerNameLabel.text = playerName;
        playerNumberLabel.text = "PLAYER " + playerNumber.ToString();

        if(playerNumber == 2) GetComponent<Image>().color = Color.yellow; 
    }
}
