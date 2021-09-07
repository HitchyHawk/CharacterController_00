using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTraversal : PhysicsImpl {
    bool targettedLanding = false, firstTime = true, isValidState = true;

    Vector3 target, initialVelocity, initialPosition, newPosition;
    Vector3[] offsets = new Vector3[2];

    float gravity, stepUpMax, jumpSpeed, timer = 0, maxAngle;
    int MAX_COLLISION = 5;

    
    public override bool inValidState()
    {
       return isValidState;
    }

    public override void refresh(Vector3 influence)
    {
        bottomSphere = body.position + offsets[0];
        topSphere = body.position + offsets[1];

        if (firstTime){
            initialVelocity = currentVelocity + Vector3.up * (influence.y > 0? jumpSpeed: 0);
            initialPosition = body.position;
            timer = 0;
            firstTime = false;
        }
        timer += Time.deltaTime;

        if (targettedLanding){
            //do something with the velocity so you land there
        }

        newPosition = initialPosition + initialVelocity * timer + Vector3.down * gravity * Mathf.Pow(timer, 2) / 2;
        currentVelocity = (newPosition - body.position) / Time.deltaTime;

        for (int i = 0; i < MAX_COLLISION; i++){
            if (!collision()) break; //safe to travel
            else{
                firstTime = true; //hit something prepare for a recalculation
            }
        }

        
        if (currentVelocity.y < 0 && isValidState){
            Debug.Log("checking for grounding");
            if (groundBelow()) isValidState = false;
            else isValidState = true;
            if (isValidState){
                Debug.Log("Ground is fine from here");
            } else{
                Debug.Log("OH SHIT GROUND NOT FINE!!");
            }
        }
        //check for ground

        body.position += currentVelocity * Time.deltaTime;
    }

    bool collision(){
        Vector3 workingVelocity = newPosition - body.position, normal1, collisionVelocity, cornerNormal;
        RaycastHit hit;

        if (quickCapsuleCast(workingVelocity, workingVelocity.magnitude, out hit)){
            normal1 = hit.normal;
            collisionVelocity = Vector3.ProjectOnPlane(workingVelocity, normal1);
            if (possibleFloorNormal(normal1)) isValidState = false;

            if (quickCapsuleCast(collisionVelocity, collisionVelocity.magnitude, out hit)){
                cornerNormal = Vector3.Cross(normal1, hit.normal);
                workingVelocity = Vector3.Project(workingVelocity, cornerNormal) * 0.9f + (normal1 + hit.normal).normalized * workingVelocity.magnitude * 0.1f; //reflect 10%

                currentVelocity = workingVelocity / Time.deltaTime;
                if (possibleFloorNormal(hit.normal)) isValidState = false;

                return true;
            }

            currentVelocity = collisionVelocity / Time.deltaTime;
            return true;
        }

        return false;
    }

    bool groundBelow(){
        RaycastHit hit;
        if (quickSphereCast(topSphere, Vector3.down, offsets[1].magnitude + Mathf.Abs(currentVelocity.y) * Time.deltaTime, out hit)){
            if (possibleFloorNormal(hit.normal)) return true;
        }
        return false;
    }

    bool possibleFloorNormal(Vector3 potentialNormal){
        return (Vector3.Angle(Vector3.up, potentialNormal) < maxAngle);
    }


    public override void setSettings(PhysicsSettings newSettings) {
        stepUpMax = newSettings.stepUpMax;
        jumpSpeed = 2 * newSettings.apexHeight / newSettings.timeToApex;
        gravity = jumpSpeed / newSettings.timeToApex;
        collidables = newSettings.collidables;
        collisionRadius = newSettings.collisionRadius;
        maxAngle = newSettings.maxStandingSlopeAngle;
    }
    public override void setCapsules(Vector3[] newOffsets) {
        offsets = newOffsets;
        offsets[0] += Vector3.up * stepUpMax;
        offsets[1] += Vector3.up * stepUpMax;
    }
    public override void setTarget(Vector3 newTarget)
    {
        target = newTarget;
    }
    public override void activate()
    {
        targettedLanding = true;
    }
    public override void reset(){
        firstTime = true;
        isValidState = true;
    }

    
}
