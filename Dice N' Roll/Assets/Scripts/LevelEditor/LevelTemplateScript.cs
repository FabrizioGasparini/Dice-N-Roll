using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LevelTemplateScript : MonoBehaviour
{
    public string LevelName;
    private SaveLoadScript saveLoadScript;

    private string saveDirectory = "/LevelEditor/";
    private string path;

    private LevelData levelEditorData;
    private LevelData tempLoad;
    private LevelData tempOverride;


    void Awake()
    {
        saveLoadScript = SaveLoadScript.Instance;
        path = Application.persistentDataPath + saveDirectory;

        levelEditorData = Resources.Load<LevelData>("Levels/CurrentEditorLevel");
    }


    public void OverrideLevel()
    {
        if (LevelName != "CurrentEditorLevel.lvldata")
        {
            FileDataWithImage.Load(LevelName, out LevelData saveData, out Texture2D screenshot);
            ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to ovverride '<b><color=lightblue>" + LevelName.Split(".")[0] + "</b><color=white>' with the current level?", "Cancel", "Override", () => { }, () =>
            {
                var EditorUI = GameObject.FindObjectOfType<LevelEditorUI>();

                EditorUI.EnableScreenshotMode();
                ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to save '<b><color=lightblue>" + LevelName.Split(".")[0] + "<color=white>' with this screenshot</b>?", "Cancel", "SAVE", () => EditorUI.DisableScreenshotMode(), () =>
                {
                    EditorUI.DisableScreenshotMode();

                    string json = JsonUtility.ToJson(levelEditorData, true);
                
                    saveLoadScript.TakeScreenshot(1920, 1080, (Texture2D screenshotTexture) =>
                    {
                        FileDataWithImage.Save(LevelName.Split(".")[0], json, screenshotTexture);
                    });
                });
            }, false, screenshot);
        }
        else
        {
            FileDataWithImage.LoadCurrentEditor(out LevelData saveData);
            ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to ovverride '<b><color=lightblue>LAST SAVE</b><color=white>' with the current level?", "Cancel", "Override", () => { }, () =>
            {
                string json = JsonUtility.ToJson(levelEditorData, true);
                FileDataWithImage.SaveCurrentEditor(json);
            });
        }
    }

    public void LoadLevel()
    {
        if (LevelName != "CurrentEditorLevel.lvldata")
        {
            FileDataWithImage.Load(LevelName, out LevelData levelData, out Texture2D screenshot);
            ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to load '<b><color=lightblue>" + LevelName.Split(".")[0] + "</b><color=white>', overriding the current level?", "Cancel", "Load", () => { }, () =>
            {
                levelEditorData.Override(levelData);
                
                EditorPlacementScript editorPlacementScript = GameObject.FindGameObjectWithTag("Editor").GetComponent<EditorPlacementScript>();
                editorPlacementScript.ResetTiles();
                editorPlacementScript.Init();
            }, false, screenshot);
        }
        else
        {
            FileDataWithImage.LoadCurrentEditor(out LevelData levelData);
            ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to load '<b><color=lightblue>LAST SAVE</b><color=white>', overriding the current level?", "Cancel", "Load", () => { }, () =>
            {
                levelEditorData.Override(levelData);

                EditorPlacementScript editorPlacementScript = GameObject.FindGameObjectWithTag("Editor").GetComponent<EditorPlacementScript>();
                editorPlacementScript.ResetTiles();
                editorPlacementScript.Init();
            });
        }
    }

    public void DeleteLevel()
    {
        ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to delete '<b><color=lightblue>" + LevelName.Split(".")[0] + "<color=white>' FOREVER</b>?", "Cancel", "Delete", () => { }, () =>
        {
            string path = Application.persistentDataPath + saveDirectory + LevelName;
            File.Delete(path);
            saveLoadScript.GenerateLevels();
        });
    }

    public void SetName(string name)
    {
        LevelName = name;
    }
}
