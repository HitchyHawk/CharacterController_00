using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public  Vector3 HoVeInput    = new Vector3(0,0,0);
    private Vector3 preHoVeInput = new Vector3(0, 0, 0); //quality of life addition.
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
    [Range(0,4)]
    public float baseSpeed = 1;         //what speed will return to after sprinting
    [Range(1,5)]
    public float sprintSpeed = 1.5f;    //what the new speed will become
    [Range(0,30)]
    public float jumpSpeed = 2;         //jump height

    private float lockedTheta;
    [Space(0)]

    /// <summary>
    /// CAMERA VARS
    /// </summary>

    [Header("Camera Controls")]
    public GameObject camera;
    public CameraPlayerCue cameraPlayerCue;
    public Vector3 cameraOffset = new Vector3(0,0,0);
    [Range(0,20)]
    public float cameraSensitivity = 10;       //this is the bigger radius that we move the "curso" on
    [Range(0.1f, 10)]
    public float cameraRadius = 1;
    [Range(1,100)]
    public float cameraSmooth = 2;      //bigger is smoother

    [HideInInspector]
    public Vector2 camSpeed = Vector2.zero;
    private bool mouseLocked = true;
    [HideInInspector]
    public Vector3 targetPos = Vector3.zero;
    [HideInInspector]
    public Vector3 targetAngleTP = Vector3.zero;
    [Space(0)]

    [HideInInspector]
    public bool isJump = false;
    [HideInInspector]
    public bool reset = false;
    [HideInInspector]
    public bool isSprint = false;

    [HideInInspector]
    public float theta = 0;
    [HideInInspector]
    public float phi = 0;

    public bool inCue = false;
    public CameraCue cue;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //camera          = GetComponent<CameraController>();
        cameraPlayerCue = GetComponent<CameraPlayerCue>();
    }


    // Update is called once per frame
    public void Update()
    {
        
        //Key presses
        {
            if      (Input.GetKey(KeyCode.D))   HoVeInput[0] = 1;
            else if (Input.GetKey(KeyCode.A))   HoVeInput[0] = -1;
            else                                HoVeInput[0] = 0;

            if      (Input.GetKey(KeyCode.W))   HoVeInput[2] = 1;
            else if (Input.GetKey(KeyCode.S))   HoVeInput[2] = -1;
            else                                HoVeInput[2] = 0;

            if      (Input.GetKey(KeyCode.LeftShift))   isSprint = true;
            else                                        isSprint = false;

            if      (Input.GetKey(KeyCode.Space))       isJump = true;
            else                                        isJump = false;

            if      (Input.GetKey(KeyCode.R))           reset = true;
            else                                        reset = false;

            if      (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!mouseLocked)   mouseLocked = true;
                else                mouseLocked = false;
            }
        }

        //camera rotation stuff
        {
            if (mouseLocked){     
                camSpeed.x = Input.GetAxis("Mouse X") * Time.deltaTime;
                camSpeed.y = -Input.GetAxis("Mouse Y") * Time.deltaTime;
            }
            else{
                Cursor.lockState = CursorLockMode.None;
                camSpeed = Vector2.zero;
            }

            //to minimize floating point errors
            if (Mathf.Abs(theta) > 2 * Mathf.PI && Mathf.Abs(cameraPlayerCue.theta) > 2 * Mathf.PI){
                theta                 -= 2 * Mathf.PI * theta                 / Mathf.Abs(theta                );  
                cameraPlayerCue.theta -= 2 * Mathf.PI * cameraPlayerCue.theta / Mathf.Abs(cameraPlayerCue.theta);
            }

            if (inCue)
            {
                targetPos = cue.position;
                targetAngleTP.y = cue.theta;
                targetAngleTP.x = cue.phi;
            }
            else {
                targetPos = cameraPlayerCue.position + transform.position;
                targetAngleTP.y = cameraPlayerCue.theta;
                targetAngleTP.x = cameraPlayerCue.phi;
            }
            
            //the smoothing bit that finallizes it.
            camera.transform.position   = (targetPos - camera.transform.position) / cameraSmooth + camera.transform.position;
            theta                       = (targetAngleTP.y - theta) / cameraSmooth + theta;
            phi                         = (targetAngleTP.x - phi) / cameraSmooth + phi;


            camera.transform.eulerAngles = new Vector3(phi, theta, 0) * Mathf.Rad2Deg;

            HoVeInput = Vector3.Normalize(new Vector3(HoVeInput[0] * Mathf.Cos(-theta) - HoVeInput[2] * Mathf.Sin(-theta),
                                                      0,
                                                      HoVeInput[0] * Mathf.Sin(-theta) + HoVeInput[2] * Mathf.Cos(-theta)));
        }
    }

    void OnTriggerStay()
    {
        Debug.Log("IN");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER");
    }

}
