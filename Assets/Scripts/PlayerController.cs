using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Misc....")]
    public Vector3 HoVeInput = new Vector3(0,0,0);
    public LayerMask collisionMask = 0;
    [Space(0)]
    /// <summary>
    /// WORLD VARS
    /// </summary>
    [Header("World controls")]
    public float grav = 0.75f;          //gravity
    [Range(0,50)]
    public float drag = 20;             //doesnt effect verticle drag
    [Space(0)]

    /// <summary>
    /// MOVE VARS
    /// </summary>
    [Header("Movement Controls")]
    [Range(0,10)]
    public float baseSpeed = 1;         //what speed will return to after sprinting
    [Range(1,5)]
    public float sprintSpeed = 1.5f;    //what the new speed will become
    [Range(0,10)]
    public float jumpSpeed = 2;         //jump height
    [Space(0)]

    /// <summary>
    /// CAMERA VARS
    /// </summary>

    [Header("Camera Controls")]
    public CameraController camera;
    public Vector3 cameraOffset = new Vector3(0,0,0);
    [Range(0.001f,1)]
    public float cameraBgR = 10;       //this is the bigger radius that we move the "curso" on
    [Range(0.1f, 2)]
    public float cameraSmllR = 1;
    [Range(1,30)]
    public float cameraSmooth = 2;      //bigger is smoother
     
    public float camHSpeed = 0;
    public float camVSpeed = 0;
    private bool mouseLocked = true;
    [Space(0)]

    [Header("Conditional statements")]
    public bool isJump = false;
    public bool reset = false;          //respawn button
    public bool isSprint = false;
    


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<CameraController>();
    }


    // Update is called once per frame
    public void FixedUpdate()
    {
        
        if      (Input.GetKey(KeyCode.D))       HoVeInput[0] = 1;
        else if (Input.GetKey(KeyCode.A))       HoVeInput[0] = -1;
        else                                    HoVeInput[0] = 0;

        if      (Input.GetKey(KeyCode.W))       HoVeInput[2] = 1;
        else if (Input.GetKey(KeyCode.S))       HoVeInput[2] = -1;
        else                                    HoVeInput[2] = 0;

        if (Input.GetKey(KeyCode.LeftShift))    isSprint = true;
        else                                    isSprint = false;

        if (Input.GetKey(KeyCode.Space))        isJump = true;
        else                                    isJump = false;
        
        if (Input.GetKey(KeyCode.R))            reset = true;
        else                                    reset = false;

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (!mouseLocked)                   mouseLocked = true;
            else                                mouseLocked = false;
        } 

        if (mouseLocked) {
            //Debug.Log("H: " + Input.GetAxis("Mouse X") + "\tV: " + Input.GetAxis("Mouse Y"));
            camHSpeed = Input.GetAxis("Mouse X") * Time.deltaTime;
            camVSpeed = -Input.GetAxis("Mouse Y") * Time.deltaTime;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            camHSpeed = camVSpeed = 0;
        }

        //I know I could reduce this down to one line, but it would look very ugly, because I would have to 
        HoVeInput = Vector3.Normalize(new Vector3(  HoVeInput[0] * Mathf.Cos(-camera.theta) - HoVeInput[2] * Mathf.Sin(-camera.theta), 
                                                    0, 
                                                    HoVeInput[0] * Mathf.Sin(-camera.theta) + HoVeInput[2] * Mathf.Cos(-camera.theta)));

        
    }

}
