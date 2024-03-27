using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

public class InpaintResponse
{
    public List<string> images;
}

public class ControlNetAPI : MonoBehaviour
{
    //private static string sdWebUIApiEndpoint = "http://128.16.14.135:7860/";
    private static string sdWebUIApiEndpoint = "http://128.16.15.169:7860/";
    private static string txt2img = "/sdapi/v1/txt2img";
    private static string img2img = "/sdapi/v1/img2img";
    private static int count = 0;
    private static ControlNetAPI instance;
    private void Awake()
    {
        instance = this;
    }
    public static void CallImg2ImgAPI(string arguments)
    {
        Debug.Log("Sending Img2Img Request");
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), img2img).ToString();
        instance.StaticCallAPI(apiUrl, arguments);
        Debug.Log("End");
        count++;
    }

    public static void CallTxt2ImgAPI(string arguments)
    {
        Debug.Log("Sending Txt2Img Request");
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), txt2img).ToString();
        instance.StaticCallAPI(apiUrl, arguments);
        Debug.Log("End");
        count++;
    }

    private void StaticCallAPI(string apiUrl, string arguments)
    {
        StartCoroutine(SendRequest(apiUrl, arguments));
    }

    IEnumerator SendRequest(string apiUrl, string arguments)
    {
        // Make a POST request to the API with the inpainting arguments
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(arguments));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log(apiUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error Code: {request.responseCode} {request.error}");
                yield break;
            }

            string responseContent = request.downloadHandler.text;
            InpaintResponse jsonResponse = JsonConvert.DeserializeObject<InpaintResponse>(responseContent);

            if (jsonResponse.images.Count != 0)
            {
                string imageB64 = jsonResponse.images[0];
                byte[] imageBytes = Convert.FromBase64String(imageB64);
                Texture2D outputTexture = new Texture2D(2, 2);
                outputTexture.LoadImage(imageBytes);
                outputTexture = ImageProcessing.Resize(outputTexture, 512, 512);
                byte[] byteArray = outputTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/Inpainted.png", byteArray);
                Debug.Log("Saved Inpainted.png");
            }
            else
            {
                Debug.Log(responseContent);
                Debug.LogError("No image generated. Printed response above.");
            }
        }

    }
}
