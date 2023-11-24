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

    Color GetMeanPaintedPixelColour(Vector2 curr)
    {
        List<Color> colours = new List<Color>();
        for (int xOffset = -2; xOffset <= 2; xOffset++)
        {
            for (int yOffset = -2; yOffset <= 2; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0)
                {
                    continue; // Skip the center point (curr.x, curr.y)
                }

                Vector2 coord = new Vector2(curr.x + xOffset, curr.y + yOffset);

                // PROBLEM float and int coordinates are not compatible
                if (paintedUVCoord.Contains(coord))
                {
                    colours.Add(tex.GetPixel((int)coord.x, (int)coord.y));
                }
            }
        }
        if (colours.Count != 0)
        {
            Debug.Log("!");
            return Color.red; //GetMeanColour(colours);
        }
        else
        {
            return tex.GetPixel((int)curr.x, (int)curr.y);
        }
    }

    Color GetMeanColour(List<Color> colors)
    {
        float totalR = 0f;
        float totalG = 0f;
        float totalB = 0f;

        foreach (Color color in colors)
        {
            totalR += color.r;
            totalG += color.g;
            totalB += color.b;
        }

        float meanR = totalR / colors.Count;
        float meanG = totalG / colors.Count;
        float meanB = totalB / colors.Count;

        return new Color(meanR, meanG, meanB);
    }



    void Interpolate()
    {
        int min_x = (int)paintedUVCoord[0].x;
        int min_y = (int)paintedUVCoord[0].y;
        int max_x = (int)paintedUVCoord[0].x;
        int max_y = (int)paintedUVCoord[0].y;

        for (int i = 1; i < paintedUVCoord.Count; i++)
        {
            Vector2 point = paintedUVCoord[i];

            if (point.x < min_x)
                min_x = (int)point.x;
            if (point.y < min_y)
                min_y = (int)point.y;
            if (point.x > max_x)
                max_x = (int)point.x;
            if (point.y > max_y)
                max_y = (int)point.y;
        }
        for (int y = min_y; y < min_y + 1; y++)
        {
            for (int x = min_x; x < max_x; x++)
            {
                Vector2 curr = new Vector2(x, y);
                if (!paintedUVCoord.Contains(curr))
                {
                    tex.SetPixel(x, y, GetMeanPaintedPixelColour(curr));
                    tex.Apply();
                }

            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PaintFromMaskImg();
            Interpolate();
            Debug.Log("Done");
        }

    }
}
