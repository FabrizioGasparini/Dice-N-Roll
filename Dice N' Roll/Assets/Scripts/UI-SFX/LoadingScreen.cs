using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TMPro.TextMeshProUGUI levelName;

    public static UnityEvent<string> LoadLevel = new UnityEvent<string>();
    public static UnityEvent<int> LoadLevelIndex = new UnityEvent<int>();

    void Awake()
    {
        loadingScreen.SetActive(false);
        LoadLevel.AddListener(LoadScene);
        LoadLevelIndex.AddListener(LoadSceneIndex);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
        levelName.text = sceneName;
    }
    public void LoadSceneIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneIndexAsync(sceneIndex));
        levelName.text = GetSceneName(sceneIndex);
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.fillAmount = progressValue;

            yield return null;
        }
    }

    IEnumerator LoadSceneIndexAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while(!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.fillAmount = progressValue;

            yield return null;
        }
    }

    private string GetSceneName(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
