import time
from keras.utils import to_categorical
from keras.models import load_model
from sklearn.model_selection import train_test_split
from glob import glob
import os
import tqdm
import numpy as np
from keras.callbacks import TensorBoard


label_map = {'at_screen':0, 'neutral':1, 'peace':2, 'pointer_left':3, 'pointer_right':4}
label = ['at_screen', 'neutral', 'peace', 'pointer_left', 'pointer_right']
print(label_map)

sequences, labels = [], []

for pose_class_name in label:
    keypoint_names = glob(os.path.join("train_data_points", pose_class_name, "*.npy"))
    
    print("searching through {}".format(pose_class_name))
    for keypoint_name in tqdm.tqdm(keypoint_names):
        file = np.load(keypoint_name)
        sequences.append(file)
        labels.append(label_map[pose_class_name])

print(len(sequences))
print(len(labels))
print()
        
print(np.array(sequences).shape)
print(np.array(labels).shape)
print()

X = np.array(sequences)
y = to_categorical(labels).astype(int)

X_train, X_test, Y_train, Y_test = train_test_split(X, y, test_size=0.05)

print(f"Xtrain:{X_train.shape}")
print(f"Ytrain:{Y_train.shape}")
print()

from keras.models import Sequential
from keras.layers import LSTM, Dense, InputLayer, Dropout, Reshape, BatchNormalization

# ===== HYPERPARAMETERS ===== #

IMG_SIZE = 50
PREV_EPOCHS = 0
LOAD_PREV = True if PREV_EPOCHS > 0 else False

EPOCHS = 100
if LOAD_PREV:
    model = load_model(f"models\FiveMovements_{PREV_EPOCHS}_steps.h5")

# ===== HYPERPARAMETERS ===== #

model = Sequential([
    InputLayer(input_shape=(IMG_SIZE, IMG_SIZE)),
    Reshape((IMG_SIZE * IMG_SIZE,)),
    Dense(64, activation='relu'),
    Dropout(0.2),
    BatchNormalization(),
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

model.compile(optimizer="Adam", loss='categorical_crossentropy', metrics=['categorical_accuracy'])
model.summary()


tensorboard_callback = TensorBoard(log_dir='./logs')

model.fit(X_train, Y_train, epochs=EPOCHS, verbose=True, callbacks=[tensorboard_callback])

result = model.predict(X_test)
print("Predicted pose:", label[np.argmax(result[1])])
print("Actual pose:", label[np.argmax(Y_test[1])])

print(np.array(X_test).shape)
print(np.array(Y_test).shape)

model.save(f'./models/FiveMovements_{PREV_EPOCHS+EPOCHS}_steps.h5')