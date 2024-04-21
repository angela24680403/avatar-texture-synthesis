using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
public class ControlNetCaller : MonoBehaviour
{
    /// <summary>
    /// The main screenshot .
    /// </summary>
    public Texture2D mainScreenshot;

    /// <summary>
    /// The mask screenshot .
    /// </summary>
    public Texture2D maskScreenshot;

    /// <summary>
    /// The dilated mask screenshot.
    /// </summary>
    public Texture2D dilatedMaskScreenshot;

    /// <summary>
    /// The model screenshot.
    /// </summary>
    public Texture2D modelScreenshot;

    /// <summary>
    /// The depth screenshot.
    /// </summary>
    public Texture2D depthScreenshot;

    /// <summary>
    /// The SD prompt.
    /// </summary>
    public string prompt = "";

    /// <summary>
    /// The SD negative prompt.
    /// </summary>
    public string negPrompt = "";

    /// <summary>
    /// The CFG parameter.
    /// </summary>
    public float CFG = 7.0f;

    /// <summary>
    /// The denoising parameter.
    /// </summary>
    public float denoisingStrength = 0.75f;

    /// <summary>
    /// The img2img inpaint mask content parameter.
    /// </summary>
    public int maskContent = 3;

    /// <summary>
    /// The seed parameter.
    /// </summary>
    public int seed = -1;

    /// <summary>
    /// The ControlNet depth weight parameter.
    /// </summary>
    public float controlNetDepthWeight = 1.0f;

    /// <summary>
    /// The ControlNet control mode parameter.
    /// </summary>
    public int controlMode = 0;

    /// <summary>
    /// Call API for the SD and ControlNet models with specific arguments
    /// for new front design generation.
    /// </summary>
    public void ControlNetNDesign()
    {
        ControlNetAPI.CallTxt2ImgAPI(NewDesignTxt2ImgArgs());
    }

    /// <summary>
    /// Call API for the SD and ControlNet models with specific arguments
    /// for new view fill.
    /// </summary>
    public void ControlNetNewViewFill()
    {
        ControlNetAPI.CallImg2ImgAPI(NewViewFillImg2ImgArgs());
    }

    /// <summary>
    /// Call API for the SD and ControlNet models with specific arguments
    /// for modifying texture.
    /// </summary>
    public void ControlNetModifyTexture()
    {
        ControlNetAPI.CallImg2ImgAPI(TextureModificationImg2ImgArgs());
    }

    /// <summary>
    /// Get arguments for the new front design txt2img API call.
    /// </summary>
    /// <returns>The arguments in string format.</returns>
    public string NewDesignTxt2ImgArgs()
    {
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depthScreenshot));

        var txt2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = negPrompt,
            seed = seed,
            cfg_scale = CFG,
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
                            control_mode = controlMode
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(txt2imgInpaintArgs);
        return arguments;
    }

    /// <summary>
    /// Get arguments for the new view fill img2img API call.
    /// </summary>
    /// <returns>The arguments in string format.</returns>
    public string NewViewFillImg2ImgArgs()
    {
        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(dilatedMaskScreenshot));
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(mainScreenshot));
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depthScreenshot));

        var img2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = negPrompt,
            seed = seed,
            denoising_strength = denoisingStrength,
            init_images = new List<string> { imgB64 },
            mask_content = maskContent,
            cfg_scale = CFG,
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
                            control_mode = controlMode
                        }
                    }
                }
            }
        };
        string arguments = JsonConvert.SerializeObject(img2imgInpaintArgs);
        return arguments;
    }

    /// <summary>
    /// Get arguments for the texture modification img2img API call.
    /// </summary>
    /// <returns>The arguments in string format.</returns>
    public string TextureModificationImg2ImgArgs()
    {

        string maskB64 = TextureToBase64(DecompressTexture.Decompress_Static(dilatedMaskScreenshot));
        string imgB64 = TextureToBase64(DecompressTexture.Decompress_Static(mainScreenshot));
        string depthB64 = TextureToBase64(DecompressTexture.Decompress_Static(depthScreenshot));
        var img2imgInpaintArgs = new
        {
            prompt = prompt,
            negative_prompt = negPrompt,
            seed = seed,
            cfg_scale = CFG,
            denoising_strength = denoisingStrength,
            init_images = new List<string> { imgB64 },
            mask = depthB64
        };
        string arguments = JsonConvert.SerializeObject(img2imgInpaintArgs);
        return arguments;
    }

    /// <summary>
    /// Converts a Texture2D image to a base64 string.
    /// </summary>
    /// <param name="texture">A Texture2D image</param>
    /// <returns>The base64 string of the image.</returns>
    private string TextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }

}
