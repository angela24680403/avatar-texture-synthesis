using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class for the screenshot process.
/// </summary>
public class Screenshot : MonoBehaviour
{
    /// <summary>
    /// The screenshot static instance.
    /// </summary>
    private static Screenshot instance;

    /// <summary>
    /// The render texture.
    /// </summary>
    public RenderTexture renderTexture;

    /// <summary>
    /// The static Screenshot function.
    /// </summary>
    public static void Screenshot_Static(Camera cam)
    {
        instance.CaptureScreenshot(cam);
    }

    /// <summary>
    /// Initializing the screenshot static instance.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Captures the screenshot from a given camera viewpoint by setting
    /// the camera target texture to the render texture temporarily,
    /// the copying from the render texture to a Texture2D texture which
    /// will be saved in the Screenshot folder.
    /// </summary>
    /// <param name="cam">The camera whose viewpoint will be captured.</param>
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
}
