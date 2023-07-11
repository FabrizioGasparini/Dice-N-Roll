using UnityEngine;

public class LevelEditorUI : MonoBehaviour
{
    public GameObject deleteTileButton;

    void Awake()
    {
        DisableScreenshotMode();
    }

    public void EnableScreenshotMode()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        transform.Find("ConfirmPanel").gameObject.SetActive(true);
        transform.Find("ScreenshotBorders").gameObject.SetActive(true);
    }

    public void DisableScreenshotMode()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        transform.Find("ScreenshotBorders").gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to return to '<b><color=lightblue>MENU<color=white>'</b>?", "Cancel", "MENU", () => {}, () =>
        {
            string json = JsonUtility.ToJson(Resources.Load<LevelData>("Levels/CurrentEditorLevel"));

            FileDataWithImage.SaveCurrentEditor(json);

            GameObject.FindGameObjectWithTag("EditorUI").SetActive(false);
            LoadingScreen.LoadLevel.Invoke("Main Menu");
        });
    }
}
