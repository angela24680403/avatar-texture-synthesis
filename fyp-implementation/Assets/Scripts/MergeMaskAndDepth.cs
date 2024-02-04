using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeMaskAndDepth : MonoBehaviour
{
    [SerializeField]
    public Texture2D mask;
    public Texture2D depth;

    void Start()
    {
        Merge();
    }

    void Merge() {
        depth = ImageProcessing.Resize(depth, 512, 512);
        mask = ImageProcessing.Resize(mask, 512, 512);
        Texture2D mergedTexture = new Texture2D(512, 512);
        for (int x=0; x < depth.width; x++)
        {
                for (int y=0; y<depth.height; y++)
                {
                    Color col = mask.GetPixel(x, y);
                    Debug.Log(col.g);
                    if (col == Color.white)
                    {
                        mergedTexture.SetPixel(x, y, depth.GetPixel(x, y));
                    } 
                    else
                    {
                        mergedTexture.SetPixel(x, y, Color.black);
                    }
                }
        }
        mergedTexture.Apply();
        mergedTexture = DecompressTexture.Decompress_Static(mergedTexture);
        byte[] maskByteArray = mergedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/Merged.png", maskByteArray);
        Debug.Log("Saved texture Merged.png");
    }
    
}
