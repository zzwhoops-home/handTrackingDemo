using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHandRecog : MonoBehaviour{
    void Start(){
        var pythonArgs = "../../hand_pose_opencv.py";
        System.Diagnostics.Process.Start("python", pythonArgs);
    }
}
