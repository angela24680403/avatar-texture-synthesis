# TextureGen.AI: A ControlNet-Guided Approach to 3D Object Texture Style Transfer and Synthesis

Welcome to the TextureGen.AI Repository! This tool aims to help designers and researchers to find optimal ControlNet parameters for texture style transfer and synthesis with a proof-of-concept pipeline to demonstration an end-to-end texture generation once the optimal parameters are found.

Key Features:

- **ControlNet API Integration**: Utilising the Inpaint and Depth modules to generate new designs for avatar appearance based on text prompt, as well as generating images that allows novel-view synthesis for a 360-degree design generation.
- **Camera System**: A simple system that allows all views of the 3D object to be covered for forward-mapping projection.
- **Object rotation**: Allows object to be rotated so that uncovered regions can be exposed to the camera for the next forward-mapping projection.
- **Mask generation and dilation**: Allow user to generate mask to highlight uncovered regions to be inpainted by the ControlNet, and add generality in the mask.
- **Depth Camera**: Obtain depth information of the object geometry for the ControlNet to get more context of the scene.


