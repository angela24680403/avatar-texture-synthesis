using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPainter : MonoBehaviour
{
    private bool controlnet_running = false;
    private int[] window = { 0, 0, 0, 0 };
    private int count = 0;
    [SerializeField]
    public int KERNELSIZE = 7;
    public GameObject mainAvatar;
    public GameObject modelAvatar;
    public GameObject maskAvatar;
    public Camera mainCam;
    public Camera modelCam;
    public Camera maskCam;
    public Camera depthCam;
    public Texture2D modelScreenshot;
    public Texture2D maskScreenshot;
    public Texture2D inpaintedImage;
    public Texture2D maskTexture;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateAll();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenshotAll();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Project();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DilateMask();
        }
    }


    public void DilateMask()
    {
        ImageProcessing.DilateMask_Static(maskScreenshot, KERNELSIZE);
    }


    public void Project()
    {
        PaintFromPose(inpaintedImage, false);
    }

    public void ProjectWithMaskFill()
    {
        PaintFromPose(inpaintedImage, true);
    }


    public void ScreenshotAll()
    {
        Debug.Log("Taking screenshot...");
        Screenshot.Screenshot_Static(mainCam);
        Screenshot.Screenshot_Static(maskCam);
        Screenshot.Screenshot_Static(modelCam);
        Screenshot.Screenshot_Static(depthCam);
    }

    public void RotateAll()
    {
        Rotate(modelAvatar, 45f);
        Rotate(maskAvatar, 45f);
        Rotate(mainAvatar, 45f);
    }

    void Rotate(GameObject avatar, float angle)
    {
        float targetAngle = avatar.transform.rotation.eulerAngles.y + angle;
        avatar.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public void SavePaintedTexture()
    {
        // send a raycast to centre of screen to get main texture
        // then save it. 
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
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, col);
            tex.SetPixel((int)pixelUV.x + 1, (int)pixelUV.y, col);
            tex.SetPixel((int)pixelUV.x - 1, (int)pixelUV.y, col);
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y + 1, col);
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y - 1, col);
            tex.SetPixel((int)pixelUV.x + 1, (int)pixelUV.y + 1, col);
            tex.SetPixel((int)pixelUV.x + 1, (int)pixelUV.y - 1, col);
            tex.SetPixel((int)pixelUV.x - 1, (int)pixelUV.y - 1, col);
            tex.SetPixel((int)pixelUV.x - 1, (int)pixelUV.y - 1, col);
            tex.Apply();
        }

    }

    bool not_black(Color col)
    {
        return col.r > 0 && col.r > 0 && col.r > 0;
    }

    bool pt_valid(int x, int y)
    {
        Color mask_col = modelScreenshot.GetPixel(x, y);
        if (not_black(mask_col))
        {
            if (not_black(modelScreenshot.GetPixel(x, y + 1)) &&
            not_black(modelScreenshot.GetPixel(x, y - 1)) &&
            not_black(modelScreenshot.GetPixel(x + 1, y)) &&
            not_black(modelScreenshot.GetPixel(x - 1, y)) &&
            not_black(modelScreenshot.GetPixel(x + 1, y + 1)) &&
            not_black(modelScreenshot.GetPixel(x - 1, y + 1)) &&
            not_black(modelScreenshot.GetPixel(x + 1, y - 1)) &&
            not_black(modelScreenshot.GetPixel(x - 1, y - 1)))
            {
                return true;
            }
        }
        return false;
    }

    void PaintFromPose(Texture2D texture, bool fill_mask)
    {
        Debug.Log("P pressed!! View");

        RaycastHit hit;

        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                Vector3 pos = new Vector3((float)x, (float)y, 0.0f);
                if (Physics.Raycast(mainCam.ScreenPointToRay(pos), out hit))
                {
                    Color col = texture.GetPixel(x, y);

                    col.a = 1.0f;
                    if (pt_valid(x, y))
                    {
                        Paint(hit, col, false);
                        if (fill_mask){
                            Paint(hit, Color.black, true);
                        }
                        
                    }

                }
            }
        }
        Debug.Log("done");
        SavePaintedTexture();
    }

}
