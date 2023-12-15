using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using UnityEngine.Rendering;

public class Screenshot : MonoBehaviour
{
    // ref: https://discussions.unity.com/t/onpostrender-is-not-called/213533/2
    // ref: https://youtu.be/lT-SRLKUe5k?si=jaoAXB0iDTyxySys
    [SerializeField]
    public Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    private void Awake()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UnityEngine.Debug.Log("Pressed left-click.");
            TakeScreenshot(727, 384);
        }

    }

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    private void OnPostRender()
    {
        UnityEngine.Debug.Log("On post render");
        if (takeScreenshotOnNextFrame)
        {
            UnityEngine.Debug.Log("Next Frame");
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/camera-view/" + myCamera.tag + ".png", byteArray);
            UnityEngine.Debug.Log("Saved " + myCamera.tag + ".png");
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }

    }

    private void TakeScreenshot(int width, int height)
    {
        UnityEngine.Debug.Log("...");
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
        UnityEngine.Debug.Log("..!");

    }

}