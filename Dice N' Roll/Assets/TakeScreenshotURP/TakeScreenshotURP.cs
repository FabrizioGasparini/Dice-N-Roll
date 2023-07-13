using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TakeScreenshotURP : MonoBehaviour 
{
    public static TakeScreenshotURP instance;
    private Action<Texture2D> onScreenshotTaken;
    private string path = "/Resources/Levels/Textures/";

    void Awake()
    {
        instance = this;
        RenderPipelineManager.endFrameRendering += RenderPipelineManager_endFrameRendering;
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
}