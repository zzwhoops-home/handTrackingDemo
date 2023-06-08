import cv2
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector
from cvzone.ClassificationModule import Classifier
import socket
import json
import numpy as np
import math
import time


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
offset = 20
imgSize = 300

# Comms
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

TO_TRAIN = "hello"
folder = f"./train_data/{TO_TRAIN}"
counter = 0
labels = ["hello", "thumbs_up"]

while(True):
    success, img = cap.read()
    # flipping image to make everything easier to control (like a mirror)

    img = cv2.flip(img, 1)
    hands, img = detector.findHands(img, flipType=False)

    data = []
    
    if hands:
        hand = hands[0]

        # get list of landmarks, add to data object
        lmList = hand["lmList"]
        for lm in lmList:
            data.extend([lm[0], IMG_HEIGHT - lm[1], lm[2]])
        sock.sendto(str.encode(str(data)), serverAddressPort)
    
    cv2.imshow('Image', img)
    key = cv2.waitKey(1)
    if key & 0xFF == ord('q'):
        break


cap.release()
cv2.destroyAllWindows()