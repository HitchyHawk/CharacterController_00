using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialMaths;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector3 HoVeInput          = new Vector3(0, 0, 0);
                      public LayerMask collisionMask    = 0;
    /// <summary>
    /// WORLD VARS
    /// </summary>
    [Header("World controls")]
    [Range(0,  2)]  public float grav = 0.75f;  //gravity
    [Range(0,100)]  public float drag = 20;     //only effects drag on the horizontal, not verticle

    [Space(0)]

    /// <summary>
    /// MOVE VARS
    /// </summary>
    [Header("Movement Controls")]
    [Range(0, 4)] public float baseSpeed    = 1;        //what speed will return to after sprinting
    [Range(1, 5)] public float sprintSpeed  = 1.5f;     //what the basespeed is multiplied by
    [Range(0,50)] public float jumpSpeed    = 2;        //jump speed
    [Range(0, 3)] public float maxSpeed     = 5;        //The maximum speed the player can have
    [Space(0)]
    [HideInInspector] public bool isJump = false;       //did the jump button get pushed
    [HideInInspector] public bool reset = false;        //did the respawn button get pushed
    [HideInInspector] public bool isSprint = false;     //did the sprint button get pushed

    /// <summary>
    /// CAMERA VARS
    /// </summary>

    [Header("Camera Controls")]
    public GameObject camera;                                       //The camera we control, we tell it where to go and what rotation it should have
    public CameraPlayerCue cameraPlayerCue;                         //the camera cue that we defualt to. tells camera where to go
    public Vector3 cameraOffset = new Vector3(0,0,0);               //the offset of rotation, still need to work on this so it rotates too.

    [Range(0   , 20)] public float cameraSensitivity    = 10;       //this is the bigger radius that we move the "curso" on
    [Range(0.1f, 10)] public float cameraRadius         = 1;        //How far away the camera is from the offset it rotates around.
    [Range(1   ,100)] public float cameraSmoothBase     = 30;       //bigger is smoother
                      private float cameraSmooth;

    [HideInInspector] public Vector2 camSpeed       = Vector2.zero; //For the CameraPlayer Cue
                      private bool mouseLocked      = true;         //Is the mouse locked in the center of the screeen?
    [HideInInspector] public Vector3 targetPos      = Vector3.zero; //The target pos that the camera pos is trying to get to
    [HideInInspector] public Vector3 targetAngleTP  = Vector3.zero; //The target angle that theta is trying to get to

    [HideInInspector] AngleSpace theta;
    [HideInInspector] AngleSpace phi;                      


    private Vector3 preHoVeInput;                 //used to the previous movement option, not dependent on camera rotation, for transition
                      private float   preTheta;                     //the last theta before changing camera cue,
                      public bool     inTransition    = false;      //did you enter or exit a camera cue?
                      private bool    inCue           = false;      //are you in a cameraCue zone
    [HideInInspector] public bool     allowedToMove   = true;       //allowed to move, part of a movement option in camera cue, USED IN KEYS
                      private CameraCue cue;                        //the cue object.


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraPlayerCue = GetComponent<CameraPlayerCue>();
        theta = new AngleSpace(0, Mathf.PI * 2, 0,true);
        phi = new AngleSpace(0, Mathf.PI * 0.5f, -Mathf.PI * 0.5f,false);
    }


    // Update is called once per frame
    public void Update()
    {
        //What keys got pushed?
        {
            if (cue == null) {
                allowedToMove   = true;
                inCue           = false;
                inTransition    = false;
            }

            if (allowedToMove)
            {
                if (Input.GetKey(KeyCode.D))        HoVeInput[0] = 1;
                else if (Input.GetKey(KeyCode.A))   HoVeInput[0] = -1;
                else                                HoVeInput[0] = 0;

                if (Input.GetKey(KeyCode.W))        HoVeInput[2] = 1;
                else if (Input.GetKey(KeyCode.S))   HoVeInput[2] = -1;
                else                                HoVeInput[2] = 0;

                if (Input.GetKey(KeyCode.Space))    isJump = true;
                else                                isJump = false;
            }
            else {
                HoVeInput *= 0;
            }

            if      (Input.GetKey(KeyCode.LeftShift))   isSprint = true;
            else                                        isSprint = false;

            if      (Input.GetKey(KeyCode.R))           reset = true;
            else                                        reset = false;

            if      (Input.GetKeyDown(KeyCode.P))
            {
                if (!mouseLocked)   mouseLocked = true;
                else                mouseLocked = false;
            }
        }

        //camera rotation stuff
        {
            if (mouseLocked) {
                camSpeed.x = Input.GetAxis("Mouse X") * Time.deltaTime;
                camSpeed.y = -Input.GetAxis("Mouse Y") * Time.deltaTime;
            } 
            else {
                Cursor.lockState = CursorLockMode.None;
                camSpeed = Vector2.zero;
            }

            //dealing with cues
            if (inCue)
            {
                if (!cue.activate && cue.makeGlobal) removeCue();

                targetPos = cue.position;
                //need to give everything the same cue angle
                targetAngleTP.y = cue.GetTheta();
                targetAngleTP.x = -cue.GetPhi();

                cameraPlayerCue.SetTheta(targetAngleTP.y);
                cameraPlayerCue.SetPhi  (targetAngleTP.x);

                cameraSmooth = cue.CueSmoothness;
            }
            else {
                targetPos       = cameraPlayerCue.position + transform.position;
                targetAngleTP   = cameraPlayerCue.rotation;
                cameraSmooth    = (cameraSmoothBase - cameraSmooth) *0.01f + cameraSmooth;
            }

            //the smoothing bit that finallizes it.
            camera.transform.position   = (targetPos - camera.transform.position)   / cameraSmooth + camera.transform.position;
            theta.StepLerp(targetAngleTP.y, cameraSmooth);
            phi.  StepLerp(targetAngleTP.x, cameraSmooth);
            camera.transform.eulerAngles = new Vector3(phi.GetDeg(), theta.GetDeg(), 0);
        }

        //Movement
        if (allowedToMove)
        {
            if (inTransition) {
                //for locked movement when switching cues.
                if (preHoVeInput == HoVeInput) {
                    HoVeInput = Vector3.Normalize(new Vector3(HoVeInput[0] * Mathf.Cos(-preTheta) - HoVeInput[2] * Mathf.Sin(-preTheta),
                                                              0,
                                                              HoVeInput[0] * Mathf.Sin(-preTheta) + HoVeInput[2] * Mathf.Cos(-preTheta)));
                }
                else inTransition = false;
            }
            else {
                preTheta = theta.GetFloat();
                preHoVeInput = HoVeInput;
                HoVeInput = Vector3.Normalize(new Vector3(HoVeInput[0] * Mathf.Cos(-theta.GetFloat()) - HoVeInput[2] * Mathf.Sin(-theta.GetFloat()),
                                                          0,
                                                          HoVeInput[0] * Mathf.Sin(-theta.GetFloat()) + HoVeInput[2] * Mathf.Cos(-theta.GetFloat())));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        inCue = true;
        cue = other.GetComponent<CameraCue>();

        //activates the transition movement locking
        if (cue.lockMovement)   inTransition = true;
        else                    inTransition = false;
        
        //activates the camera movement
        cue.activate = true;
    }
    void OnTriggerStay(Collider other)
    {
        allowedToMove = cue.allowedToMove;
    }
    void OnTriggerExit(Collider other)
    {
        if (!cue.makeGlobal) {
            removeCue();
            inCue = false;
        }
    }
    void removeCue() {
        inCue = false;
        cue.currentTime = 0;
        if (cue.lockMovement) inTransition = true;
        else inTransition = false;
        allowedToMove = true;
        cue = null;
    }

}
