using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpRecieve;
    public GameObject[] handPoints;
    public GameObject center;
    public float scaleFactor = 200f;
    private float[] centerFloats;
    private String prediction;
    void Start(){
        centerFloats = new float[] {Screen.width / 2, Screen.height / 2};
    }

    // Update is called once per frame
    void Update(){
        // constantly update received data from local server
        string data = udpRecieve.data;

        if(!string.IsNullOrEmpty(data) || data.Length != 0){
            // remove first and last character, which are "[" and "]" respectively
            data = data.Trim('[', ']');

            string[] points = data.Split(",");
            
            int len = points.Length;
            string[] centerStrings = points[len - 2].Trim(' ', '\'').Split(" ");
            prediction = points[len - 1];

            // IMPORTANT: CENTER INFORMATION IS SENT SECOND TO LAST ITEM IN ARRAY, LAST ITEM ARRAY IS STRING PREDICTION
            centerFloats = new float[] {float.Parse(centerStrings[0]), float.Parse(centerStrings[1])};

            /*
                ok ideas i will get to: make the hand always the same size by using the size of the bounding box because as it gets closer to the camera it gets bigger
                make some magic shooty game
            */
            for(int i=0; i<21; ++i){
                
                float x = -1 * (3 - float.Parse(points[i*3]) / scaleFactor);
                float y = float.Parse(points[i*3+1]) / scaleFactor;
                float z = float.Parse(points[i*3+2]) / scaleFactor;
                
                handPoints[i].transform.localPosition = new Vector3(x, y, z);
                // print(handPoints[0].transform.localPosition.z);
            }
        }
    }
    public float[] GetCenterPoints() {
        return centerFloats;
    }
    public String GetPosePrediction() {
        return prediction;
    }
}
