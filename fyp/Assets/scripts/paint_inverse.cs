using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class PaintInverse : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    public Texture2D maskImg;

    void Start()
    {
        Debug.Log(Screen.width);
        cam = GetComponent<Camera>();
        // Ensure mask size is same as screen size
        float sceneWidth = 10;
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;

        // Get uv coordinates matched with vertices of mesh code taken from documentation
        // Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
        // Vector3[] vertices = mesh.vertices;
        // Vector2[] uvs = new Vector2[vertices.Length];
        // Vector2[] vertices_screen_pos = new Vector2[vertices.Length];

        // //SkinnedMeshRenderer rend = GetComponentInChildren<SkinnedMeshRenderer>();
        // MeshRenderer rend = GetComponentInChildren<MeshRenderer>();
        // Texture2D tex = rend.material.mainTexture as Texture2D;

        // for (int i = 0; i < uvs.Length; i++)
        // {
        //     uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        // }
        // mesh.uv = uvs;

        // for (int i = 0; i < uvs.Length; i++)
        // {
        //     vertices_screen_pos[i] = cam.WorldToScreenPoint(gameObject.transform.TransformPoint(vertices[i]));
        // }
        // Debug.Log(vertices_screen_pos[0]);
    }

    bool notBlack(Color color)
    {
        return color.r != 0.0f && color.g != 0.0f && color.b != 0.0f;
    }

    // void SaveNewSkin()
    // {
    //     // Save new skin as image
    //     byte[] newSkin = tex.EncodeToPNG();
    //     string filename = "new_skin.png";
    //     File.WriteAllBytes(Application.dataPath + "/" + filename, newSkin);
    //     AssetDatabase.Refresh();
    // }

    void Update()
    {
    }
}
