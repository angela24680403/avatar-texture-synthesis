import numpy as np
from PIL import Image
import cv2
from scipy.stats import mode
def remove_background(image_path, mask_path, output_path):
    # Read the input image and mask
    image = cv2.imread(image_path)
    mask = cv2.imread(mask_path, cv2.IMREAD_GRAYSCALE)

    # Make sure the image and mask have the same dimensions
    if image.shape[:2] != mask.shape:
        mask = cv2.resize(mask, (image.shape[1], image.shape[0]))

    # Convert the mask to a binary mask
    _, binary_mask = cv2.threshold(mask, 1, 255, cv2.THRESH_BINARY)

    inverted_mask = cv2.bitwise_not(binary_mask)

    non_masked_pixels = image[inverted_mask > 0]
    most_common_color = mode(non_masked_pixels, axis=0).mode[0].astype(np.uint8)

    alpha_channel = np.ones(inverted_mask.shape, dtype=image.dtype) * 255

    # Set the alpha channel in the masked area to 0
    alpha_channel[inverted_mask > 0] = 0

    # Add the alpha channel to the image
    result = cv2.merge([image, alpha_channel])

    # Save the result
    cv2.imwrite(output_path, result)

    return most_common_color

def colour_in(image_path, output_path, colour):
    image = cv2.imread(image_path)
    mask = cv2.imread(image_path,  cv2.IMREAD_GRAYSCALE)
    _, binary_mask = cv2.threshold(mask, 1, 255, cv2.THRESH_BINARY)
    print(colour)
    print(binary_mask.shape)
    image[binary_mask > 0] = colour
    cv2.imwrite(output_path, image)


def overlay_images(background_path, overlay_path, output_path, position=(0, 0)):
    # Read the input images
    image1 = cv2.imread(background_path)
    image2 = cv2.imread(overlay_path)

    # Add the RGBA values together
    result_image = cv2.addWeighted(image1, 1.0, image2, 1.0, 0)

    # Save the result
    cv2.imwrite(output_path, result_image)


if __name__ == "__main__":
    # Replace these paths with your actual file paths
    image_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Inpaint.png"
    mask_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Mask.png"
    mask2_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/MaskPadding.png"
    output_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Result.png"
    output2_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Result2.png"
    output3_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Result3.png"
    output4_path = "/Users/angelayu/Documents/GitHub/avatar-texture-synthesis/fyp/Assets/Scripts/Textures/Result4.png"

    colour = remove_background(image_path, mask_path, output_path)
    remove_background(mask2_path, mask2_path, output2_path)
    colour_in(mask2_path, output3_path, colour)
    remove_background(output3_path, mask2_path, output3_path)
    #overlay_images(output3_path, output_path, output4_path)




