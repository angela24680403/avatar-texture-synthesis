using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessing : MonoBehaviour
{
    [SerializeField]
    public Texture2D mask;
    public int kernelSize;
    public Texture2D image;

    void Start()
    {
        DilateMask(mask, kernelSize);
    }

    void RemoveBg()
    {
        Texture2D newImage = new Texture2D(image.width, image.height);
        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                Color col = image.GetPixel(x, y);
                float threshold = 0.1f;
                if (col.r > threshold && col.g > threshold && col.b > threshold)
                {
                    newImage.SetPixel(x, y, col);
                }
            }
        }
        newImage.Apply();
        Texture2D decompressedTexture = DecompressTexture.Decompress_Static(newImage);
        byte[] byteArray = decompressedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/RemovedBg.png", byteArray);
        Debug.Log("Saved background removed image RemovedBg.png");
    }

    void DilateMask(Texture2D mask, int kernelSize)
    {
        Texture2D dilatedMask = new Texture2D(mask.width, mask.height);
        int padding = kernelSize / 2;

        for (int x = padding; x < mask.width - padding; x++)
        {
            for (int y = padding; y < mask.height - padding; y++)
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
        }
        dilatedMask.Apply();
        Texture2D decompressedTexture = DecompressTexture.Decompress_Static(dilatedMask);
        byte[] byteArray = decompressedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/Dilated.png", byteArray);
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
