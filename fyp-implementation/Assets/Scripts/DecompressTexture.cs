using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class decompresses a given texture to a saveable format.
/// </summary>
public class DecompressTexture : MonoBehaviour
{
    /// <summary>
    /// The DecompressTexture static instance.
    /// </summary>
    private static DecompressTexture instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Converts a compressed texture into a decompressed Texture2D by blitting the source 
    /// to render texture, then copy the pixels from render texture to a new Texture2D.
    /// </summary>
    /// <param name="source">The compressed Texture2D source that needs to be decompressed.</param>
    /// <returns>A decompressed Texture2D texture.</returns>
    /// <remarks>
    /// Reference: https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity/51317663#51317663
    /// </remarks>
    Texture2D Decompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    /// <summary>
    /// The static function of Decompress.
    /// </summary>
    public static Texture2D Decompress_Static(Texture2D source)
    {
        return instance.Decompress(source);
    }
}
