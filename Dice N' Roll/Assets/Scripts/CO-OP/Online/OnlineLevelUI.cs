using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineLevelUI : MonoBehaviour
{
    private OnlineDice p1dice;
    private OnlineDice p2dice;

    [SerializeField] private TMPro.TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject completedLevelMenu;
    [SerializeField] private GameObject failedLevelMenu;

    [HideInInspector] public bool isPaused;
    private bool canPause = true;

    void Start()
    {
        p1dice = OnlineGameManager.P1Dice;
        p2dice = OnlineGameManager.P1Dice;
        
        levelLabel.text = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        KeyCode pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause"));
        if (Input.GetKeyDown(pause))
        {
            //Pause();
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
    }

    public void NextLevel()
    {
        Sound();
    }

    public void Retry()
    {
        Sound();
    }

    public void Back()
    {
        OnlineGameManager.Instance.Pause(false);
    }

    public void UnPause()
    {
        Sound();
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        if (canPause)
        {
            Sound();
        
            if (!isPaused)
            {
                isPaused = true;
                pauseMenu.SetActive(true);
            }
            else Back();
        }
    }

    public void CompleteLevel()
    {
        canPause = false;
        completedLevelMenu.SetActive(true);
    }

    public void FailLevel()
    {        
        canPause = false;
        failedLevelMenu.SetActive(true);
    }

    public void CloseEndScreen()
    {
        completedLevelMenu.SetActive(false);
        failedLevelMenu.SetActive(false);
    }

    IEnumerator RemoveSound(AudioSource sound)
    {
        yield return new WaitForSeconds(1);
        DestroyImmediate(sound, true);
    }
}
