using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Drawing;
using Unity.VisualScripting;

public class PaintInverse : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    public GameObject g;
    public Texture2D mask;
    public Texture2D inpaint;
    public Texture2D avatarTexture;

    bool notBlack(UnityEngine.Color color)
    {
        return color.r != 0.0f && color.g != 0.0f && color.b != 0.0f;
    }

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
        Debug.Log("Split mesh.");
        IterateThroughTexturePixels(mesh);
        Debug.Log("Colour in.");
        avatarTexture.Apply();
    }

    void SplitMesh(Mesh mesh)
    {
        // Code Reference: https://stackoverflow.com/questions/45854076/set-color-for-each-vertex-in-a-triangle
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
    }

    float GetTriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (float)Math.Abs((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2.0);
    }

    bool IsPointInsideTriangle(Vector2[] triangle2DVertices, Vector2 pointUV)
    {
        // ref: https://www.geeksforgeeks.org/check-whether-a-given-point-lies-inside-a-triangle-or-not/
        double A = GetTriangleArea(triangle2DVertices[0], triangle2DVertices[1], triangle2DVertices[2]);
        double A1 = GetTriangleArea(pointUV, triangle2DVertices[1], triangle2DVertices[2]);
        double A2 = GetTriangleArea(pointUV, triangle2DVertices[0], triangle2DVertices[2]);
        double A3 = GetTriangleArea(pointUV, triangle2DVertices[0], triangle2DVertices[1]);
        return (A == A1 + A2 + A3);
    }

    (Vector2[], int) GetContainingTriangle(Vector2 point, Mesh mesh)
    {
        Vector2[] curr_triangle;
        for (int n = 0; n < mesh.vertexCount; n = n + 3)
        {
            curr_triangle = new Vector2[] { mesh.uv[n], mesh.uv[n + 1], mesh.uv[n + 2] };
            if (IsPointInsideTriangle(curr_triangle, point))
            {
                Debug.Log(n);
                return (curr_triangle, n);
            }
        }
        curr_triangle = new Vector2[] { mesh.uv[0], mesh.uv[1], mesh.uv[2] };
        return (curr_triangle, 0);
    }

    float[] Barycentric(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        // Check if ComputeBarycentricCoordinates computes the same
        Vector2 v0 = b - a, v1 = c - a, v2 = p - a;
        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;
        float[] baryCoord = { u, v, w };
        return baryCoord;
    }

    float[] ComputeBarycentricCoordinates(Vector2[] triangle2DVertices, Vector2 P)
    {
        Vector2 A = triangle2DVertices[0];
        Vector2 B = triangle2DVertices[1];
        Vector2 C = triangle2DVertices[2];

        float detT = (B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y);

        // Calculate barycentric coordinates
        float u = ((P.y - A.y) * (C.x - A.x) - (P.x - A.x) * (C.y - A.y)) / detT;
        float v = ((P.y - A.y) * (B.x - A.x) - (P.x - A.x) * (B.y - A.y)) / detT;
        float w = 1 - u - v;

        float[] baryCoord = { u, v, w };

        return baryCoord;
    }

    Vector3 Get3DTriangle(float[] baryCoord, Vector3[] triangle3DVertices)
    {
        Vector3 A = triangle3DVertices[0];
        Vector3 B = triangle3DVertices[1];
        Vector3 C = triangle3DVertices[2];
        float u = baryCoord[0];
        float v = baryCoord[1];
        float w = baryCoord[2];

        float pX = u * A.x + v * B.x + w * C.x;
        float pY = u * A.y + v * B.y + w * C.y;
        float pZ = u * A.z + v * B.z + w * C.z;

        Vector3 point3D = new Vector3(pX, pY, pZ);
        return point3D;
    }

    void IterateThroughTexturePixels(Mesh mesh)
    {
        for (int x = 0; x < avatarTexture.width; x++)
        {
            for (int y = 0; y < avatarTexture.height; y++)
            {
                Vector2 point = new Vector2((float)x / avatarTexture.width, (float)y / avatarTexture.height);
                var (triangle, n) = GetContainingTriangle(point, mesh);
                float[] baryCoord = ComputeBarycentricCoordinates(triangle, point);
                Vector3[] triangle3DVertices = { mesh.vertices[n], mesh.vertices[n + 1], mesh.vertices[n + 2] };
                Vector3 mapped3DPoint = Get3DTriangle(baryCoord, triangle3DVertices);
                Vector2 screenPos = cam.WorldToScreenPoint(g.transform.TransformPoint(mesh.vertices[n]));
                SetColor(screenPos, new Vector2(x, y));
            }
        }
    }

    void SetColor(Vector2 screenPos, Vector2 point)
    {
        UnityEngine.Color color;
        if (notBlack(mask.GetPixel((int)screenPos.x, (int)screenPos.y)))
        {
            color = inpaint.GetPixel((int)screenPos.x, (int)screenPos.y);
            //color = UnityEngine.Color.red;
        }
        else
        {
            color = UnityEngine.Color.white;
        }
        avatarTexture.SetPixel((int)point.x, (int)point.y, color);
    }

    void ShowTriangles(Mesh mesh)
    {
        UnityEngine.Color[] colors = new UnityEngine.Color[mesh.vertexCount];
        for (int i = 0; i < colors.Length; i += 3)
        {
            colors[i] = UnityEngine.Color.red;
            colors[i + 1] = UnityEngine.Color.green;
            colors[i + 2] = UnityEngine.Color.blue;
        }
        mesh.colors = colors;
    }
}
