using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script combines all avatar painting functions such as taking
/// screenshots, rotating the avatars, dilating masks, and projecting
/// an image onto the main avatar texture.
/// </summary>
public class AvatarPainter : MonoBehaviour
{
    /// <summary>
    /// The size of the kernel dilating the mask.
    /// </summary>
    public int KERNELSIZE = 7;

    public GameObject mainAvatar;
    public GameObject modelAvatar;
    public GameObject maskAvatar;

    /// <summary>
    /// The main camera.
    /// </summary>
    public Camera mainCam;

    /// <summary>
    /// The model camera.
    /// </summary>
    public Camera modelCam;

    /// <summary>
    /// The mask camera.
    /// </summary>
    public Camera maskCam;

    /// <summary>
    /// The depth camera.
    /// </summary>
    public Camera depthCam;

    /// <summary>
    /// The model screenshot.
    /// </summary>
    public Texture2D modelScreenshot;

    /// <summary>
    /// The mask screenshot.
    /// </summary>
    public Texture2D maskScreenshot;

    /// <summary>
    /// The inpainted image.
    /// </summary>
    public Texture2D inpaintedImage;

    /// <summary>
    /// The texture attached to the mask avatar.
    /// </summary>
    public Texture2D maskTexture;

    /// <summary>
    /// The width and height of camera viewpoints.
    /// </summary>
    private int VIEWPOINT_SIZE = 512;

    /// <summary>
    /// Counts the number of times the texture has been saved.
    /// </summary>
    private int count = 0;

    /// <summary>
    /// Dilates a mask screenshot given a kernel size.
    /// </summary>
    public void DilateMask()
    {
        ImageProcessing.DilateMask_Static(maskScreenshot, KERNELSIZE);
    }

    /// <summary>
    /// Project an image from the main camera viewpoint onto the main avatar
    /// without projecting black from the mask camera viewpoint onto the mask avatar.
    /// </summary>
    public void Project()
    {
        PaintFromPose(inpaintedImage, false);
    }

    /// <summary>
    /// Project an image from the main camera viewpoint onto the main avatar
    /// and projects black from the mask camera viewpoint onto the mask avatar.
    /// </summary>
    public void ProjectWithMaskFill()
    {
        PaintFromPose(inpaintedImage, true);
    }


    /// <summary>
    /// Simultaneously screenshots from all four camera viewpoints.
    /// </summary>
    public void ScreenshotAll()
    {
        Debug.Log("Taking screenshot...");
        Screenshot.Screenshot_Static(mainCam);
        Screenshot.Screenshot_Static(maskCam);
        Screenshot.Screenshot_Static(modelCam);
        Screenshot.Screenshot_Static(depthCam);
    }

    /// <summary>
    /// Simultaneously rotates all avatars by 45 degrees.
    /// </summary>
    public void RotateAll()
    {
        Rotate(modelAvatar, 45f);
        Rotate(maskAvatar, 45f);
        Rotate(mainAvatar, 45f);
    }

    /// <summary>
    /// Rotates the given avatar by a specified angle around the Y-axis.
    /// </summary>
    /// <param name="avatar">The game object to be rotated.</param>
    /// <param name="angle">The angle in degrees the game object should be rotated around the Y-axis.</param>
    void Rotate(GameObject avatar, float angle)
    {
        float targetAngle = avatar.transform.rotation.eulerAngles.y + angle;
        avatar.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    /// <summary>
    /// Saves the edited texture of main avatar as a decompressed png.
    /// The main avatar texture is obtained from the main material 
    /// hit by a ray sent from the centre of the screen.
    /// </summary
    public void SavePaintedTexture()
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(256.0f, 256.0f, 0.0f);
        if (Physics.Raycast(mainCam.ScreenPointToRay(pos), out hit))
        {
            SkinnedMeshRenderer rend = hit.transform.GetComponent<SkinnedMeshRenderer>();
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Texture2D readable = DecompressTexture.Decompress_Static(tex);
            byte[] byteArray = readable.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/Skin" + count.ToString() + ".png", byteArray);
            Debug.Log("Saved texture Skin.png");
        }
        Texture2D readableMask = DecompressTexture.Decompress_Static(maskTexture);
        byte[] maskByteArray = readableMask.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/Mask" + count.ToString() + ".png", maskByteArray);
        Debug.Log("Saved texture Mask.png");
        count++;
    }


