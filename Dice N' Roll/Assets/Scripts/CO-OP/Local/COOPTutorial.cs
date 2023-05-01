using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COOPTutorial : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI whiteDiceTutorialLabel;

    private string userW;
    private string userA;
    private string userS;
    private string userD;

    void Start()
    {
        userW = PlayerPrefs.GetString("Up");
        userA = PlayerPrefs.GetString("Down");
        userS = PlayerPrefs.GetString("Left");
        userD = PlayerPrefs.GetString("Right");

        whiteDiceTutorialLabel.text="● <color=green> THE <color=white> WHITE DICE <color=green> MOVES WITH<b> <color=white> USER SELECTED KEYS</b> <color=#DDD>(" + userW + userA + userS + userD + ")";
    }
}
