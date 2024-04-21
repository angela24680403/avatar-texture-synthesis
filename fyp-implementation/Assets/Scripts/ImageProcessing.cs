using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class includes all the image processing functions that
/// can be potentially used to manipulate the mask for better 
/// SD/ControlNet outputs. Some functions are not used in the 
/// main system.
/// </summary>
public class ImageProcessing : MonoBehaviour
{
    /// <summary>
    /// The ImageProcessing static instance.
    /// </summary>
    private static ImageProcessing instance;

    /// <summary>
    /// Initialise the ImageProcessing static instance.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// The DilateMask static function.
    /// </summary>
    public static void DilateMask_Static(Texture2D mask, int kernalSize)
    {
        instance.DilateMask(mask, kernalSize);

    }

    /// <summary>
    /// Rezize a given texture to new width and height. (Not used)
    /// </summary>
    /// <param name="image">The Texture2D image to be resized.</param>
    /// <param name="newW">The new width.</param>
    /// <param name="newH">The new height.</param>
    /// <returns>A Texture2D image with the new width and height.</returns>
    public static Texture2D Resize(Texture2D image, int newW, int newH)
    {
        RenderTexture rt = new RenderTexture(newW, newH, 24);
        RenderTexture.active = rt;
        Graphics.Blit(image, rt);
        Texture2D result = new Texture2D(newW, newH);
        result.ReadPixels(new Rect(0, 0, newW, newH), 0, 0);
        result.Apply();
        return result;
    }

    /// <summary>
    /// Remove black pixels of an image. (Not used)
    /// </summary>
    /// <param name="image">The Texture2D image to be processed.</param>
    /// <returns>A Texture2D image without black pixels.</returns>
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

    /// <summary>
    /// Dilates the mask image. 
    /// </summary>
    /// <param name="mask">The Texture2D mask to be processed.</param>
    /// <param name="kernelSize">The kernel size.</param>
    /// <returns>The diluted image.</returns>
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
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/Dilated.png", byteArray);
        Debug.Log("Saved Dilated.png");
    }

    /// <summary>
    /// Checks if there is a white neighbouring pixel to the 
    /// centre pixel. If so, returns a boolean showing that the 
    /// main pixel is white, otherwise returns false.
    /// </summary>
    /// <param name="xCoord">The x coordinate of centre pixel.</param>
    /// <param name="yCoord">The y coordinate of centre pixel.</param>
    /// <param name="mask">The Texture2D mask to be processed.</param>
    /// <param name="kernelSize">The kernel size.</param>
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
