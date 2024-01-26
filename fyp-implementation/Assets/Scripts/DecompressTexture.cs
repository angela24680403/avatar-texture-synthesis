using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecompressTexture : MonoBehaviour
{
    private static DecompressTexture instance;
    private void Awake()
    {
        instance = this;
    }

    Texture2D Decompress(Texture2D source)
    {
        // https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity/51317663#51317663
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

    public static Texture2D Decompress_Static(Texture2D source)
    {
        return instance.Decompress(source);
    }
}
