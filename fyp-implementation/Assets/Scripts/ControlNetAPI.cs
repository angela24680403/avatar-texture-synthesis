using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// The response from calling the img2img or txt2img API, 
/// containing a list of image as base64 strings.
/// </summary>
public class InpaintResponse
{
    /// <summary>
    /// List of images returned by the API represented as strings.
    /// </summary>
    public List<string> images;
}

/// <summary>
/// This class includes functions that call to img2img and txt2img API.
/// </summary>
public class ControlNetAPI : MonoBehaviour
{

    /// <summary>
    /// The SD Web UI API end point.
    /// </summary>
    private static string sdWebUIApiEndpoint = "http://128.16.15.169:7860/";

    /// <summary>
    /// The relative API endpoint for calling the txt2img model.
    /// </summary>
    private static string txt2img = "/sdapi/v1/txt2img";

    /// <summary>
    /// The relative API endpoint for calling the img2img model.
    /// </summary>
    private static string img2img = "/sdapi/v1/img2img";

    /// <summary>
    /// The ControlNetAPI static instance.
    /// </summary>
    private static ControlNetAPI instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Calling the img2img API.
    /// </summary>
    /// <param name="arguments">The string of serialised arguments.</param>
    public static void CallImg2ImgAPI(string arguments)
    {
        Debug.Log("Sending Img2Img Request");
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), img2img).ToString();
        instance.StaticCallAPI(apiUrl, arguments);
        Debug.Log("End");
    }

    /// <summary>
    /// Calling the txt2img API.
    /// </summary>
    /// <param name="arguments">The string of serialised arguments.</param>
    public static void CallTxt2ImgAPI(string arguments)
    {
        Debug.Log("Sending Txt2Img Request");
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), txt2img).ToString();
        instance.StaticCallAPI(apiUrl, arguments);
        Debug.Log("End");
    }

    /// <summary>
    /// The main API calling process.
    /// </summary>
    /// <param name="apiUrl">The API URL.</param>
    /// <param name="arguments">The string of serialised arguments.</param>
    private void StaticCallAPI(string apiUrl, string arguments)
    {
        StartCoroutine(SendRequest(apiUrl, arguments));
    }

    /// <summary>
    /// Sending an API request.
    /// </summary>
    /// <param name="apiUrl">The API URL.</param>
    /// <param name="arguments">The string of serialised arguments.</param>
    IEnumerator SendRequest(string apiUrl, string arguments)
    {
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

            // Saving the response image as Inpainted.png in the Screenshots folder.
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
