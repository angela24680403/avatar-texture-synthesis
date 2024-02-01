# ControlNet Avatar Texture Synthesis

This repository demonstrates a proof-of-concept pipeline that uses the ControlNet API to produce new avatar texture given a text prompt.

Key Features:

- ControlNet API Integration: Utilising the Inpaint and Depth modules to generate new designs for avatar appearance based on text prompt, as well as generating images that allows novel-view synthesis for a 360 degree design generation.
- Camera System: A simple system that allows all views of the avatar to be covered for forward-mapping projection.
- Extensibility: The original avatar mesh can be replaced by any 3D object, though may need to adjust camera system to optimise forward-mapping projection quality.