    /// <summary>
    /// Given xy coordinates of a pixel of a texture, sets the colour of diagonal and adjacent pixels.
    /// </summary>
    /// <param name="tex">The texture on which the color will be applied.</param>
    /// <param name="x">The x-coordinate of the centre pixel where the color application begins.</param>
    /// <param name="y">The y-coordinate of the centre pixel where the color application begins.</param>
    /// <param name="col">The color to apply to the specified group of pixels.</param>
    void SetGroupPixels(Texture2D tex, int x, int y, Color col)
    {
        tex.SetPixel(x, y, col);
        tex.SetPixel(x + 1, y, col);
        tex.SetPixel(x - 1, y, col);
        tex.SetPixel(x, y + 1, col);
        tex.SetPixel(x, y - 1, col);
        tex.SetPixel(x + 1, y + 1, col);
        tex.SetPixel(x + 1, y - 1, col);
        tex.SetPixel(x - 1, y - 1, col);
        tex.SetPixel(x - 1, y - 1, col);
        tex.Apply();
    }

    /// <summary>
    /// Saves the edited texture of main avatar as a decompressed png.
    /// </summary
    void Paint(RaycastHit hit, Color col, bool is_mask)
    {
        SkinnedMeshRenderer rend = hit.transform.GetComponent<SkinnedMeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        Texture2D tex = maskTexture;
        if (!(rend == null || meshCollider == null))
        {
            if (is_mask != true)
            {
                tex = rend.material.mainTexture as Texture2D;
            }
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            SetGroupPixels(tex, (int)pixelUV.x, (int)pixelUV.y, col);
        }

    }

    /// <summary>
    /// Checks if the color is not black.
    /// </summary>
    /// <param name="col">The color to check.</param>
    /// <returns>
    /// Returns true if any of the color's RGB components are greater 
    /// than 0. Otherwise return false.
    /// </returns>
    bool NotBlack(Color col)
    {
        return col.r > 0 || col.g > 0 || col.b > 0;
    }

    /// <summary>
    /// Checks if a point and its neighbors in a texture are not black.
    /// </summary>
    /// <param name="x">The x coordinate of the point.</param>
    /// <param name="y">The y coordinate of the point.</param>
    /// <returns>
    /// Returns true if all specified points are not black. 
    /// Otherwise return false. 
    /// </returns>
    bool PtValid(int x, int y)
    {
        Color mask_col = modelScreenshot.GetPixel(x, y);
        if (NotBlack(mask_col) &&
            NotBlack(modelScreenshot.GetPixel(x, y + 1)) &&
            NotBlack(modelScreenshot.GetPixel(x, y - 1)) &&
            NotBlack(modelScreenshot.GetPixel(x + 1, y)) &&
            NotBlack(modelScreenshot.GetPixel(x - 1, y)) &&
            NotBlack(modelScreenshot.GetPixel(x + 1, y + 1)) &&
            NotBlack(modelScreenshot.GetPixel(x - 1, y + 1)) &&
            NotBlack(modelScreenshot.GetPixel(x + 1, y - 1)) &&
            NotBlack(modelScreenshot.GetPixel(x - 1, y - 1)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Projects a given image on to main avatar from main camera viewpoint and optionally fills in the mask.
    /// </summary>
    /// <param name="inpaintedImg">The texture to be painted on. This should be a pre-existing Texture2D object.</param>
    /// <param name="fillMask">Indicates whether the mask should be filled as part of the painting process.</param>
    void PaintFromPose(Texture2D inpaintedImg, bool fillMask)
    {
        RaycastHit hit;
        for (int x = 0; x < VIEWPOINT_SIZE; x++)
        {
            for (int y = 0; y < VIEWPOINT_SIZE; y++)
            {
                Vector3 pos = new Vector3((float)x, (float)y, 0.0f);
                if (Physics.Raycast(mainCam.ScreenPointToRay(pos), out hit))
                {
                    Color col = inpaintedImg.GetPixel(x, y);
                    col.a = 1.0f;
                    if (PtValid(x, y))
                    {
                        Paint(hit, col, false);
                        if (fillMask)
                        {
                            Paint(hit, Color.black, true);
                        }
                    }
                }
            }
        }
    }

}
