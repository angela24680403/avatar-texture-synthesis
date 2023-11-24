using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CameraSnapshot : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Camera cam = gameObject.GetComponent<Camera>();
            SceneView sceneView = SceneView.lastActiveSceneView;
            int width = cam.pixelWidth;
            int height = cam.pixelHeight;
            Texture2D capture = new Texture2D(width, height);
            sceneView.camera.Render();
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
