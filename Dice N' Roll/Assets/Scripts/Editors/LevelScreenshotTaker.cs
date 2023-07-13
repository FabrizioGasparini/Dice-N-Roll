#if (UNITY_EDITOR)
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelScreenshotTaker : EditorWindow
{
    private GUIStyle titleStyle = new GUIStyle();
    private GameObject UIParent = null;

    private string path;
    
    [MenuItem("Dice N' Roll/Level Screenshot Taker")]
    public static void ShowWindow()
    {
        GetWindow<LevelScreenshotTaker>("Level Screenshot Taker");
    }

    private void OnGUI()
    {
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 25;
        titleStyle.normal.textColor = Color.white;

        
        GUILayout.Label("");
        GUILayout.Label(" LEVEL SCREENSHOT TAKER", titleStyle);
        GUILayout.Label("");
        
        GUILayout.Label("", GUILayout.Width(30),GUILayout.Height(5));
        
        if(GUILayout.Button("Take Screenshot", GUILayout.Height(50)))
        {
            path = Application.dataPath + "/UI/MainMenu/Sprites/Levels/";
            TakeScreenshotURP.instance.TakeScreenshot(1920, 1080, (Texture2D screenshot) =>
            {
                byte[] screenshotByteArray = screenshot.EncodeToPNG();
                File.WriteAllBytes(path + SceneManager.GetActiveScene().name + "_NoBG.png", screenshotByteArray);
            });
        }
    }
}
#endif