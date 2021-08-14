using System;
using UnityEngine;

public enum moveState
{
    IN_AIR = 0,
    GROUNDED,
    CLIMBING
}

[RequireComponent(typeof(Hitch_CharController))]
public class Hitch_CharMovement : MonoBehaviour {

    /**
     * Manually set the character variables.
     * I cant add them through code since
     * scriptable objects only exist in the folders
     * cant be attached to a gameobject
     */
    private Hitch_CharacterVariables vars;
    Hitch_CharController controller;
    

    public Vector3 initialVelocity = Vector3.zero;
    public Vector3 currentVelocity = Vector3.zero;
    public Vector3 groundNormal = Vector3.up;
    private Vector3 airUserVelocity = Vector3.zero;
    public float velocityMag = 0;
    //DEBUG STUFF
    Vector3 hitPosition0, hitNormal0;
    Vector3 hitPosition1, hitNormal1;
    Vector3 hitPosition2, hitNormal2;
    //DEBUG STUFF
    Vector3 direction;
    public moveState state = moveState.IN_AIR;
    public bool sliding = false;
    public bool grounded = false;

    public float airTimer = 0;
    float acceleration, maxSpeed;
    public bool drawDebug = false;

    void Start() {
        //Get the controller components, if you havent already.

        controller = GetComponent<Hitch_CharController>();
        vars = controller.vars;
    }
    public void UpdateMovement() {
        
        switch (state) {
            case moveState.GROUNDED: {

                    groundMovement();
                    applyIt();

                    //so you stay out of the ground
                    lifterSphere(vars.stickiness);

                    break;
                }
            case moveState.IN_AIR: {

                    inAirMovement();
                    mainBodyCollision();
                    applyIt();

                    //incase you hit the ground
                    lifterSphere(vars.stickiness);
                    break;
                }
            case moveState.CLIMBING: {

                    climbingMovement();

                    break;
                }
            default: {
                    Debug.LogWarning("How did you get in here dude?");
                    break;
                }
        }
    }

