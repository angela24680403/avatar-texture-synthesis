using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class paint_cube : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    public Texture2D mask_img;

    void Start()
    {
        Debug.Log(Screen.width);
        Debug.Log(mask_img.width);
        // simplify for more consistency in testing
        cam = GetComponent<Camera>();
        // Ensure mask size is same as screen size
        float sceneWidth = 10;
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;

    }

    bool IsWhite(Color color)
    {
        return Mathf.Approximately(color.r, 0.9f) && Mathf.Approximately(color.g, 0.9f) && Mathf.Approximately(color.b, 0.9f);
    }

    bool notBlack(Color color)
    {
        return color.r != 0.0f && color.g != 0.0f && color.b != 0.0f;
    }

    void Paint(RaycastHit hit)
    {
        MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (!(rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null))
        {
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
            tex.Apply();
        }
    }

    void MousePaint(RaycastHit hit)
    {
        // This is what I used to demo last week!
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Debug.Log(Input.mousePosition);

        Paint(hit);
    }


    Vector3 worldPosition;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // look into shader code
            RaycastHit hit;

            for (int y = 0; y < mask_img.height; y++)
            {
                for (int x = 0; x < mask_img.width; x++)
                {

                    Color pixelColor = mask_img.GetPixel(x, y);



                    if (notBlack(pixelColor))
                    {
                        // Vector3 new_pos = new Vector3((float)(x - (Screen.width / 2)), (float)(y - (Screen.height / 2)), 0.0f);
                        Vector3 pos = new Vector3((float)x, (float)y, 0.0f);

                        // maybe we don't need raycast?
                        if (Physics.Raycast(cam.ScreenPointToRay(pos), out hit))
                        {
                            Paint(hit);
                        }
                    }
                }
            }
        }

    }
}
