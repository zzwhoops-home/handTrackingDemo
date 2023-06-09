import cv2
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector
import socket
import json
import numpy as np
import math
import time
import os
from keras.models import load_model

cap = cv2.VideoCapture(0) # uncomment if you have webcam
if not cap.isOpened():
    cap = cv2.VideoCapture("http://10.0.0.115:4747/video") # ONLY OPENS UP WHEN ONE INSTANCE OF THIS IS OPEN! DO NOT OPEN IN BROWSER!
IMG_HEIGHT = 720
IMG_WIDTH = 1280 
cap.set(3, IMG_WIDTH)
cap.set(4, IMG_HEIGHT)
if not cap.isOpened():
    raise IOError("Cannot open webcam")
    
# DRAW BOTH HANDS ON SCREEN
detector = HandDetector(maxHands=1, detectionCon=0.8)

from keras.models import Sequential
from keras.layers import LSTM, Dense, InputLayer, Dropout, Reshape

offset = 20
imgSize = 300

# Comms
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)
serverAddressPort2 = ("127.0.0.1", 5053)

labels = ['at_screen', 'neutral', 'ok', 'pointer_left', 'pointer_right']

model = Sequential([
    InputLayer(input_shape=(21, 3)),
    Dense(64, activation='relu'),
    Dense(32, activation='relu'),
    Dense(5, activation='relu'),
    Reshape((105,)),
    Dense(64, activation='relu'),
    Dense(32, activation='relu'),
    Dense(16, activation='relu'),
    Dense(len(labels), activation='softmax')
])

print(model.summary())
pose_num = 1
TO_TRAIN = labels[pose_num]
NUM_IMAGES_RECORD = 200
TRAINING = False

if TRAINING:
    print("======================")
    print(f"RECORDING: {TO_TRAIN}")
    print("======================")
else:
    model = load_model("./models/FiveMovements_20000_steps.h5")
folder = f"./train_data_points/"
counter = 0

while(True):
    success, img = cap.read()
    # flipping image to make everything easier to control (like a mirror)

    img = cv2.flip(img, 1)
    hands, img = detector.findHands(img, flipType=False)

    data = []
    
    if hands:
        hand = hands[0]
        x, y, w, h = hand['bbox']

        imgWhite = np.ones((imgSize, imgSize, 3), np.uint8) * 255
        imgCrop = img[y - offset:y + h + offset, x - offset:x + w + offset]

        imgCropShape = imgCrop.shape

        aspectRatio = h / w

        if aspectRatio > 1:
            k = imgSize / h
            wCal = math.ceil(k * w)
            try:
                imgResize = cv2.resize(imgCrop, (wCal, imgSize))
            except:
                pass
            imgResizeShape = imgResize.shape
            wGap = math.ceil((imgSize - wCal) / 2)
            try:
                imgWhite[:, wGap:wCal + wGap] = imgResize
            except:
                print("hand out of range!")
                pass

            

        else:
            k = imgSize / w
            hCal = math.ceil(k * h)
            try:
                imgResize = cv2.resize(imgCrop, (imgSize, hCal))
            except:
                pass
            imgResizeShape = imgResize.shape
            hGap = math.ceil((imgSize - hCal) / 2)
            try:
                imgWhite[hGap:hCal + hGap, :] = imgResize
            except:
                print("hand out of range!")
                pass

        # cv2.imshow("ImageCrop", imgCrop)
        # cv2.imshow("ImageWhite", imgWhite)


        # get list of landmarks, add to data object
        lmList = hand["lmList"]
        lmList_in = np.expand_dims(np.array(lmList, dtype=np.uint8), axis=0)
        string_pred = None
        if not TRAINING:
            prediction = model(lmList_in)
            print("PREDICITON: ", labels[np.argmax(prediction)])
            string_pred = labels[np.argmax(prediction)]
        for lm in lmList:
            data.extend([lm[0], IMG_HEIGHT - lm[1], lm[2]])
        # add "center" landmark to the end
        data.append(hand["center"])
        sock.sendto(str.encode(str(data)), serverAddressPort)
        sock.sendto(str.encode(str(string_pred)), serverAddressPort2)
    
    cv2.imshow('Image', img)
    key = cv2.waitKey(1)
    if (key == ord("s") or key == 32) and TRAINING:
        counter += 1
        lmList = np.array(lmList, dtype=np.uint8)
        np.save(os.path.join(folder, TO_TRAIN, f"{TO_TRAIN}2_{counter}"), lmList)
        print(f"Count:{counter}")
        if counter >= NUM_IMAGES_RECORD:
            break
    if key & 0xFF == ord('q'):
        break

if TRAINING:
    print("======================")
    print(f"END RECORDING OF: {TO_TRAIN}")
    print("======================")

cap.release()
cv2.destroyAllWindows()