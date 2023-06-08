using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFromMouse : MonoBehaviour
{
    public Camera cam;
    private Ray mouseRay;
    private Vector3 direction;
    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        mouseRay = cam.ScreenPointToRay(mousePos);
        LookMouseDirection(gameObject, mouseRay.direction);
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
