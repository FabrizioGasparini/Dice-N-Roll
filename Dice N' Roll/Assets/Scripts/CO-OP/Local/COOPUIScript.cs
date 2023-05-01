using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class COOPUIScript : MonoBehaviour
{
    private COOPDiceScript p1CoopDice;
    private COOPDiceScript p2CoopDice;

    [SerializeField] private TMPro.TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject completedLevelMenu;
    [SerializeField] private GameObject failedLevelMenu;

    [HideInInspector] public bool isPaused;
    private bool canPause = true;

    void Start()
    {
        p1CoopDice = GameObject.FindObjectsOfType<COOPDiceScript>()[0];
        p2CoopDice = GameObject.FindObjectsOfType<COOPDiceScript>()[1];

        levelLabel.text = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        KeyCode pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause"));
        if (Input.GetKeyDown(pause))
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

        p1CoopDice.SetGameStatus(true);
        p2CoopDice.SetGameStatus(true);
    }

    public void Pause()
    {
        if (canPause)
        {
            Sound();
            
            if ((p1CoopDice.GetGameStatus() || p2CoopDice.GetGameStatus()) && !isPaused)
            {
                isPaused = true;
                pauseMenu.SetActive(true);

                p1CoopDice.SetGameStatus(false);
                p2CoopDice.SetGameStatus(false);
            }
            else Back();
        }
    }

    public void CompleteLevel()
    {
        Back();

        p1CoopDice.SetGameStatus(false);
        p2CoopDice.SetGameStatus(false);

        canPause = false;
        completedLevelMenu.SetActive(true);
    }

    public void FailLevel()
    {
        Back();

        p1CoopDice.SetGameStatus(false);
        p2CoopDice.SetGameStatus(false);

        canPause = false;
        failedLevelMenu.SetActive(true);
    }

    IEnumerator RemoveSound(AudioSource sound)
    {
        yield return new WaitForSeconds(1);
        DestroyImmediate(sound, true);
    }
}
