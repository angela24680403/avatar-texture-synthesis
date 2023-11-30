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
    public GameObject g;
    public Texture2D mask;

    bool notBlack(Color color)
    {
        return color.r != 0.0f && color.g != 0.0f && color.b != 0.0f;
    }

    // Code Reference: https://stackoverflow.com/questions/45854076/set-color-for-each-vertex-in-a-triangle
    void Start()
    {
        cam = GetComponent<Camera>();
        // Ensure mask size is same as screen size
        float sceneWidth = 10;
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;

        Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
        SplitMesh(mesh);
        SetColors(mesh);
    }

    void SplitMesh(Mesh mesh)
    {

        int[] triangles = mesh.triangles;
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        Vector3[] newVerts;
        Vector3[] newNormals;
        Vector2[] newUvs;

        int n = triangles.Length;
        newVerts = new Vector3[n];
        newNormals = new Vector3[n];
        newUvs = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            newVerts[i] = verts[triangles[i]];
            newNormals[i] = normals[triangles[i]];
            if (uvs.Length > 0)
            {
                newUvs[i] = uvs[triangles[i]];
            }
            triangles[i] = i;
        }
        mesh.vertices = newVerts;
        mesh.normals = newNormals;
        mesh.uv = newUvs;
        mesh.triangles = triangles;
        Debug.Log(mesh.triangles[1]);
        Debug.Log(mesh.vertices.Length);
        Debug.Log(mesh.triangles.Length);
        Debug.Log(mesh.uv.Length);
    }

    void SetColors(Mesh mesh)
    {
        Color[] colors = new Color[mesh.vertexCount];

        Vector3[] screen_positions = new Vector3[mesh.vertexCount];
        for (int i = 0; i < screen_positions.Length; i++)
        {
            screen_positions[i] = cam.WorldToScreenPoint(g.transform.TransformPoint(mesh.vertices[i]));
            if (notBlack(mask.GetPixel((int)screen_positions[i].x, (int)screen_positions[i].y - 60)))
            {
                colors[i] = Color.blue;
            }
            else
            {
                colors[i] = Color.white;
            }
        }

        mesh.colors = colors;


    }

}
