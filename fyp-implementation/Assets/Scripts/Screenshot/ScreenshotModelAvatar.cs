using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://unitycodemonkey.com/video.php?v=lT-SRLKUe5k

public class ScreenshotModelAvatar : MonoBehaviour
{
    private static ScreenshotModelAvatar instance;
    private int captureCount = 0;
    private bool takeScreenshotOnNextFrame;
    private Camera myCamera;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
        instance = this;
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;
            myCamera.Render();
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);
            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Captured-Screenshots/Main/screenshot" + captureCount.ToString() + ".png", byteArray);
            Debug.Log("Saved screenshot" + captureCount.ToString() + ".png");
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
            captureCount++;
        }

    }

    private void CaptureScreenshot()
    {
        Debug.Log("C pressed.");
        myCamera.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        takeScreenshotOnNextFrame = true;
    }

    public static void CaptureScreenshot_Static()
    {
        instance.CaptureScreenshot();
    }
}
