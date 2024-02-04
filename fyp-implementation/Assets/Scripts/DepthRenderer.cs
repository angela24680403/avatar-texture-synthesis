using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthRenderer : MonoBehaviour
{
    public Camera cam;
    public Material mat;
    void Start()
    {
        if (cam == null)
        {
            cam = this.GetComponent<Camera>();
            cam.depthTextureMode = DepthTextureMode.DepthNormals;
        }
        if (mat == null)
        {
            mat = new Material(Shader.Find("Hidden/DepthShader"));
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
