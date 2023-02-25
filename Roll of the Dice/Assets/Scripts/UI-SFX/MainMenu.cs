using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("General")]
    private GameObject menusPanel;

    private GameObject openMenusButton;
    private GameObject closeMenusButton;

    private GameObject mainMenu;
    private GameObject settingsMenu;
    private GameObject levelMenu;
    private GameObject controlsMenu;

    private GameObject volumeSlider;
    private GameObject volumeValue;

    private float gameVolume = 0.85f;

    void Start() 
    {
        menusPanel = transform.Find("Menus").gameObject;
        openMenusButton = menusPanel.transform.Find("OpenMain").gameObject;

        mainMenu = menusPanel.transform.Find("Main").gameObject;
        closeMenusButton = mainMenu.transform.Find("CloseMenu").gameObject;

        settingsMenu = menusPanel.transform.Find("SettingsMenu").gameObject;
        levelMenu = menusPanel.transform.Find("LevelSelectorMenu").gameObject;
        controlsMenu = menusPanel.transform.Find("ControlsMenu").gameObject;

        volumeSlider = settingsMenu.transform.Find("Volume").Find("VolumeSlider").gameObject;
        volumeValue = settingsMenu.transform.Find("Volume").Find("VolumeValue").gameObject;

        LoadVolume();

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        levelMenu.SetActive(false);
        controlsMenu.SetActive(false);
        
        OpenMenus(true);
    }

    public void Open(string menu){
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        DestroyImmediate(audio, true);

        if (menu == "Main")
        {
            ResetButtons();
            mainMenu.SetActive(true);
        } 
        else if(menu == "Settings") 
        {
            ResetButtons();
            settingsMenu.SetActive(true);
        } 
        else if(menu == "Levels")
        {
            ResetButtons();
            levelMenu.SetActive(true);
        }  
        else 
        {
            ResetButtons();
            controlsMenu.SetActive(true);
        }
    }

    public void Quit(){
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        DestroyImmediate(audio, true);

        Application.Quit();
    }

    public void PlayLevel(string levelName){
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        DestroyImmediate(audio, true);

        LoadingScreen.LoadLevel.Invoke(levelName);
    }

    public void LoadVolume(){
        gameVolume = PlayerPrefs.GetFloat("Volume");
        volumeValue.GetComponent<TMPro.TextMeshProUGUI>().text = (gameVolume * 100).ToString("0.0");
        volumeSlider.GetComponent<Slider>().value = gameVolume;
        AudioListener.volume = gameVolume;
    }

    public void SetVolume(float volume)
    {
        volumeValue.GetComponent<TMPro.TextMeshProUGUI>().text = (volume * 100).ToString("0.0");
        gameVolume = volume;
    }

    public void ApplyVolume()
    {
        PlayerPrefs.SetFloat("Volume",  gameVolume);
        AudioListener.volume = gameVolume;
    }

    private void ResetButtons()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        levelMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void OpenMenus(bool value)
    {
        if(value)
        {
            if(!Camera.main.GetComponent<MenuCameraScript>().canMove) return;

            menusPanel.GetComponent<Animator>().Play("OpenMenusPanel");
            closeMenusButton.SetActive(true);
            openMenusButton.SetActive(false);
        } 
        else
        {
            menusPanel.GetComponent<Animator>().Play("CloseMenusPanel");
            closeMenusButton.SetActive(false);
            openMenusButton.SetActive(true);
        }
            
    }

    IEnumerator RemoveSound(AudioSource sound)
    {
        yield return new WaitForSeconds(1);
        DestroyImmediate(sound, true);
    }
}
