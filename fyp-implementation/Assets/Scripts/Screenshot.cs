using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private static Screenshot instance;
    [SerializeField]
    public RenderTexture renderTexture;
    private void Awake()
    {
        instance = this;
    }

    private void CaptureScreenshot(Camera cam)
    {
        RenderTexture original = cam.targetTexture;
        cam.targetTexture = renderTexture;
        cam.Render();
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;
        byte[] byteArray = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/" + cam.tag + ".png", byteArray);
        Debug.Log("Saved screenshot " + cam.tag + ".png");
        cam.targetTexture = original;
    }

    public static void Screenshot_Static(Camera cam)
    {
        instance.CaptureScreenshot(cam);
    }
}
