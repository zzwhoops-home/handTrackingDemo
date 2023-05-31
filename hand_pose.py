# WIP, DOES NOT RUN YET

import cv2
import numpy as np
import mediapipe as mp
import matplotlib.pyplot as plt

BaseOptions = mp.tasks.BaseOptions
HandLandmarker = mp.tasks.vision.HandLandmarker
HandLandmarkerOptions = mp.tasks.vision.HandLandmarkerOptions
HandLandmarkerResult = mp.tasks.vision.HandLandmarkerResult
VisionRunningMode = mp.tasks.vision.RunningMode

# camera instance
capture = cv2.VideoCapture(0)

while True:
  success, img = capture.read()

  if not success:
    break
  cv2.imshow("Webcam", img)
  cv2.waitKey(25)

# Create a hand landmarker instance with the live stream mode:
def print_result(result: HandLandmarkerResult, output_image: mp.Image, timestamp_ms: int):
    print('hand landmarker result: {}'.format(result))

options = HandLandmarkerOptions(
    base_options=BaseOptions(model_asset_path='/path/to/model.task'),
    running_mode=VisionRunningMode.LIVE_STREAM,
    result_callback=print_result)

with HandLandmarker.create_from_options(options) as landmarker:
  # The landmarker is initialized. Use it here.
  # ...
  exit()

capture.release()
cv2.destroyAllWindows()