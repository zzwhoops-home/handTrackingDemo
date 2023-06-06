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
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        // constantly update received data from local server (IDK HOW TO DO THIS)
        string data = udpRecieve.data;
        var jsonObject = JsonUtility.FromJson<String>(data);

        if(!string.IsNullOrEmpty(data) || data.Length != 0){

            string[] points = data.Split(",");
            // print(points);

            /*
                ok ideas i will get to: make the hand always the same size by using the size of the bounding box because as it gets closer to the camera it gets bigger
                so send the entire dict of info to the server to process
                find the center of the hand
                make some magic shooty game
            */
            for(int i=0; i<21; ++i){
                
                float x = -1 * (3 - float.Parse(points[i*3]) / 100);
                float y = float.Parse(points[i*3+1]) / 100;
                float z = float.Parse(points[i*3+2]) / 100;
                
                handPoints[i].transform.localPosition = new Vector3(x, y, z);
                // print(handPoints[0].transform.localPosition.z);
            }
        }
        
    }
}
