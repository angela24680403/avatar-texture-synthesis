# Blur texture
import cv2 as cv
import numpy as np
from matplotlib import pyplot as plt

img = cv.imread('new_skin.png')
assert img is not None, "file could not be read, check with os.path.exists()"

new_height, new_width, _ = img.shape
resized_image = cv.resize(img, (new_width, new_height), interpolation=cv.INTER_LINEAR)

plt.subplot(121),plt.imshow(img),plt.title('Original')
plt.xticks([]), plt.yticks([])
plt.subplot(122),plt.imshow(resized_image),plt.title('interpolation')
plt.xticks([]), plt.yticks([])
plt.show()

