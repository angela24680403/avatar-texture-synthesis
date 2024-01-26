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
    private readonly string sdWebUIApiEndpoint = "http://128.16.14.135:7860/";
    private readonly string prompt = "cat";
    private int count = 0;

    [SerializeField]
    public Texture2D mask;
    public Texture2D image;


    void Start()
    {

        Debug.Log("Start");

        // Open and convert the mask image to base64 string

        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(mask));

        // Open and convert the target image to base64 string
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(image));

        string apiUrl = new Uri(new Uri(sdWebUIApiEndpoint), "/sdapi/v1/txt2img").ToString();
        StartCoroutine(SendRequest(apiUrl, imgB64, maskB64));

    }

    IEnumerator SendRequest(string apiUrl, string imgB64, string maskB64)
    {
        var inpaintArgs = new
        {
            prompt = prompt,
            batch_size = 1,
            denoising_strength = 1.0,
            steps = 30,
            sampler = "DPM++ 2S a Karras",
            alwayson_scripts = new
            {
                controlnet = new
                {
                    args = new[]
                    {
                        new
                        {
                            enabled = true,
                            input_image = imgB64,
                            model = "control_v11p_sd15_inpaint [ebff9138]",
                            module = "inpaint_only",
                            mask = maskB64,
                            weight = 1.0,
                            control_mode = 0
                        }
                    }
                }
            }
        };

        Debug.Log("Going to send request...");
        Debug.Log(apiUrl);


        // Make a POST request to the API with the inpainting arguments
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(inpaintArgs)));
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
                System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Inpainted/texture" + count.ToString() + ".png", byteArray);
                Debug.Log("Saved inpaint output" + count.ToString() + ".png");
                count++;
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
