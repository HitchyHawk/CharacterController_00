using UnityEngine;
using Hitch_EasyMath;

[RequireComponent(typeof(Hitch_CharController))]
public class Hitch_CharAnimation : MonoBehaviour
{
    private Hitch_CharacterVariables vars;
    [HideInInspector]public GameObject player;
    [HideInInspector]public Animator animator;
    [HideInInspector]public Hitch_CharController controller;
    //[HideInInspector]public Hitch_CharMovement charMovement;
    //[HideInInspector] Hitch_CharPhysics physics;

    Vector3 facingDirection, 
            newFacingDirection,
            travelingDirection, 
            previousTD,
            velocity;

    float currentSidewaysStrength = 0,
          angle = 0;
    [HideInInspector] public float desiredForwardAngle = 0;
    [HideInInspector] public bool leftfoot = true;
    
    [Range(0, 1)] public float phase = 0;
                  public float distanceTraveled = 0;
    [Range(0, 1)] public float stationaryTimer = 0;

                  public bool  transitioning = false;
                  public float transitionTime = 0;
                  public float transitionTimer = 0;
    
    public int rawCurrentFrame = 0;

    Hitch_AnimationMeta currentAnimationMeta;
    Hitch_AnimationMeta transitionAnimationMeta;

    public string cAnim;
    public string tAnim;


    private void Start() {
        
        controller = GetComponent<Hitch_CharController>();
        //charMovement = GetComponent<Hitch_CharMovement>();

        vars = controller.vars;

        facingDirection = new Vector3(-Mathf.Sin(-angle*Mathf.Deg2Rad), 0, Mathf.Cos(-angle * Mathf.Deg2Rad));
        previousTD = facingDirection;
        currentAnimationMeta = vars.idleMeta;
        transitionAnimationMeta = vars.idleMeta;
    }
    public void UpdateState() {
        float deltaTime = controller.deltaTime;
        float t = 1 - Mathf.Pow(0.001f, deltaTime);
        float a = 0, currentForwardAngle = player.transform.localEulerAngles.x;
        float rawStrength = 0;
        float orientation;
        velocity = controller.getVelocity();

        //GETTING DIRECTIONS
        travelingDirection = Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude != 0 ? Vector3.ProjectOnPlane(velocity, Vector3.up).normalized : previousTD;

        //SETS UP LOOKING DIRECTION
        orientation = 1 - Vector3.Distance(Vector3.Cross(facingDirection, travelingDirection).normalized, Vector3.up);


        if (velocity.magnitude > 0){
            newFacingDirection = Vector3.RotateTowards( facingDirection,
                                                        travelingDirection,
                                                        deltaTime * vars.turnSpeed * (2.2f - Vector3.Dot(facingDirection, travelingDirection)), 1);

            a = Vector3.Angle(facingDirection, newFacingDirection) * (orientation == 0 ? 1 : orientation);

            currentForwardAngle = Mathf.LerpAngle(currentForwardAngle, desiredForwardAngle, t * 2);
        }

        if (velocity.magnitude > 0){
            /*
            switch (controller.currentPhysicsMode){
                case PhysicsModes.STANDING:
                    rawStrength = (1 - Vector3.Dot(newFacingDirection, travelingDirection)) / 2f;
                    break;
            }
            */
        }

        currentSidewaysStrength = Mathf.Lerp(currentSidewaysStrength, -orientation * rawStrength, t);

        player.transform.eulerAngles += new Vector3(0, a, 0);
        player.transform.localEulerAngles = new Vector3(currentForwardAngle,
                                                        player.transform.localEulerAngles.y,
                                                        currentSidewaysStrength * vars.sidewaysRotation);

        facingDirection = new Vector3(-Mathf.Sin(-player.transform.localEulerAngles.y * Mathf.Deg2Rad),
                                      0, 
                                       Mathf.Cos(-player.transform.localEulerAngles.y * Mathf.Deg2Rad));

        previousTD = travelingDirection;
    }
    
    public void UpdateAnimation(){
        
    }

