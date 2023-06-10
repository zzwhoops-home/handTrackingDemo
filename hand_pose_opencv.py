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
from keras.layers import LSTM, Dense, InputLayer, Dropout, Reshape, BatchNormalization

# ===== HYPER PARAMETERS ===== #
IMG_SIZE = 50
TRAINING = False
POSE_NUM = 4 

NUM_IMAGES_RECORD = 500
MODEL_ACTIVE = not TRAINING 

counter = 0
offset = 20
# ===== HYPER PARAMETERS ===== #

# Comms
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

folder = f"./train_data_points/"
labels = ['at_screen', 'neutral', 'peace', 'pointer_left', 'pointer_right']
TO_TRAIN = labels[POSE_NUM]

for label in labels:
    if not os.path.exists(folder + label):
        os.makedirs(folder + label)

model = Sequential([
    InputLayer(input_shape=(21, 3)),
    Reshape((21 * 3,)),
    Dense(32, activation='relu'),
    Dropout(0.2),
    BatchNormalization(),
    Dense(16, activation='relu'),
    Dropout(0.2),
    BatchNormalization(),
    Dense(8, activation='relu'),
    Dropout(0.2),
    BatchNormalization(),
    Dense(len(label), activation='softmax')
])

print(model.summary())


if TRAINING:
    print("======================")
    print(f"RECORDING: {TO_TRAIN}")
    print("======================")
elif MODEL_ACTIVE:
    UPDATE_EPOCH = 1000
    model = load_model(f"./models/FiveMovements_{UPDATE_EPOCH}_steps.h5")


while(True):
    success, img = cap.read()
    # flipping image to make everything easier to control (like a mirror)

    img = cv2.flip(img, 1)
    hands, img = detector.findHands(img, flipType=False)

    # list of landmarks and center tuple to send
    data = []
    center = []
    
    if hands:
        hand = hands[0]
        x, y, w, h = hand['bbox']

        imgWhite = np.ones((IMG_SIZE, IMG_SIZE, 3), np.uint8) * 255
        imgCrop = img[y - offset:y + h + offset, x - offset:x + w + offset]

        imgCropShape = imgCrop.shape

        aspectRatio = h / w

        if aspectRatio > 1:
            k = IMG_SIZE / h
            wCal = math.ceil(k * w)
            try:
                imgResize = cv2.resize(imgCrop, (wCal, IMG_SIZE))
            except:
                pass
            imgResizeShape = imgResize.shape
            wGap = math.ceil((IMG_SIZE - wCal) / 2)
            try:
                imgWhite[:, wGap:wCal + wGap] = imgResize
            except:
                print("hand out of range!")
                pass

            

        else:
            k = IMG_SIZE / w
            hCal = math.ceil(k * h)
            try:
                imgResize = cv2.resize(imgCrop, (IMG_SIZE, hCal))
            except:
                pass
            imgResizeShape = imgResize.shape
            hGap = math.ceil((IMG_SIZE - hCal) / 2)
            try:
                imgWhite[hGap:hCal + hGap, :] = imgResize
            except:
                print("hand out of range!")
                pass

        # cv2.imshow("ImageCrop", imgCrop)
        imgWhite = cv2.cvtColor(imgWhite, cv2.COLOR_BGR2GRAY)
        cv2.imshow("ImageWhite", imgWhite)

        # get list of landmarks, add to data object
        lmList = hand["lmList"]

        lmListNormalized = []
        # IMG_WIDTH = x, IMG_HEIGHT = y
        
        for coord in lmList:
            coordNorm = [coord[0] / IMG_WIDTH, coord[1] / IMG_HEIGHT, coord[2]]
            lmListNormalized.append(coordNorm)
            # print(f"z is {coord[2]} in relation to x of {coordNorm[0]}")
        
        lmListNormalized = np.array(lmListNormalized, dtype=np.float32) # --> (21, 3) for color image

        string_pred = None
        if not TRAINING and MODEL_ACTIVE:
            prediction = model(np.expand_dims(lmListNormalized, axis=0))
            print("PREDICTON: ", labels[np.argmax(prediction)])
            string_pred = labels[np.argmax(prediction)]
        for lm in lmList:
            data.extend([lm[0], IMG_HEIGHT - lm[1], lm[2]])
        center = list(hand["center"])
        # print(f"center rel: {center[0] / IMG_WIDTH}, {center[1] / IMG_HEIGHT}")
        data.extend([f"{center[0]} {center[1]}", f"{string_pred}"])
        # send with UDP all in 1 port
        sock.sendto(str.encode(str(data)), serverAddressPort)
    
    cv2.imshow('Image', img)
    key = cv2.waitKey(1)
    
    # BLOCK TO SEND MODEL POINTS TO NP ARRAY
    if (key == ord("s") or key == 32) and TRAINING:
        counter += 1
        
        lmListNormalized = np.array(lmListNormalized, dtype=np.float32) 

        np.save(os.path.join(folder, TO_TRAIN, f"{TO_TRAIN}_{counter}"), lmListNormalized)
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