import cv2
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector
import socket
import json

cap = cv2.VideoCapture(0) # uncomment if you have webcam
# cap = cv2.VideoCapture("http://10.0.0.115:4747/video") # ONLY OPENS UP WHEN ONE INSTANCE OF THIS IS OPEN! DO NOT OPEN IN BROWSER!
IMG_HEIGHT = 720
IMG_WIDTH = 1280 
cap.set(3, IMG_WIDTH)
cap.set(4, IMG_HEIGHT)
if not cap.isOpened():
    raise IOError("Cannot open webcam")
    
# DRAW BOTH HANDS ON SCREEN
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Comms
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

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
    # if hands:
    #     hand = hands[0]
    #     # get list of landmarks
    #     lmList = hand['lmList']

    #     for lm in lmList:
    #         data.extend([lm[0], IMG_HEIGHT - lm[1], lm[2]])
    #     sock.sendto(str.encode(str(data)), serverAddressPort)
    
    cv2.imshow('Image', img)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

print(hand['lmList'])
print(hand)

cap.release()
cv2.destroyAllWindows()