using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI playerNameLabel;
    [SerializeField] Image playerCharacterImage;
    [SerializeField] TMPro.TextMeshProUGUI playerColorLabel;

    private Color32 playerColor;
    
    public void SetPlayerInfo(string playerName, string playerColor, Color32 color)
    {
        playerNameLabel.text = playerName;
        playerColorLabel.text = playerColor;

        this.playerColor = color;
        GetComponent<Image>().color = this.playerColor; 
    }

    public void ChangeCharacter(bool changeRight)
    {
        if(changeRight)
        {
            // CHANGE CHARACTER ID += 1
        }
        else
        {
            // CHANGE CHARACTER ID -= 1
        }
    }
}
