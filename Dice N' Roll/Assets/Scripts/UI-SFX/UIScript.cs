using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    private DiceScript dice;

    [SerializeField] private TMPro.TextMeshProUGUI rollsLabel;
    [SerializeField] private TMPro.TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject completedLevelMenu;
    [SerializeField] private GameObject failedLevelMenu;

    [HideInInspector] public bool isPaused;
    private bool canPause = true;

    void Start()
    {
        dice = GameObject.FindGameObjectWithTag("Dice").GetComponent<DiceScript>();
        levelLabel.text = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if(dice == null) dice = GameObject.FindGameObjectWithTag("Dice").GetComponent<DiceScript>();

        rollsLabel.text = ("ROLLS - " + dice.DiceValue);

        KeyCode pause = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause"));
        if(Input.GetKeyDown(pause))
        {
            Pause();
        }
    }

    public void Sound()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        StartCoroutine(RemoveSound(audio));
    }

    public void Menu()
    {
        Sound();
        LoadingScreen.LoadLevel.Invoke("Main Menu");
    }

    public void NextLevel()
    {
        Sound();
        LoadingScreen.LoadLevelIndex.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        Sound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Back()
    {
        Sound();
        isPaused = false;
        pauseMenu.SetActive(false);
        dice.SetGameStatus(true);
    }

    public void Pause()
    {
        if(canPause)
        {
            Sound();
            if (dice.GetGameStatus() && !isPaused)
            {
                isPaused = true;
                pauseMenu.SetActive(true);
                dice.SetGameStatus(false);
            } 
            else 
            {
                Back();
            }
        }
    }

    public void CompleteLevel()
    {
        Back();
        dice.SetGameStatus(false);
        canPause = false;
        completedLevelMenu.SetActive(true);
    }

    public void FailLevel()
    {
        Back();
        dice.SetGameStatus(false);
        canPause = false;
        failedLevelMenu.SetActive(true);
    }

    IEnumerator RemoveSound(AudioSource sound)
    {
        yield return new WaitForSeconds(1);
        DestroyImmediate(sound, true);
    }
}
