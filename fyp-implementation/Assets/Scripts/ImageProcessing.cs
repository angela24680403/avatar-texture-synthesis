using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessing : MonoBehaviour
{
    private static ImageProcessing instance;

    private void Awake()
    {
        instance = this;
    }


    public static void DilateMask_Static(Texture2D mask, int kernalSize)
    {
        instance.DilateMask(mask, kernalSize);

    }

    public static Texture2D Resize(Texture2D texture2D, int new_w, int new_h)
    {
        RenderTexture rt = new RenderTexture(new_w, new_h, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(new_w, new_h);
        result.ReadPixels(new Rect(0, 0, new_w, new_h), 0, 0);
        result.Apply();
        return result;
    }

    void RemoveBlackPixels(Texture2D image)
    {
        Texture2D newImage = new Texture2D(image.width, image.height);
        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                Color col = image.GetPixel(x, y);
                bool is_black = col.g < 0.25f && col.b < 0.25f && col.r < 0.25f;
                if (!is_black)
                {
                    newImage.SetPixel(x, y, col);
                }
            }
        }
        newImage.Apply();
        byte[] byteArray = newImage.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/RemoveBlackPixels.png", byteArray);
        Debug.Log("Saved background removed image RemovedBg.png");
    }

    void DilateMask(Texture2D mask, int kernelSize)
    {
        Texture2D dilatedMask = new Texture2D(mask.width, mask.height);
        int padding = kernelSize / 2;


        for (int x = 0; x < mask.width; x++)
        {
            for (int y = 0; y < mask.height; y++)
            {
                if (x > padding && x < mask.width - padding && y > padding && y < mask.height - padding)
                {
                    if (UpdatePixelValue(x, y, mask, kernelSize))
                    {
                        dilatedMask.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        dilatedMask.SetPixel(x, y, Color.black);
                    }
                }
                else
                {
                    dilatedMask.SetPixel(x, y, Color.black);
                }
            }
        }
        dilatedMask.Apply();
        Texture2D decompressedTexture = DecompressTexture.Decompress_Static(dilatedMask);
        byte[] byteArray = decompressedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/Mask.png", byteArray);
        Debug.Log("Saved Dilated.png");
    }

    bool UpdatePixelValue(int xCoord, int yCoord, Texture2D mask, int kernelSize)
    {
         bool has_white = false;
        for (int i = 0; i < kernelSize; i++)
        {
            for (int j = 0; j < kernelSize; j++)
            {
                int newX = xCoord + i - kernelSize / 2;
                int newY = yCoord + i - kernelSize / 2;
                Color col = mask.GetPixel(newX, newY);
                float threshold = 0.5f;
                if (col.r > threshold && col.g > threshold && col.b > threshold)
                {
                    has_white = true;
                }
            }
        }
        return has_white;
    }
}
