using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthRenderer : MonoBehaviour
{
    public Camera cam;
    [ExecuteInEditMode]
    public Material mat;
    void Start()
    {
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
