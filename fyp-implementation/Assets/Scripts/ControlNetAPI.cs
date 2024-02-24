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
    private static int count = 0;
    private static ControlNetAPI instance;
    private readonly string txt2img = "/sdapi/v1/txt2img";
    private readonly string img2img = "/sdapi/v1/img2img";
    private readonly string sdWebUIApiEndpoint1 = "http://128.16.15.169:7860/";
    private readonly string sdWebUIApiEndpoint2 = "http://128.16.14.135:7860/";

    private void Awake()
    {
        instance = this;
    }

    public static void GetControlNetImg2Img_Static(Texture2D mask, Texture2D image, Texture2D depth, string prompt, string neg_prompt)
    {
        instance.GetControlNetImg2Img(mask, image, depth, prompt, neg_prompt);
        Debug.Log(count);
        count++;
    }

    public static void GetControlNetTxt2Img_Static(Texture2D depth, string prompt, string neg_prompt)
    {
        instance.GetControlNetTxt2Img(depth, prompt, neg_prompt);
        Debug.Log(count);
        count++;
    }

    private void GetControlNetImg2Img(Texture2D mask, Texture2D image, Texture2D depth, string prompt, string neg_prompt)
    {
        // Open and convert the mask image to base64 string
        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(mask));
        // Open and convert the target image to base64 string
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(image));
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depth));
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint2), img2img).ToString();

        // var img2imgInpaintArgs1 = new
        // {
        //     prompt = prompt,
        //     negative_prompt = neg_prompt,
        //     width = 512,
        //     height = 512,
        //     denoising_strength = 0.75,
        //     init_images = new List<string> { imgB64 },
        //     mask = depthB64
        // };
        var img2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = neg_prompt,
            width = 512,
            height = 512,
            denoising_strength = 0.75,
            init_images = new List<string> { imgB64 },
            //mask_content = 3,
            mask = maskB64,
            alwayson_scripts = new
            {
                controlnet = new
                {
                    args = new[]
                    {
                        new
                        {   enabled = true,
                            input_image = depthB64,
                            model = "control_v11f1p_sd15_depth [cfd03158]",
                            module = "none",
                            weight = 1.0,
                            control_mode = 0
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(img2imgInpaintArgs);
        Debug.Log("Sending Request");
        StartCoroutine(SendRequest(apiUrl, arguments));
        Debug.Log("End");

    }

    private void GetControlNetTxt2Img(Texture2D depth, string prompt, string neg_prompt)
    {
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depth));
        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint2), txt2img).ToString();

        var txt2ImgArgs = new
        {
            prompt = prompt,
            negative_prompt = neg_prompt,
            width = 512,
            height = 512,
            denoising_strength = 0.75,
            alwayson_scripts = new
            {
                controlnet = new
                {
                    args = new[]
                    {
                        new
                        {   enabled = true,
                            input_image = depthB64,
                            model = "control_v11f1p_sd15_depth [cfd03158]",
                            module = "none",
                            weight = 1.0,
                            control_mode = 0
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(txt2ImgArgs);
        Debug.Log("Sending Request");
        StartCoroutine(SendRequest(apiUrl, arguments));
        Debug.Log("End");

    }

    IEnumerator SendRequest(string apiUrl, string arguments)
    {
        // Make a POST request to the API with the inpainting arguments
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(arguments));
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
    private string TextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }
}
