import time
from keras.utils import to_categorical
from sklearn.model_selection import train_test_split
from glob import glob
import os
import tqdm
import numpy as np
counter = 0

label_map = {'at_screen':0, 'closed_fist':1, 'neutral':2, 'ok':3, 'peace':4, 'pointer_left':5, 'pointer_right':6, 'thumbs_up':7}
pose_class_names = ['at_screen', 'closed_fist', 'neutral', 'ok', 'peace', 'pointer_left', 'pointer_right', 'thumbs_up']
print(label_map)

sequences, labels = [], []

for pose_class_name in pose_class_names:
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
from keras.layers import LSTM, Dense, InputLayer, Dropout, Reshape

model2 = Sequential([
    InputLayer(input_shape=(21, 3)),
    Dense(64, activation='relu'),
    Dense(32, activation='relu'),
    Dense(5, activation='relu'),
    Reshape((105,)),
    Dense(len(label_map), activation='softmax')
])

model2.compile(optimizer="Adam", loss='categorical_crossentropy', metrics=['categorical_accuracy'])
model2.summary()

model2.fit(X_train, Y_train, epochs=10000, verbose=True)

result = model2.predict(X_test)
print("Predicted pose:", pose_class_names[np.argmax(result[1])])
print("Actual pose:", pose_class_names[np.argmax(Y_test[1])])

print(np.array(X_test).shape)
print(np.array(Y_test).shape)

model2.save('./models/EightMovements_500steps.h5')