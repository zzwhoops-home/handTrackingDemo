using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFromHand : MonoBehaviour
{
    public Camera cam;
    public Ray handRay;
    private Vector3 direction;
    private Quaternion rotation;
    public Transform crosshair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var handPos = crosshair.position;
        handRay = cam.ScreenPointToRay(handPos);
        LookMouseDirection(gameObject, handRay.direction);
    }
    
    private void LookMouseDirection(GameObject obj, Vector3 destination) {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = rotation;
    }

    public Quaternion GetRotation() {
        return rotation;
    }
}
