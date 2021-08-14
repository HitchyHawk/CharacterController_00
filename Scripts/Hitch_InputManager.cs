using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hitch_InputManager : MonoBehaviour
{

    public Vector2 inLeftH = new Vector2(0,0);
    public Vector2 inRightH = new Vector2(0,0);
    public bool jump = false;
    public bool restart = false;
    public bool sprinting = false;
    public bool isCursor = false;

    [Header("controller type: 0 - kBoard, 1 - ps4")]
    [Range(0,2)] public int controllerType = 0;
    float time2Start = 0.5f;
    
    //[Range(0, 5)] public float mouseClamp = 0.5f;
    // Update is called once per frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (time2Start > 0){
            time2Start -= Time.deltaTime;
            return;
        }

        switch (controllerType) {
            ///==========================
            ///Keyboard and mouse
            ///==========================
            case 0:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    
                    isCursor = !isCursor;
                    if (isCursor) Cursor.lockState = CursorLockMode.None;
                    else Cursor.lockState = CursorLockMode.Locked;
                }
                

                //anti control jamming, where most recent input gets prioritized.
                if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                {
                    if (Input.GetKeyDown(KeyCode.A)) inLeftH.x = -1;
                    else if (Input.GetKeyDown(KeyCode.D)) inLeftH.x = 1;
                }
                else
                {
                    inLeftH.x = 0;
                    if (Input.GetKey(KeyCode.A)) inLeftH.x = -1;
                    if (Input.GetKey(KeyCode.D)) inLeftH.x = 1;
                }

                if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKeyDown(KeyCode.S)) inLeftH.y = -1;
                    else if (Input.GetKeyDown(KeyCode.W)) inLeftH.y = 1;
                }
                else
                {
                    inLeftH.y = 0;
                    if (Input.GetKey(KeyCode.S)) inLeftH.y = -1;
                    if (Input.GetKey(KeyCode.W)) inLeftH.y = 1;
                }

                if (isCursor){
                    inRightH.x = 0;
                    inRightH.y = 0;
                }
                else {
                    inRightH.x = Input.GetAxis("Mouse X");
                    inRightH.y = -Input.GetAxis("Mouse Y");
                }
                

                if (Input.GetKey(KeyCode.R)) restart = true;
                else restart = false;

                if (Input.GetKey(KeyCode.Space)) jump = true;
                else jump = false;

                //sprinting = Input.GetKeyDown(KeyCode.LeftControl);

                if (Input.GetKey(KeyCode.LeftControl)) sprinting = true;
                else sprinting = false;

                break;
            ///==========================
            ///PS4 ANALOG CONTROLLER
            ///==========================
            case 1:
                Debug.Log("Ps4 controller");
                break;

            default:
                Debug.LogWarning("invalid controller setup");
                break;
        }
        

    }

    
}
