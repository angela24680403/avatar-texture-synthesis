# TextureGen.AI: A ControlNet-Guided Approach to 3D Object Texture Modification and Synthesis

Welcome to the TextureGen.AI Repository! This tool aims to help designers and researchers find optimal ControlNet parameters for texture modification and synthesis with a proof-of-concept pipeline to demonstrate an end-to-end texture generation once the optimal parameters are found.

Key Features:

- **ControlNet API Txt2Img and Img2Img Integration**: Utilising the Inpaint and Depth modules to generate new designs for avatar appearance based on text prompt, as well as generating images that allow novel-view synthesis for a 360-degree design generation.
- **Camera System**: A simple system that allows dynamic views of the 3D object to be covered for forward-mapping projection.
- **Object rotation**: Allows an object to be rotated so that uncovered regions can be exposed to the camera for the next forward-mapping projection.
- **Mask generation and dilation**: Allow the user to generate a mask to highlight uncovered regions to be inpainted by the ControlNet, and add generality in the mask.
- **Depth Camera**: Obtain depth information of the object geometry for the ControlNet to get more context of the scene.

GUI demonstration:

**Texture Modification**: Modifies an existing texture, such as adding star patterns to an existing top.
<img width="881" alt="Screenshot 2024-03-27 at 15 57 26" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/cb0aa0a2-47a6-4555-a4ee-9fb7c45d4e8d">

Prompt: (((detailed floral patterns)), pink top, blue jeans, octane render, skin-coloured arms.

<img width="506" alt="Screenshot 2024-03-29 at 20 18 34" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/68e87933-2134-4e1a-9c77-4bcf27c54668">

Prompt: ((detailed yellow butterflies)), pink top, blue jeans, octane render, skin-coloured arms.

<img width="538" alt="Screenshot 2024-03-29 at 20 19 10" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/406641da-433d-41f1-9cf5-aff161988c3f">



You can access the tutorial here: [Texture Modification Tutorial](https://youtu.be/AQRaxg3edHc)

**Texture Synthesis**: Synthesises a new texture from a blank white image.

This includes two steps:
1. Generate New Design.
2. Rotate Camera, generate a new view fill.

   
<img width="887" alt="Screenshot 2024-03-27 at 16 07 01" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/a92ee523-1562-42d7-874d-6775c5eca036">

Here is a simple walk-through:
First, configure parameters to generate a desired front view design, then project this onto the main avatar.

<img width="492" alt="Screenshot 2024-03-27 at 16 11 00" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/5c7f895f-6b92-40e6-aa58-60a29e7941eb">

Next, rotate the avatar, take screenshots and change parameter configurations for new view fill.

<img width="497" alt="Screenshot 2024-03-27 at 16 11 14" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/7b9db0a5-3848-4832-b332-facf902fbb06">

The new view fill response image should appear under the projected button. Project this onto the avatar.

<img width="496" alt="Screenshot 2024-03-27 at 16 16 00" src="https://github.com/angela24680403/avatar-texture-synthesis/assets/72133521/13e14b0d-07f5-45c9-8aeb-38e5cdb4300d">

Repeat this until the avatar is fully covered.









