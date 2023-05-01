using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private List<MenuPanel> menuPanels = new List<MenuPanel>();

    [Header("References")]
    [SerializeField] GameObject openMenusButton;
    [SerializeField] GameObject closeMenusButton;


    [Header("Volume References")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMPro.TextMeshProUGUI volumeValueLabel;

    private float gameVolume = 0.85f;

    void Start() 
    {
        LoadVolume();

        Open("MainMenu");
    }

    public void Open(string menu)
    {
        PlayAudio();

        ResetMenus();

        foreach (MenuPanel panel in menuPanels) if(panel.PanelName == menu) panel.Panel.SetActive(true);
    }

    public void Quit()
    {
        PlayAudio();

        Application.Quit();
    }

    public void PlayLevel(string levelName)
    {
        PlayAudio();

        LoadingScreen.LoadLevel.Invoke(levelName);
    }

    public void LoadVolume()
    {
        gameVolume = PlayerPrefs.GetFloat("Volume");

        volumeValueLabel.text = (gameVolume * 100).ToString("0.0");
        volumeSlider.value = gameVolume;
        
        AudioListener.volume = gameVolume;
    }

    public void SetVolume(float volume)
    {
        volumeValueLabel.GetComponent<TMPro.TextMeshProUGUI>().text = (volume * 100).ToString("0.0");
        gameVolume = volume;
    }

    public void ApplyVolume()
    {
        PlayerPrefs.SetFloat("Volume",  gameVolume);
        AudioListener.volume = gameVolume;
    }

    private void PlayAudio()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        DestroyImmediate(audio, true);
    }

    private void ResetMenus()
    {
        foreach (MenuPanel panel in menuPanels)
        {
            panel.Panel.SetActive(false);
        }
    }

    IEnumerator RemoveSound(AudioSource sound)
    {
        yield return new WaitForSeconds(1);
        DestroyImmediate(sound, true);
    }
}

[System.Serializable]
public class MenuPanel
{
    public string PanelName;
    public GameObject Panel;
}