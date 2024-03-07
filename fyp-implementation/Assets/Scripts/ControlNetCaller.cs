using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
public class ControlNetCaller : MonoBehaviour
{
    public Texture2D mainScreenshot;
    public Texture2D maskScreenshot;
    public Texture2D dilatedMaskScreenshot;
    public Texture2D modelScreenshot;
    public Texture2D depthScreenshot;
    public string prompt = "";
    public string negPrompt = "";
    public float CFG = 0.0f;
    public float denoisingStrength = 0.75f;
    public float controlNetDepthWeight = 1.0f;

    public int maskContent = 3;

    public void ControlNetDesign()
    {
        ControlNetAPI.CallTxt2ImgAPI(NewDesignTxt2ImgArgs());
    }

    public void ControlNetNewViewFill()
    {
        ControlNetAPI.CallImg2ImgAPI(NewViewFillImg2ImgArgs());
    }

    public string NewDesignTxt2ImgArgs()
    {
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depthScreenshot));

        var txt2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = negPrompt,
            width = 512,
            height = 512,
            denoising_strength = denoisingStrength,
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
                            weight = controlNetDepthWeight,
                            control_mode = 0
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(txt2imgInpaintArgs);
        return arguments;
    }

    public string NewViewFillImg2ImgArgs()
    {
        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(dilatedMaskScreenshot));
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(mainScreenshot));
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depthScreenshot));

        var img2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = negPrompt,
            width = 512,
            height = 512,
            denoising_strength = denoisingStrength,
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
                            weight = controlNetDepthWeight,
                            control_mode = 0
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(img2imgInpaintArgs);
        return arguments;
    }

    private string TextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }

}
