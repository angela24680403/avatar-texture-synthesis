using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class screenshot : MonoBehaviour
{
    public Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cam = GetComponent<Camera>();
            int width = cam.pixelWidth;
            int height = cam.pixelHeight;
            Texture2D capture = new Texture2D(width, height);
            cam.Render();
            RenderTexture.active = cam.targetTexture;
            capture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            capture.Apply();

            byte[] bytes = capture.EncodeToPNG();
            string filename = "sceneViewCapture.png";
            File.WriteAllBytes(Application.dataPath + "/" + filename, bytes);
            AssetDatabase.Refresh();
        }
        
    }
}
