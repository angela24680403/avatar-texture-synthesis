using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPainter : MonoBehaviour
{
    private bool controlnet_running = false;
    private int[] window = { 0, 0, 0, 0 };
    private int count = 0;
    [SerializeField]
    public GameObject mainAvatar;
    public GameObject modelAvatar;
    public GameObject maskAvatar;
    public Camera mainCam;
    public Camera modelCam;
    public Camera maskCam;
    public Texture2D mainScreenshot;
    public Texture2D maskScreenshot;
    public Texture2D modelScreenshot;
    public Texture2D inpaintedImage;
    public Texture2D mask;
    public string prompt = "";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Pipeline1();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Pipeline2();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Screenshot.Screenshot_Static(mainCam);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Screenshot.Screenshot_Static(maskCam);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Screenshot.Screenshot_Static(modelCam);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            FindMinScreenWindow();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PaintFromPose(modelScreenshot);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateAvatar.RotateAvatar_Static(mainAvatar, 45f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateAvatar.RotateAvatar_Static(maskAvatar, 45f);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            RotateAvatar.RotateAvatar_Static(modelAvatar, 45f);
        }
    }

    void Pipeline1()
    {
        // fill front
        Screenshot.Screenshot_Static(modelCam);
        Screenshot.Screenshot_Static(maskCam);
        ControlNetAPI.GetControlNetResult_Static(maskScreenshot, modelScreenshot, prompt);
        // inpaintedImage now has the correct image to project
    }

    void Pipeline2()
    {
        FindMinScreenWindow();
        PaintFromPose(inpaintedImage);
        // fill back
        RotateAvatar.RotateAvatar_Static(modelAvatar, 180f);
        RotateAvatar.RotateAvatar_Static(maskAvatar, 180f);
        RotateAvatar.RotateAvatar_Static(mainAvatar, 180f);
        Screenshot.Screenshot_Static(modelCam);
        Screenshot.Screenshot_Static(maskCam);
        ControlNetAPI.GetControlNetResult_Static(maskScreenshot, modelScreenshot, prompt);
    }


    void SavePaintedTexture()
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
            System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/Skin.png", byteArray);
            Debug.Log("Saved texture Skin.png");
        }
        Texture2D readableMask = DecompressTexture.Decompress_Static(mask);
        byte[] maskByteArray = readableMask.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Saved/Mask.png", maskByteArray);
        Debug.Log("Saved texture Mask.png");
        count++;

    }

    void Paint(RaycastHit hit, Color col, bool is_mask)
    {
        SkinnedMeshRenderer rend = hit.transform.GetComponent<SkinnedMeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Texture2D tex = mask;
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
        SavePaintedTexture();
    }

    void PaintFromPose(Texture2D texture)
    {
        Debug.Log("P pressed");

        RaycastHit hit;
        for (int x = window[0]; x < window[2]; x++)
        {
            for (int y = window[1]; y < window[3]; y++)
            {
                Vector3 pos = new Vector3((float)x, (float)y, 0.0f);
                if (Physics.Raycast(mainCam.ScreenPointToRay(pos), out hit))
                {
                    Color col = texture.GetPixel(x, y);
                    col.a = 1.0f;
                    bool is_green = col.g > col.r && col.g > col.b;
                    if (!is_green)
                    {
                        Debug.Log(col);
                        Paint(hit, col, false);
                        Paint(hit, Color.black, true);
                    }

                }
            }
        }
        Debug.Log("done");
    }

    void FindMinScreenWindow()
    {
        int min_x = 2000;
        int min_y = 2000;
        int max_x = 0;
        int max_y = 0;
        RaycastHit hit;

        for (int x = 0; x < Screen.width; x++)
        {
            for (int y = 0; y < Screen.height; y++)
            {
                Vector3 pos = new Vector3((float)x, (float)y, 0.0f);
                if (Physics.Raycast(mainCam.ScreenPointToRay(pos), out hit))
                {
                    if (pos.x < min_x) { min_x = x; }
                    if (pos.y < min_y) { min_y = y; }
                    if (pos.x > max_x) { max_x = x; }
                    if (pos.y > max_y) { max_y = y; }

                }
            }
        }
        window = new int[] { min_x, min_y, max_x, max_y };
        Debug.Log("Window set");
    }
}