    /*
    public void UpdateAnimation()
    {
        float deltaTime = controller.deltaTime;
        Vector3 groundVelocity = Vector3.ProjectOnPlane(velocity, charMovement.getStandingNormal());
        distanceTraveled += groundVelocity.magnitude * deltaTime;

        
        if (charMovement.isGrounded()){
            if (controller.startJump && controller.sprintCondition) {
                if (phase % 1 < 0.5f)   CustomAnimationCaller(vars.sRJumpMeta, 0);
                else                    CustomAnimationCaller(vars.sLJumpMeta, 0);   

            } else if (groundVelocity.magnitude > 0.75f) {
                if (groundVelocity.magnitude < 0.65f * vars.maxRunSpeed) CustomAnimationCaller(vars.walkMeta, 0.1f);
                else                                                     CustomAnimationCaller(vars.sprintMeta, 0.1f);

            } else CustomAnimationCaller(vars.idleMeta, 0.5f);

        } else if (charMovement.getAirTimer() > vars.time2Apex*1.5f) CustomAnimationCaller(vars.fallingMeta, 1.5f);


        if (rawCurrentFrame >= 1 && controller.startJump) { controller.Jump(); controller.startJump = false; }

        AnimationStep();

        cAnim = currentAnimationMeta.animationName;
        tAnim = transitionAnimationMeta.animationName;
    }
    void CustomAnimationCaller(Hitch_AnimationMeta _meta, float transTime){
        if (_meta.animationName != currentAnimationMeta.animationName)
        {
            if (currentAnimationMeta.loops){
                animator.CrossFadeInFixedTime(  _meta.animationName, 
                                                transTime, 
                                                -1, 
                                                (rawCurrentFrame - 1) / (float)(currentAnimationMeta.frameAmount));

                //since we are transitioning to a new animation
                //and the system remeber the last automatically we can just set everything over
                currentAnimationMeta = _meta;
                //adjusting the phase so its at the right time.
                //also dingus proofed
                phase = distanceTraveled / (2 * currentAnimationMeta.distancePerCycle);

            } else {
                //since the new animation doesnt loop, we need to initiate our transition thing
                //if its the start, transition timer starts at zero.
                //else just keep it all the same
                if (!transitioning) transitionTimer = 0;
                transitioning = true;
                transitionTime = transTime;
            }

            stationaryTimer = 0;

            if (!_meta.loops) rawCurrentFrame = 0;

            transitionAnimationMeta = _meta;
        }
        else {
            //since nothing is different, dont do anything.
            transitioning = false;
            transitionTimer = 0;
            transitionTime = 0;
        }
        
    }
    void AnimationStep() {
        if (transitioning)
        {
            //since we are transitioning
            transitionTimer += controller.deltaTime;
            animator.CrossFadeInFixedTime(  transitionAnimationMeta.animationName, 
                                            transitionTime, 
                                            -1, 
                                            (rawCurrentFrame - 1) / (float)(currentAnimationMeta.frameAmount), 
                                            transitionTimer / transitionTime);

            //if the transition is done, stop transitioning further
            if (transitionTimer >= transitionTime)
            {
                transitionTimer = 0;
                transitioning = false;
                currentAnimationMeta = transitionAnimationMeta;
            }
        }
        else {
            //normal animation stepping
            if (currentAnimationMeta.isStationary)
            {

                distanceTraveled = 0;
                stationaryTimer += controller.deltaTime / currentAnimationMeta.distancePerCycle;
                if (!currentAnimationMeta.loops)
                {
                    if (stationaryTimer > (currentAnimationMeta.frameAmount - 1) / ((float)currentAnimationMeta.frameAmount))
                    {
                        stationaryTimer = (currentAnimationMeta.frameAmount - 1) / ((float)currentAnimationMeta.frameAmount);
                    }
                }
                else if (stationaryTimer > 1) stationaryTimer -= 1;

                rawCurrentFrame = (int)(currentAnimationMeta.frameAmount * stationaryTimer);

            }
            else
            {

                if (distanceTraveled > 2 * currentAnimationMeta.distancePerCycle)
                {
                    distanceTraveled = currentAnimationMeta.loops ? distanceTraveled - 2 * currentAnimationMeta.distancePerCycle : 2 * currentAnimationMeta.distancePerCycle;
                }
                phase = distanceTraveled / (2 * currentAnimationMeta.distancePerCycle);
                

                rawCurrentFrame = (int)(currentAnimationMeta.frameAmount * phase);
            }
           
        }
        
        

        animator.SetFloat("FrameCycle", rawCurrentFrame / ((float)currentAnimationMeta.frameAmount));

    }
    */
    public void setPlayer(GameObject playerObject){
        player = playerObject;
    }
    public void setAnimator(Animator newAnimator){
        animator = newAnimator;
    }
}
