using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The depth renderer to obtain a depth viewpoint of the avatar.
/// </summary>
public class DepthRenderer : MonoBehaviour
{
    /// <summary>
    /// The depth camera.
    /// </summary>
    public Camera depthCam;

    /// <summary>
    /// The material that displays the depth render.
    /// </summary>
    public Material depthMat;

    /// <summary>
    /// Set the depth camera texture mode at the start of runtime.
    /// </summary>
    void Start()
    {
        depthCam.depthTextureMode = DepthTextureMode.DepthNormals;
    }

    /// <summary>
    /// Processes the camera's render output by coping pixels from source to destination while 
    /// using the shader attached to the depth material.
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, depthMat);
    }
}
