using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFrameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image levelImage;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private Image levelCompleted;
    
    [Header("Sprites")]
    [SerializeField] Sprite checkboxFull;
    [SerializeField] Sprite checkboxEmpty;

    public void UpdateLevelFrame(Sprite sprite, string name, bool completed)
    {
        levelImage.sprite = sprite;
        levelName.text = name;

        if (completed) levelCompleted.sprite = checkboxFull;
        else levelCompleted.sprite = checkboxEmpty;
    }

    public void OpenLevel()
    {
        SceneManager.LoadScene(levelName.text);
    }
}
