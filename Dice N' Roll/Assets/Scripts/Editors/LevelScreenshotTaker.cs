#if (UNITY_EDITOR)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelScreenshotTaker : EditorWindow
{
    private GUIStyle titleStyle = new GUIStyle();
    private GameObject UIParent = null;

    [MenuItem("Dice N' Roll/Leve Screenshot Taker")]
    public static void ShowWindow()
    {
        GetWindow<LevelScreenshotTaker>("Level Screenshot Taker");
    }

    private void OnGUI()
    {
        if(UIParent == null) UIParent = GameObject.Find("UI");

        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 25;
        titleStyle.normal.textColor = Color.white;

        
        GUILayout.Label("");
        GUILayout.Label(" LEVEL SCREENSHOT TAKER", titleStyle);
        GUILayout.Label("");
        
        GUILayout.Label("", GUILayout.Width(30),GUILayout.Height(5));
        
        if(GUILayout.Button("Take Screenshot", GUILayout.Height(50)))
        {
            GameObject.Find("Screenshot").GetComponent<TakeScreenshotURP>().StartCoroutine(TakeScreenshotURP.instance.CoroutineScreenshot(SceneManager.GetActiveScene().name));
        }
        if(GUILayout.Button("Hide / Show UI", GUILayout.Height(50)))
        {
            UIParent.SetActive(!UIParent.transform.GetChild(0).gameObject.activeInHierarchy);
        }
    }
}
#endif