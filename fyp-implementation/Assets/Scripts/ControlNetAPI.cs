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
    private static ControlNetAPI instance;
    private readonly string txt2img = "/sdapi/v1/txt2img";
    private readonly string img2img = "/sdapi/v1/img2img";
    private readonly string sdWebUIApiEndpoint = "http://128.16.15.169:7860/";
    // private var txt2imgInpaintArgs = new
    // {
    //     prompt = prompt,
    //     batch_size = 1,
    //     denoising_strength = 1.0,
    //     steps = 30,
    //     sampler = "DPM++ 2S a Karras",
    //     alwayson_scripts = new
    //     {
    //         controlnet = new
    //         {
    //             args = new[]
    //                 {
    //                     new
    //                     {
    //                         enabled = true,
    //                         input_image = imgB64,
    //                         model = "control_v11p_sd15_inpaint [ebff9138]",
    //                         module = "inpaint_only",
    //                         mask = maskB64,
    //                         weight = 1.0,
    //                         control_mode = 0
    //                     }, new
    //                     {   enabled = true,
    //                         input_image = depthB64,
    //                         model = "control_v11f1p_sd15_depth [cfd03158]",
    //                         module = "depth_midas",
    //                         mask = "",
    //                         weight = 1.0,
    //                         control_mode = 1
    //                     }
    //                 }
    //         }
    //     }
    // };

    private void Awake()
    {
        instance = this;
    }
    public static void GetControlNetResult_Static(Texture2D mask, Texture2D image, Texture2D depth, string prompt)
    {
        instance.GetControlNetResult(mask, image, depth, prompt);
    }
    private void GetControlNetResult(Texture2D mask, Texture2D image, Texture2D depth, string prompt)
    {
        // Open and convert the mask image to base64 string
        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(mask));
        // Open and convert the target image to base64 string
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(image));
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depth));
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), img2img).ToString();
        StartCoroutine(SendRequest(apiUrl, imgB64, maskB64, depthB64, prompt));
    }

    IEnumerator SendRequest(string apiUrl, string imgB64, string maskB64, string depthB64, string prompt)
    {
        var img2imgInpaintArgs = new
        {
            prompt = prompt,
            denoising_strength = 0.75,
            init_images = new List<string> { imgB64 },
            mask = depthB64
        };

        Debug.Log(apiUrl);

        // Make a POST request to the API with the inpainting arguments
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(img2imgInpaintArgs)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

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
                Texture2D decompressedTexture = DecompressTexture.Decompress_Static(outputTexture);
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

    private string TextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }
}
