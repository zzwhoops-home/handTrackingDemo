using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpRecieve;
    public GameObject[] handPoints;
    public float scaleFactor = 200f;
    private float[] centerFloats;
    private string prediction;
    private string previousData = "";

    private Queue<String> recentPredictions;
    private int maxRecent = 20;
    // higher threshold = LESS sensitive predictions. MUST BE LESS THAN OR EQUAL TO maxRecent! This is in a percentage. It is automatically clamped.
    
    [Range(0.5f, 1.0f)]
    public float thresholdRange;
    private int threshold;

    void Start(){
        centerFloats = new float[] {Screen.width / 2, Screen.height / 2};
        recentPredictions = new Queue<String>();

        threshold = (int) Mathf.Round(maxRecent * thresholdRange);
        print(threshold);
        StartCoroutine(Prediction());
    }

    // Update is called once per frame
    void Update(){
        // constantly update received data from local server
        string data = udpRecieve.data;

        if((!string.IsNullOrEmpty(data) || data.Length != 0) && previousData != data){
            // remove first and last character, which are "[" and "]" respectively
            data = data.Trim('[', ']');

            string[] points = data.Split(",");
            
            int len = points.Length;
            string[] centerStrings = points[len - 2].Trim(' ', '\'').Split(" ");
            prediction = points[len - 1];
            prediction = prediction.Trim(' ', '\'');

            UpdateRecentPoses(prediction);

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
            previousData = data;
        }
    }

    private void UpdateRecentPoses(string prediction)
    {
        if (!string.IsNullOrEmpty(prediction)) {
            recentPredictions.Enqueue(prediction);
            if (recentPredictions.Count > maxRecent) {
                recentPredictions.Dequeue();
            }
        }
    }
    private IEnumerator Prediction() {
        yield return null;
    }

    public float[] GetCenterPoints() {
        return centerFloats;
    }
    public string GetPosePrediction() {
        if (recentPredictions.Count < maxRecent) {
            return "neutral";
        }

        // count # of each in 10 most recent poses
        Dictionary<string, int> poseCounts = new Dictionary<string, int>();
        foreach (string pred in recentPredictions) {
            if (poseCounts.ContainsKey(pred)) {
                poseCounts[pred] += 1;
            } else {
                poseCounts[pred] = 1;
            }
        }

        // get pose that appeared most in 10 most recent poses
        string posePrediction = null;
        int maxCount = 0;
        foreach (KeyValuePair<string, int> pair in poseCounts)
        {
            if (pair.Value > maxCount) {
                posePrediction = pair.Key;
                maxCount = pair.Value;
            }
        }
        // print(String.Format("{0} {1}", maxCount, posePrediction));
        string queueContents = string.Join(", ", recentPredictions.ToArray());
        print(queueContents);
        if (maxCount > threshold) {
            return posePrediction;
        } else {
            return "neutral";
        }
    }
}
