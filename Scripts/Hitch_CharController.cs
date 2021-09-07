using System;
using UnityEngine;
using Hitch_EasyMath;

public enum CameraModes{ 
    THIRD_PERSON = 0, 
    CUSTOM 
}

//[AddComponentMenu("Test")] // Changes the name in the component menu!!//
[RequireComponent(typeof(Hitch_InputManager))]
[RequireComponent(typeof(Hitch_CharAnimation))]
public class Hitch_CharController : MonoBehaviour
{
    
    public Hitch_CharacterVariables vars;
    public CameraSettings cameraSettings;
    public PhysicsSettings physicsSettings;
    
    [HideInInspector] new public Camera camera;
    [HideInInspector] public GameObject player;
    [HideInInspector] public Transform cameraLookAt, cameraPivot;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public Hitch_InputManager controls;
    [HideInInspector] public Hitch_CharAnimation charAnimation;


    public CameraActor[] cameraTypes = new CameraActor[4];
    public CameraActor currentCamera;
    public CameraModes currentCamMode = CameraModes.THIRD_PERSON;

    public TraversalFacade traversalFacade = new TraversalFacade();

    [HideInInspector] public Vector3 direction, resetPos, resetCameraPos;
    [HideInInspector] public float deltaTime;

    public bool startJump = false, sprintCondition = false, jumpCondition = false;
    private float t = 0;

    public bool drawDebug = false;

    void Start()
    {
        Hitch_TC_Finder finder = GetComponentInParent<Hitch_TC_Finder>();
        Hitch_TCompactor compactor = finder.compactor;

        cameraTypes[(int)CameraModes.THIRD_PERSON] = GetComponent<ThirdPersonCamera>();
        //cameraTypes[(int)CameraModes.CUSTOM] = this.gameObject.AddComponent(typeof(ThirdPersonCamera)) as ThirdPersonCamera; Should remain as NULL untill something adds to it

        camera          = Camera.main;
        cameraLookAt    = compactor.GetTransform("CameraLook");
        cameraPivot     = compactor.GetTransform("CameraPivot");

        cameraSetup((int)currentCamMode);
        animationSetup();

        controls = GetComponent<Hitch_InputManager>();

        traversalFacade.setPhysicsBody(this.gameObject.transform);
        traversalFacade.setPhysicsSettings(physicsSettings);
        traversalFacade.setUp(); //done after as setUp requires the settings and transform first.

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
    private void cameraSetup(int index)
    {
        cameraTypes[index].setCamera(camera);
        cameraTypes[index].setLookAt(cameraLookAt);
        cameraTypes[index].setPivot(cameraPivot);
        cameraTypes[index].setSecondary(this.transform);
        cameraTypes[index].setCameraSettings(cameraSettings);
    }

    void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        t = 1 - Mathf.Pow(0.001f, deltaTime);
        currentCamera = cameraTypes[(int)currentCamMode];
        //Controller and movement
        UpdateState();

        try
        {
            
            direction = currentCamera.transformToView(controls.inLeftH, Vector3.up);
            direction = direction.normalized;
            traversalFacade.refresh(direction, controls.jump, controls.sprinting, false);
        }
        catch (Exception e){
            Debug.LogWarning("something from Update Movement: " + e.Message);
            Debug.LogWarning("additional: " + e.StackTrace);
        }
        
        try
        {
            //charAnimation.UpdateState();
            //charAnimation.UpdateAnimation();
        }
        catch (Exception e) {
            Debug.LogWarning("Animation error, probably had to divide by zero: " + e.Message);
        }

        //camera based stuff
        try{
            //Vector3 playerOffset = new Vector3(charMovement.getVelocity().x, (charMovement.isInAir()? 0.5f : 1) * charMovement.getVelocity().y, charMovement.getVelocity().z);
            currentCamera.setCameraSettings(cameraSettings);
            currentCamera.refresh(controls.inRightH);

        }
        catch (Exception) {
            Debug.LogWarning("something from Camera");
        }
    }
    void UpdateState()
    {
        if (controls.restart) { transform.position = resetPos; }
    }

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

    public Vector3 getVelocity() => traversalFacade.getVelocity();
    
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
