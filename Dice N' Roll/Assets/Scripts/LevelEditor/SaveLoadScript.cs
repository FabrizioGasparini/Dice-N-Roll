using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class SaveLoadScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject levelTemplate;
    [SerializeField] private Transform levelsContent;

    [Header("Varaibles")]
    [SerializeField] private string bannedCharaters;
    private char[] bannedCharactersArray = new char[50];

    public static SaveLoadScript Instance;
    

    private string newLevelName;
    private string saveDirectory = "/LevelEditor/";

    public List<string> generatedLevels = new List<string>();

    private Action<Texture2D> onScreenshotTaken;


    void Start()
    {
        Instance = this;
        RenderPipelineManager.endFrameRendering += RenderPipelineManager_endFrameRendering;

        bannedCharactersArray = bannedCharaters.ToCharArray();

        GenerateLevels();
        OpenPanel(false);
        
        LoadEditorLevel();
    }

    public void ChangeLevelName(string newName)
    {
        newLevelName = newName;
    }
    
    public void Save()
    {
        string dir = Application.persistentDataPath + saveDirectory;

        if (newLevelName == "")
        {
            ErrorsPanelScript.SendError.Invoke("you need to choose a name to save your level");
            return;
        }

        foreach (char character in bannedCharactersArray)
        {            
            if (newLevelName.Contains(character))
            {
                ErrorsPanelScript.SendError.Invoke("level name cannot contain <color=white>" + character);
                return;
            }
        }


        if (File.Exists(dir + newLevelName + ".lvldata"))
        {
            ErrorsPanelScript.SendError.Invoke("a level named <color=white>" + newLevelName + "<color=red> already exists");
            return;
        }
        
        var EditorUI = GameObject.FindGameObjectWithTag("EditorUI").GetComponent<LevelEditorUI>();

        EditorUI.EnableScreenshotMode();
        ConfirmActionUI.Instance.ConfirmAction("Are you sure you want to save '<b><color=lightblue>" + newLevelName + "<color=white>' with this screenshot</b>?", "Cancel", "SAVE", () => EditorUI.DisableScreenshotMode(), () =>
        {
            EditorUI.DisableScreenshotMode();

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            LevelData data = Resources.Load<LevelData>("Levels/CurrentLevelEditor");
            string json = JsonUtility.ToJson(data, false);


            TakeScreenshot(1920, 1080, (Texture2D screenshotTexture) =>
            {
                FileDataWithImage.Save(newLevelName, json, screenshotTexture);
            });

        }, true);

        GenerateLevels();
    }

    public void GenerateLevels()
    {
        var filesPaths = Directory.GetFiles(Application.persistentDataPath + "/LevelEditor");

        List<string> names = new List<string>();

        foreach (string path in filesPaths)
        {
            string name = path.Split("LevelEditor\\")[1];
            if(name == "CurrentEditorLevel.lvldata") break;

            names.Add(name);

            if(!generatedLevels.Contains(name))
            {
                generatedLevels.Add(name);

                var level = Instantiate(levelTemplate, levelsContent);
                level.name = name;

                level.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = name;
                level.GetComponent<LevelTemplateScript>().SetName(name);
            }
        }

        foreach (Transform child in levelsContent)
        {
            bool exists = false;
            foreach (string name in names)
            {
                if(child.gameObject.name == name) exists = true;
            }

            if(!exists) Destroy(child.gameObject);
        }
    }

    private void RenderPipelineManager_endFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
    {
        if (onScreenshotTaken != null)
        {
            RenderTexture renderTexture = Camera.main.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            RenderTexture.ReleaseTemporary(renderTexture);
            Camera.main.targetTexture = null;

            onScreenshotTaken(renderResult);
            onScreenshotTaken = null;
        }
    }

    public void TakeScreenshot(int width, int height, Action<Texture2D> onScreenshotTaken)
    {
        Camera.main.targetTexture = RenderTexture.GetTemporary(width,height, 16);
        this.onScreenshotTaken = onScreenshotTaken;
    }

    public void OpenPanel(bool value)
    {
        transform.GetChild(0).gameObject.SetActive(value);
        transform.GetChild(1).gameObject.SetActive(!value);
    }

    public void LoadEditorLevel()
    {
        if(!File.Exists(Application.persistentDataPath + "/LevelEditor/CurrentEditorLevel.lvldata")) return;
        
        FileDataWithImage.Load("CurrentEditorLevel.lvldata", out LevelData levelData, out Texture2D screenshot);

        Resources.Load<LevelData>("Levels/CurrentEditorLevel").Override(levelData);

        EditorPlacementScript editorPlacementScript = GameObject.FindGameObjectWithTag("Editor").GetComponent<EditorPlacementScript>();
        editorPlacementScript.Init();
    }
}

public class FileDataWithImage
{

    [Serializable]
    public class Header
    {
        public int jsonByteSize;
    }

    public static void Save(string levelName, string json, Texture2D screenshot)
    {
        string dir = Application.persistentDataPath + "/LevelEditor/";

        byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);
        byte[] screenshotByteArray = screenshot.EncodeToPNG();

        Header header = new Header
        {
            jsonByteSize = jsonByteArray.Length
        };
        string headerJson = JsonUtility.ToJson(header, true);
        byte[] headerJsonByteArray = Encoding.Unicode.GetBytes(headerJson);

        ushort headerSize = (ushort)headerJsonByteArray.Length;
        byte[] headerSizeByteArray = BitConverter.GetBytes(headerSize);

        List<byte> byteList = new List<byte>();
        byteList.AddRange(headerSizeByteArray);
        byteList.AddRange(headerJsonByteArray);
        byteList.AddRange(jsonByteArray);
        byteList.AddRange(screenshotByteArray);

        File.WriteAllBytes(dir + levelName + ".lvldata", byteList.ToArray());
    }
    public static void Load(string levelName, out LevelData saveData, out Texture2D screenshotTexture2D)
    {
        saveData = ScriptableObject.CreateInstance<LevelData>();

        string dir = Application.persistentDataPath + "/LevelEditor/";

        byte[] byteArray = File.ReadAllBytes(dir + levelName);
        List<byte> byteList = new List<byte>(byteArray);

        ushort headerSize = BitConverter.ToUInt16(new byte[] { byteArray[0], byteArray[1] }, 0);
        List<byte> headerByteList = byteList.GetRange(2, headerSize);
        string headerJson = Encoding.Unicode.GetString(headerByteList.ToArray());
        Header header = JsonUtility.FromJson<Header>(headerJson);

        List<byte> jsonByteList = byteList.GetRange(2 + headerSize, header.jsonByteSize);
        string gameDataJson = Encoding.Unicode.GetString(jsonByteList.ToArray());
        JsonUtility.FromJsonOverwrite(gameDataJson, saveData);

        int startIndex = 2 + headerSize + header.jsonByteSize;
        int endIndex = byteArray.Length - startIndex;
        List<byte> screenshotByteList = byteList.GetRange(startIndex, endIndex);
        screenshotTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        screenshotTexture2D.LoadImage(screenshotByteList.ToArray());
    }
}