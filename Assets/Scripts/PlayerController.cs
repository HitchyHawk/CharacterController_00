using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 HoVeInput = new Vector3(0,0,0);
    private Vector3 tempHV;
    public LayerMask collisionMask = 0;

    /// <summary>
    /// WORLD VARS
    /// </summary>
    public float grav = 0.75f;
    public float drag = 20;

    /// <summary>
    /// MOVE VARS
    /// </summary>
    public float baseSpeed = 1;
    public float sprintSpeed = 1.5f;
    public float jumpSpeed = 2;

    public bool isJump = false;
    public bool reset = false;
    public bool isSprint = false;

    /// <summary>
    /// CAMERA VARS
    /// </summary>
    public bool mouseLocked = true;
    public float sensitivity = 1;
    public float cameraXRot = 30;
    public float cameraSmooth = 2;
    public float baseRadius = 6;
    public float baseHeight = 1;
    public float theta = 0;
    public float h = 0;
    float temp;
    public float heightLimit = 3;
    public float cameraSize = 0.5f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    public void FixedUpdate()
    {
        
        if      (Input.GetKey(KeyCode.D))   HoVeInput[0] = 1;
        else if (Input.GetKey(KeyCode.A))   HoVeInput[0] = -1;
        else                                HoVeInput[0] = 0;

        if      (Input.GetKey(KeyCode.W))   HoVeInput[2] = 1;
        else if (Input.GetKey(KeyCode.S))   HoVeInput[2] = -1;
        else                                HoVeInput[2] = 0;

        if (Input.GetKey(KeyCode.LeftShift)) isSprint = true;
        else                                 isSprint = false;

        if (Input.GetKey(KeyCode.Space))    isJump = true;
        else                                isJump = false;
        
        if (Input.GetKey(KeyCode.R))        reset = true;
        else                                reset = false;

        if (Input.GetKey(KeyCode.Escape)) mouseLocked = false;

        if (mouseLocked) {
            //Debug.Log("H: " + Input.GetAxis("Mouse X") + "\tV: " + Input.GetAxis("Mouse Y"));
            theta += Input.GetAxis("Mouse X") * sensitivity*Time.deltaTime;
            if (Mathf.Abs(theta) > 2 * Mathf.PI) theta -= 2 * Mathf.PI * theta / Mathf.Abs(theta);

            temp = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime*0.5F;
            if (h * temp > 0)
            {
                if (h > 0)
                {
                    h += (Mathf.Cos((h) * Mathf.PI / baseHeight) + 1) * temp;
                    
                }
                else {
                    h += (Mathf.Cos((h) * Mathf.PI / heightLimit) + 1) * temp;
                }
                
            }
            else h += temp;
            if (h < -heightLimit) h = -heightLimit;
            if (h > baseHeight) h = baseHeight;


        }
        else {
            Cursor.lockState = CursorLockMode.None;
        }
        tempHV = Vector3.Normalize(HoVeInput);

        HoVeInput[0] =  tempHV[0]*Mathf.Cos(-theta) - tempHV[2]* Mathf.Sin(-theta);
        HoVeInput[2] =  tempHV[0]*Mathf.Sin(-theta) + tempHV[2]* Mathf.Cos(-theta);
    }

}
