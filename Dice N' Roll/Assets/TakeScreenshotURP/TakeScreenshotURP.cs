using System.Collections;
using UnityEngine;

public class TakeScreenshotURP : MonoBehaviour {


    public static TakeScreenshotURP instance;
    private string path = "/Resources/Levels/Textures/";

    void Awake()
    {
        instance = this;
    }

    public IEnumerator CoroutineScreenshot(string name) {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshotTexture.ReadPixels(rect, 0, 0);
        screenshotTexture.Apply();

        byte[] byteArray = screenshotTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + path + name + ".png", byteArray);
    }

}