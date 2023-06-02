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
# 3 for frame width, 4 for frame height
capture.set(3, 1280)
capture.set(4, 720)

while capture.isOpened():
    success, img = capture.read()

    if not success:
        break

    # print(result.multi_hand_landmarks)
    cv2.imshow("Webcam Display", img)
    cv2.waitKey(20)

    # unicode black magic
    if cv2.waitKey(20) & 0xFF == ord('q'):
        break

capture.release()
cv2.destroyAllWindows()

# Create a hand landmarker instance with the live stream mode:
def print_result(
    result: HandLandmarkerResult, output_image: mp.Image, timestamp_ms: int
):
    print("hand landmarker result: {}".format(result))


options = HandLandmarkerOptions(
    base_options=BaseOptions(model_asset_path="hand_landmarker.task"),
    running_mode=VisionRunningMode.LIVE_STREAM,
    result_callback=print_result,
)

with HandLandmarker.create_from_options(options) as landmarker:
    # The landmarker is initialized. Use it here.
    # ...
    exit()

