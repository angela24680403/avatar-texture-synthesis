from rembg import remove 
from PIL import Image 
  
# input_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Textures/camera-view/Front Camera.png"
# output_path = '/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Textures/remove-bg/Front Camera.png' 
input_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Textures/camera-view/Back Camera.png"
output_path = '/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Textures/remove-bg/Back Camera.png' 

input = Image.open(input_path) 
output = remove(input) 
output.save(output_path)