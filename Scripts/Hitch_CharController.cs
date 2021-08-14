using System;
using UnityEngine;
using Hitch_EasyMath;



[RequireComponent(typeof(Hitch_InputManager))]
[RequireComponent(typeof(Hitch_CharMovement))]
[RequireComponent(typeof(Hitch_CharAnimation))]
public class Hitch_CharController : MonoBehaviour
{
    public Hitch_CharacterVariables vars;
    public CameraSettings cameraSettings;
    
    [HideInInspector] new public Camera camera;
    [HideInInspector] public GameObject player;
    public Transform cameraLookAt, cameraPivot;
    [HideInInspector] public Animator playerAnimator;
    

    [HideInInspector] public Hitch_InputManager controls;
    [HideInInspector] public Hitch_CharMovement charMovement;
    [HideInInspector] public Hitch_CharAnimation charAnimation;
    [HideInInspector] Hitch_CameraPlayer cameraActorDefault;


    [HideInInspector] public Vector3 direction, resetPos;
    [HideInInspector] public float deltaTime;

    public bool startJump = false, sprintCondition = false, jumpCondition = false;
    private float sprintTimer = 0;
    private float t = 0;

    public bool drawDebug = false;

    void Start()
    {
        Hitch_TC_Finder finder = GetComponentInParent<Hitch_TC_Finder>();
        Hitch_TCompactor compactor = finder.compactor;

        camera          = Camera.main;
        cameraLookAt    = compactor.GetTransform("CameraLook");
        cameraPivot     = compactor.GetTransform("CameraPivot");

        cameraActorDefault = new Hitch_CameraPlayer();

        cameraSetup();
        animationSetup();

        controls = GetComponent<Hitch_InputManager>();
        charMovement = GetComponent<Hitch_CharMovement>();
        
        resetPos = transform.position;
    }
    private void animationSetup()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = GetComponentInChildren<Animator>();
        charAnimation = GetComponent<Hitch_CharAnimation>();

        charAnimation.setPlayer(player);
        charAnimation.setAnimator(playerAnimator);
    }
    private void cameraSetup()
    {
        
        cameraActorDefault.setCamera(camera);
        cameraActorDefault.setLookAt(cameraLookAt);
        cameraActorDefault.setPivot(cameraPivot);
        cameraActorDefault.setCameraSettings(cameraSettings);
    }

    void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        t = 1 - Mathf.Pow(0.001f, deltaTime);
        //Controller and movement
        UpdateState();

        try
        {
            charMovement.UpdateMovement();
        }
        catch (Exception){
            Debug.LogWarning("something from Update Movement");
        }
        
        try
        {
            charAnimation.UpdateState();
            charAnimation.UpdateAnimation();
        }
        catch (Exception e) {
            Debug.LogWarning("Animation error, probably had to divide by zero: " + e.Message);
        }

        //camera based stuff
        try{
            Vector3 playerOffset = new Vector3(charMovement.getVelocity().x, (charMovement.isInAir()? 0.5f : 1) * charMovement.getVelocity().y, charMovement.getVelocity().z);
            cameraActorDefault.refresh(controls.inRightH);
        }
        catch (Exception) {
            Debug.LogWarning("something from Camera");
        }
    }
     

    void UpdateState()
    {
        if (controls.restart) { transform.position = resetPos; }

        direction = cameraActorDefault.transformToView(controls.inLeftH, Vector3.up);
        direction = direction.normalized;

        //once startJump is on, it'll stay on. Gets turned off from animation.
        if (charMovement.isGrounded()) {
            if (controls.jump){
                charMovement.Jump();
            }
        }

        //stays on till you stop
        if (!sprintCondition) sprintCondition = controls.sprinting;
        else sprintTimer += deltaTime;

        if (sprintTimer > 1.1f) {
            sprintTimer = 1;
            if (direction.magnitude == 0) { sprintCondition = false; sprintTimer = 0; }
        }

        
    }

    /**GETTERS
     * 
     */
     //depricated
    public bool getJumpCondition() {
        return jumpCondition;
    }
    public float getDeltaTime() {
        return deltaTime;
    }
    public float getPreservedTime() {
        //using the get function to minimize "destruction"
        return t;
    }
    public bool getSprint() {
        return sprintCondition;
    }

    void OnDrawGizmos()
    {
        if (drawDebug) {
            try
            {
                Gizmos.color = Color.red;
                //Gizmos.DrawRay(camTracker.transform.position, debugVector);

                Gizmos.color = Color.cyan;
                //Gizmos.DrawSphere(debugPoint0, 0.01f);
                Gizmos.color = Color.green;
                //Gizmos.DrawSphere(debugPoint1, 0.01f);

                Gizmos.color = Color.white;
                //Gizmos.DrawWireSphere(pivotPoint, 0.01f);

                Gizmos.color = Color.red;
                //Gizmos.DrawSphere(cameraBehavior.getRef(), 0.1f);
                //Gizmos.DrawWireSphere(cameraBehavior.getRef(), 1f);

                Gizmos.color = Color.blue;
            }
            catch (Exception)
            {

            }
        }
    }
}
