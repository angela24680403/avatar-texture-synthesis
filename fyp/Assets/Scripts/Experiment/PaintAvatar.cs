using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class PaintAvatar : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    public Texture2D maskImg;

    public Texture2D tex;
    private List<Vector2> paintedUVCoord = new List<Vector2>();


    void Start()
    {
        Debug.Log(Screen.width);
        cam = GetComponent<Camera>();
        // Ensure mask size is same as screen size
        float sceneWidth = 10;
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;

    }

    bool notBlack(Color color)
    {
        return color.r != 0.0f && color.g != 0.0f && color.b != 0.0f;
    }

    void Paint(RaycastHit hit)
    {
        SkinnedMeshRenderer rend = hit.transform.GetComponent<SkinnedMeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (!(rend == null || meshCollider == null))
        {
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            paintedUVCoord.Add(pixelUV);
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
            tex.Apply();
        }
    }

    void PaintFromMaskImg()
    {
        // look into shader code
        RaycastHit hit;

        for (int y = 0; y < maskImg.height; y++)
        {
            for (int x = 0; x < maskImg.width; x++)
            {

                Color pixelColor = maskImg.GetPixel(x, y);
                if (notBlack(pixelColor))
                {
                    Vector3 pos = new Vector3((float)x, (float)y, 0.0f);
                    if (Physics.Raycast(cam.ScreenPointToRay(pos), out hit))
                    {
                        Paint(hit);
                    }
                }
            }
        }
        Debug.Log(paintedUVCoord.Count);

    }

    void SaveNewSkin()
    {
        // Save new skin as image
        byte[] newSkin = tex.EncodeToPNG();
        string filename = "new_skin.png";
        File.WriteAllBytes(Application.dataPath + "/" + filename, newSkin);
        AssetDatabase.Refresh();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PaintFromMaskImg();
            // Interpolate();
            Debug.Log("Done");
        }

    }
}
