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
    public Texture2D inpaint;
    public Texture2D avatarTexture;

    bool notBlack(Color color)
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
        IterateThroughTexturePixels(mesh);
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
        return abs((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2.0);
    }

    bool IsPointInsideTriangle(Vector2[] triangle2DVertices, Vector2 pointUV)
    {
        // ref: https://www.geeksforgeeks.org/check-whether-a-given-point-lies-inside-a-triangle-or-not/
        /* Calculate area of triangle ABC */
        double A = GetTriangleArea(triangle2DVertices[0], triangle2DVertices[1], triangle2DVertices[2]);

        /* Calculate area of triangle PBC */
        double A1 = GetTriangleArea(pointUV, triangle2DVertices[1], triangle2DVertices[2]);

        /* Calculate area of triangle PAC */
        double A2 = GetTriangleArea(pointUV, triangle2DVertices[0], triangle2DVertices[2]);

        /* Calculate area of triangle PAB */
        double A3 = GetTriangleArea(pointUV, triangle2DVertices[0], triangle2DVertices[1]);

        /* Check if sum of A1, A2 and A3 is same as A */
        return (A == A1 + A2 + A3);
    }

    (Vector2[], int) GetContainingTriangle(Vector2 point, Mesh mesh)
    {
        for (int n = 0; n < mesh.uv.Count; n = n + 3)
        {
            Vector2[] triangle = { mesh.uv[n], mesh.uv[n + 1], mesh.uv[n + 2] };

            if (IsPointInsideTriangle(triangle, point))
            {
                return (triangle, n);
            }
        }
        return null;
    }

    float[] ComputeBarycentricCoordinates(Vector2[] triangle2DVertices, Vector2 P)
    {
        Vector2 A = triangle2DVertices[0];
        Vector2 B = triangle2DVertices[1];
        Vector2 C = triangle2DVertices[2];

        double detT = (B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y);

        // Calculate barycentric coordinates
        double u = ((P.y - A.y) * (C.x - A.x) - (P.x - A.x) * (C.y - A.y)) / detT;
        double v = ((P.y - A.y) * (B.x - A.x) - (P.x - A.x) * (B.y - A.y)) / detT;
        double w = 1 - u - v;

        return { u, v, w };
    }

    Vector3[] Get3DTriangle(float[] baryCoord, Vector3[] triangle3DVertices)
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

        Vector3[] point3D = { pX, pY, pZ };
    }

    void IterateThroughTexturePixels(Mesh mesh)
    {
        for (int x = 0; x < avatarTexture.Width; x++)
        {
            for (int y = 0; y < avatarTexture.Height; y++)
            {
                Vector2 point = new Vector2((float)x / avatarTexture.Width, (float)y / avatarTexture.Height);
                var (triangle, n) = GetContainingTriangle(point, mesh);
                float[] baryCoord = ComputeBarycentricCoordinates(triangle, point);
                Vector3[] triangle3DVertices = { mesh.vertices[n], mesh.vertices[n + 1], mesh.vertices[n + 2] };
                Vector3 mapped3DPoint = Get3DTriangle(baryCoord, triangle3DVertices);
                Vector2 screenPos = cam.WorldToScreenPoint(g.transform.TransformPoint(mapped3DPoint));
                SetColor(screenPos);
            }
        }
    }

    void SetColor(Vector2 screenPos)
    {
        if (notBlack(mask.GetPixel((int)screenPos.x, (int)screenPos.y)))
        {
            colors[i] = inpaint.GetPixel((int)screenPos.x, (int)screenPos.y);
        }
        else
        {
            colors[i] = Color.white;
        }
    }


}