    void groundMovement() {
        acceleration = controller.sprintCondition ? vars.runAcceleration : vars.walkAcceleration;
        maxSpeed = controller.sprintCondition ? vars.maxRunSpeed : vars.maxWalkSpeed;


        if (sliding && false)
        {
            //will add functionality later
            resetAirStuff(currentVelocity);
            inAirMovement();
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
        }
        else
        {
            direction = Vector3.ProjectOnPlane(controller.direction, groundNormal);
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, groundNormal);

            currentVelocity += direction * acceleration * controller.deltaTime * (((4 - 1) * Mathf.Pow(Vector3.Dot(currentVelocity.normalized, direction) - 1, 2f)) / 4f + 1);

            if (currentVelocity.magnitude > maxSpeed) {
                currentVelocity = Vector3.Lerp(currentVelocity, currentVelocity.normalized * maxSpeed, controller.getPreservedTime() * 3);
            }

            if (direction.magnitude == 0)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, controller.getPreservedTime() / vars.time2Stop);
            }

        }

        //Just incase you hit multiple surfaces at the same time.
        for (int i = 0; i < vars.collisionChecks - 1; i++) {
            if (!mainBodyCollision()) return;
        }
        if (mainBodyCollision()) {
            Debug.LogWarning("Main body collision might be sketch as frick");
        }

    }
    void inAirMovement() {
        airTimer += controller.getDeltaTime();
        airUserVelocity += controller.direction * acceleration * controller.getDeltaTime() / 2;

        Vector3 usersAir = Vector3.ProjectOnPlane(initialVelocity, Vector3.up) + airUserVelocity;
        
        if (controller.getSprint()) {
            if (usersAir.magnitude > vars.maxRunSpeed) usersAir = usersAir.normalized * vars.maxRunSpeed;
        }
        else {
            if (usersAir.magnitude > vars.maxWalkSpeed) usersAir = usersAir.normalized * vars.maxWalkSpeed;
        }
        usersAir -= Vector3.ProjectOnPlane(initialVelocity, Vector3.up);

        currentVelocity = initialVelocity + vars.gravity * Vector3.up * airTimer + usersAir;
        
    }
    void climbingMovement()
    {

    }

    /**Does a main Body collision test.
     * Returns true if it had to make some difficult decision
     * Returns false if it didnt hit anything.
     */
    bool mainBodyCollision() {
        bool hitSomething;
        RaycastHit hitInfo0, hitInfo1;

        (hitSomething, hitInfo0) = capsuleCast(currentVelocity, currentVelocity.magnitude * controller.getDeltaTime());
        if (hitSomething)
        {
            //DEBUG
            hitPosition0 = hitInfo0.point;
            hitNormal0 = hitInfo0.normal;
            //DEBUG
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, hitInfo0.normal);

            (hitSomething, hitInfo1) = capsuleCast(currentVelocity, currentVelocity.magnitude * controller.getDeltaTime());
            if (hitSomething)
            {
                //DEBUG
                hitPosition1 = hitInfo1.point;
                hitPosition2 = (hitPosition1 + hitPosition0) / 2f;
                hitNormal1 = hitInfo1.normal;
                //DEBUG
                //check if acute or obtuse so we can handle it properly
                if (Vector3.Dot(hitInfo0.normal, hitInfo1.normal) > 0.5f)
                {
                    hitNormal2 = Vector3.Normalize(hitInfo0.normal + hitInfo1.normal);
                    //slides along the average
                    //Debug.Log("sliding");
                    currentVelocity = Vector3.ProjectOnPlane(currentVelocity, Vector3.Normalize(hitInfo0.normal + hitInfo1.normal));
                }
                else
                {
                    hitNormal2 = Vector3.Cross(hitInfo0.normal, hitInfo1.normal);
                    //preserves "verticle" movement
                    //Debug.Log("corner");
                    currentVelocity = Vector3.Project(currentVelocity, Vector3.Cross(hitInfo0.normal, hitInfo1.normal));
                }

                resetAirStuff(currentVelocity);
                return true;
            }
            resetAirStuff(currentVelocity);
        }
        
        return false;
    }
    void lifterSphere(float stickiness) {
        bool hitSomething;
        RaycastHit hitInfo;

        

        (hitSomething, hitInfo) = sphereCast(Vector3.down, stickiness);
        if (hitSomething) {
            groundNormal = hitInfo.normal;
            groundNormal = triangleNormal(Vector3.down, stickiness * 2, 0.5f);


            if (hitInfo.distance > 0){
            }
            else{
                //Might be good to do do this in both? For smoothness?

                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down * hitInfo.distance, controller.getPreservedTime() * 2);
                //rigidbody.MovePosition(Vector3.Lerp(transform.position, transform.position + Vector3.down * hitInfo.distance, controller.getPreservedTime() * 2) + transform.position);
            }

            if (shouldSlide())
            {
                sliding = true;
                //currentVelocity = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
                if (mainBodyCollision()) Debug.LogWarning("Comming from lifterSphere, that main body collision was a bit wack, but dont worry");
            }
            else {
                sliding = false;
            }
            state = moveState.GROUNDED;
            resetAirStuff(currentVelocity);
        }
        else {
            state = moveState.IN_AIR;
        }
    }
    void applyIt() {
        RaycastHit hit;
        //DOUBLE CHECK
        if (Physics.CapsuleCast(transform.position + vars.bOffset1,
                                transform.position + vars.bOffset2,
                                vars.bodyRadius * 0.2f,
                                currentVelocity,
                                out hit,
                                vars.bodyRadius * (1 - 0.2f), 
                                vars.mask))
        {
            //YOU ARE INSIDE OF SOMETHING GET THE FUCK OUT. 

            transform.position -= currentVelocity.normalized * (vars.bodyRadius * (1 - 0.2f) - hit.distance + 0.001f);
            //rigidbody.MovePosition(transform.position - currentVelocity.normalized * (vars.bodyRadius * (1 - 0.2f) - hit.distance + 0.001f));  

            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, hit.normal);
            resetAirStuff(currentVelocity);
            state = moveState.IN_AIR;
        }

        if ((currentVelocity.magnitude < 0.8f && direction.magnitude == 0)) currentVelocity = Vector3.zero;
        velocityMag = currentVelocity.magnitude;

        //division is never zero, as current > terminal
        if (currentVelocity.magnitude > vars.terminalVelocity) currentVelocity = currentVelocity * vars.terminalVelocity / currentVelocity.magnitude;
        transform.position += currentVelocity * controller.getDeltaTime();
        //rigidbody.MovePosition(transform.position + currentVelocity * controller.getDeltaTime());
    }
    void resetAirStuff(Vector3 newInitialVelocity) {
        airUserVelocity = Vector3.zero;
        initialVelocity = newInitialVelocity;
        airTimer = 0;
    }

    private (bool, RaycastHit) capsuleCast(Vector3 rayDirection, float distance) {
        RaycastHit hit = new RaycastHit();
        
        if (Physics.CapsuleCast(transform.position + vars.bOffset1, transform.position + vars.bOffset2, vars.bodyRadius, rayDirection, out hit, distance, vars.mask))
        {
            return (true, hit);
        }

        return (false, hit);
    }
    private (bool, RaycastHit) sphereCast(Vector3 rayDirection, float distance) {
        RaycastHit hit = new RaycastHit();
        
        //adding on the buffer incase environment is inside lifter sphere
        if (Physics.SphereCast( transform.position + vars.bOffset2, 
                                vars.lifterRadius, 
                                rayDirection , 
                                out hit, 
                                distance + vars.bOffset2.magnitude - vars.lOffset.magnitude, 
                                vars.mask)) {

            //remove buffer distance
            //Just incase you're inside something
            hit.distance -= (vars.bOffset2.magnitude - vars.lOffset.magnitude);
            return (true, hit);
        }
        return (false, hit);
    }
    //To smooth out corners/ edges
    private Vector3 triangleNormal(Vector3 rayDirection,float distance, float ratio) {
        Vector3 output1, output2, rayOffset;
        Vector3[] hitPoints = new Vector3[6];
        RaycastHit hit;
        float bufferDistance = vars.bOffset2.magnitude + (1 - ratio) * vars.lifterRadius - vars.lOffset.magnitude;
        int numberOfHits = 0;
        int increments = 0;
        do{
            increments++;
            rayOffset = new Vector3(Mathf.Cos(2 * Mathf.PI * 1.618f * increments), 0, Mathf.Sin(2 * Mathf.PI * 1.618f * increments)) * vars.lifterRadius * 2;
            if (Physics.SphereCast( transform.position + vars.bOffset2 + rayOffset, 
                                    vars.lifterRadius * ratio, 
                                    rayDirection, 
                                    out hit, 
                                    distance + bufferDistance, 
                                    vars.mask)) {

                hitPoints[numberOfHits] = hit.point;
                numberOfHits++;
            }

            if (increments > vars.collisionChecks * 6) return groundNormal;
        } while (numberOfHits < 6);

        output1 = Vector3.Normalize(Vector3.Cross(hitPoints[2] - hitPoints[0], hitPoints[1] - hitPoints[0]));
        output2 = Vector3.Normalize(Vector3.Cross(hitPoints[5] - hitPoints[3], hitPoints[4] - hitPoints[3]));

        if (Vector3.Dot(output1, Vector3.up) < 0) output1 *= -1;
        if (Vector3.Dot(output2, Vector3.up) < 0) output2 *= -1;

        return Vector3.Normalize(output1 + output2);
    }

    bool shouldSlide() {
        return (Vector3.Angle(Vector3.up, groundNormal) >= vars.maxStandingAngle);
    }

    public void Jump() {
        state = moveState.IN_AIR;
        initialVelocity.y = vars.jumpSpeed;
    }
    public bool isGrounded() { 
        switch (state) {
            case (moveState.GROUNDED):
                return true;
            default:
                return false;
        }
    }
    public bool isInAir() {
        switch (state)
        {
            case (moveState.IN_AIR):
                return true;
            default:
                return false;
        }
    }
    public bool isClimbing() {
        switch (state)
        {
            case (moveState.CLIMBING):
                return true;
            default:
                return false;
        }
    }
    public float getAirTimer() {
        return airTimer;
    }
    public Vector3 getVelocity() {
        return currentVelocity;
    }
    public Vector3 getStandingNormal() {
        return groundNormal;
    }


    
    //NICE DEBUG TOOLS TO SEE IF STUFF IS IN THEIR PROPER SPOT
    void OnDrawGizmos()
    {
        if (drawDebug) {
            try
            {


                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position + vars.lOffset, vars.lifterRadius);
                //Gizmos.DrawWireSphere(transform.position + vars.bOffset2, vars.lifterRadius);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hitPosition0, 0.1f);
                Gizmos.DrawLine(hitPosition0, hitPosition0 + hitNormal0);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hitPosition1, 0.1f);
                Gizmos.DrawLine(hitPosition1, hitPosition1 + hitNormal1);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hitPosition2, 0.1f);
                Gizmos.DrawLine(hitPosition2, hitPosition2 + hitNormal2);

                /*
                Vector3 rayOffset;
                for (int i = 0; i < 6; i++) {
                    rayOffset = new Vector3(Mathf.Cos(2 * Mathf.PI * 1.618f * i), 0, Mathf.Sin(2 * Mathf.PI * 1.618f * i)) * vars.lifterRadius * 0.75f;
                    Gizmos.DrawWireSphere(transform.position + vars.bOffset2 + rayOffset + Vector3.down*(vars.bOffset2.magnitude + (1 - 0.5f) * vars.lifterRadius - vars.lOffset.magnitude) , vars.lifterRadius * 0.5f);
                }
                */

                //Gizmos.DrawWireSphere(transform.position + vars.bOffset2, vars.lifterRadius);
                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(transform.position + vars.bOffset2, transform.position + vars.bOffset2 + Vector3.down * (vars.bOffset2.magnitude - vars.lOffset.magnitude + vars.stickiness));

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + vars.bOffset1, vars.bodyRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + vars.bOffset2, vars.bodyRadius);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + groundNormal);
            }
            catch (NullReferenceException)
            {
                //do nothing
            }
        }
        
    }
}
