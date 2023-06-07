using System.Collections;
using System.Collections.Generic;
using IGameControlsActions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IGameControlsActions.GameControls.ITurnActions
{    
    GameControls controls;
    public Camera cam;

    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new GameControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            controls.Turn.SetCallbacks(this);
        }
        controls.Turn.Enable();
    }

    public void OnDisable()
    {
        controls.Turn.Disable();
    }    
    
    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.started) {
            // 'Use' code here.
            float turn = context.ReadValue<float>();
            if (turn == -1.0f) {
                // rotate 90 deg left
                cam.transform.Rotate(Vector3.up, -90f, Space.World);
            } else if (turn == 1.0f) {
                // rotate 90 deg right
                cam.transform.Rotate(Vector3.up, 90f, Space.World);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
