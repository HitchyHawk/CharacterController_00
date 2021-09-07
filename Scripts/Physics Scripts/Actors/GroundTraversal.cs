using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTraversal : PhysicsImpl {

    RaycastHit groundHit;
    Vector3 nextPoint, acceleration = Vector3.zero, primeAcceleration = Vector3.zero, previousGroundNormal;
    Vector3 previousVelocity = Vector3.zero, previousAcceleration = Vector3.zero;
    
    Vector3[] sphereOffsets = new Vector3[2];
    Vector3 prediction = Vector3.zero;
    float maxSlopeAngle = 60, collisionHeight, stepUpMax = 1, preservedTime;
    bool isValidState = true;
    float time = 0;
    [Range(0.01f, 1f)] public float timeMax = 0.5f;

    private int[] options = {1,0,-1,0,0,1,0,-1};
    public override bool inValidState() {
        return isValidState;
    }

    public override void refresh(Vector3 newInfluence) {
        preservedTime = 1 - Mathf.Pow(0.001f, Time.deltaTime);
        bottomSphere = body.position + sphereOffsets[0];
        topSphere    = body.position + sphereOffsets[1];

        Vector3 potentialVelocity = Vector3.zero;
        Vector3 newPosition = body.position;
        
        currentVelocity += newInfluence;
        
        if (newInfluence.magnitude < 0.01f || !isValidState){
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, preservedTime);
            if (currentVelocity.magnitude < 0.1f) currentVelocity = Vector3.zero;
        }

        acceleration = Vector3.Lerp(acceleration, (currentVelocity - previousVelocity) / Time.deltaTime, preservedTime * 0.5f);
        primeAcceleration = Vector3.Lerp(primeAcceleration, (acceleration - previousAcceleration) / Time.deltaTime, preservedTime * 0.5f);

        previousVelocity = currentVelocity;
        previousAcceleration = acceleration;
        
        time += Time.deltaTime;
        if (time > timeMax || true){
            prediction = Vector3.Lerp(prediction, body.position + currentVelocity * timeMax, preservedTime / timeMax); // figure out a nice way to smoothly go back to max speed
                                                                                                                 // probably with lerps and conditions
            prediction.y = body.position.y;
            
            time = 0;
        }

        isValidState = calculateGroundInformation();
        

        if (isValidState){
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.ProjectOnPlane(currentVelocity, groundHit.normal), preservedTime * 4);
            newPosition = Vector3.Lerp(newPosition, topSphere + Vector3.down * (groundHit.distance + collisionRadius), preservedTime * 2);

            if (calculateNextPoint() && newInfluence.magnitude > 0.1f){
                potentialVelocity = (nextPoint - newPosition) / (Time.deltaTime * timeMax); //timeMax since thats where it will be in timeMax seconds.
                newPosition.y = Mathf.Lerp(newPosition.y, nextPoint.y, preservedTime * timeMax);
                //currentVelocity = Vector3.Lerp(currentVelocity, potentialVelocity, preservedTime * 2);
                currentVelocity = Vector3.RotateTowards(currentVelocity, potentialVelocity, Time.deltaTime * 60 * Mathf.Deg2Rad, 0);
            } else{
                
                
                //lerp to stay ontop of the ground
            }
        } else { 
            //currentVelocity = Vector3.Project(currentVelocity, Vector3.ProjectOnPlane(Vector3.down, groundHit.normal));
        }
        currentVelocity = Vector3.Lerp(currentVelocity, (newPosition + currentVelocity * Time.deltaTime - body.position) / Time.deltaTime, preservedTime);
        

        for (int i = 0; i < 3; i++){
            if (velocityCollision()) break;
            //breaks if theres no collision... WHICH IS A GOOD THING!! Dont need to keep calculating.
        }
        if (currentVelocity.magnitude > 10) currentVelocity = currentVelocity.normalized * 10;

        body.position += currentVelocity * Time.deltaTime;
        previousGroundNormal = groundHit.normal;
        
    }

    bool calculateGroundInformation(){
        Vector3[] points = new Vector3[4];
        if (quickSphereCast(topSphere, Vector3.down, collisionHeight + collisionRadius + 3 * stepUpMax, out groundHit)){
            RaycastHit hit;
            Vector3 rayOrigin;
            for (int i = 0; i < 4; i++){
                rayOrigin = topSphere + new Vector3(options[i], 0 , options[i + 4]) * collisionRadius * 0.75f;
                if (quickSphereCast(rayOrigin, Vector3.down, collisionHeight + collisionRadius + 3 * stepUpMax, collisionRadius / 4f, out hit)){
                    points[i] = rayOrigin + Vector3.down * hit.distance;
                } else {
                    points[i] = rayOrigin + Vector3.down * (collisionHeight + collisionRadius + 3 * stepUpMax);  
                }
            }

            Vector3 n1 = Vector3.Cross(points[2] - points[0], points[2] - points[1]).normalized;
            Vector3 n2 = Vector3.Cross(points[3] - points[0], points[3] - points[1]).normalized;

            //makes sure normals go up.
            if (Vector3.Dot(n1, Vector3.down) > 0) n1 *= -1;
            if (Vector3.Dot(n2, Vector3.down) > 0) n2 *= -1;

            groundHit.normal = Vector3.Lerp(previousGroundNormal, (groundHit.normal + n1 + n2).normalized, preservedTime).normalized;
            Debug.DrawRay(body.position, groundHit.normal, Color.red, 2);
            //Debug.Log("FIRE THE RAY!!: " + (n1 + n2).normalized);

            return isValidAngle(groundHit.normal);
        }
        return false;
        
    }
    bool calculateNextPoint(){
        RaycastHit hit;
        Vector3 origin = prediction + topSphere - body.position;
        if (quickSphereCast(topSphere, origin - topSphere, (origin - topSphere).magnitude)){
            return false;
        }

        if (quickSphereCast(origin, Vector3.down, collisionHeight)) return false;

        if (quickSphereCast(origin + Vector3.down * collisionHeight, Vector3.down, collisionRadius + stepUpMax * 3, out hit)){
            //should check vertical spacing FIX ME!!
            nextPoint = origin + Vector3.down * (hit.distance + collisionRadius + collisionHeight);
            return true;
        }
        return false;
    }
    bool velocityCollision(){
        RaycastHit hit;
        Vector3 workingVelocity = currentVelocity * Time.deltaTime;
        Vector3 collisionVelocity, collisionNormal;

        if (quickCapsuleCast(workingVelocity, workingVelocity.magnitude, out hit)){
            collisionNormal = hit.normal;
            collisionVelocity = Vector3.ProjectOnPlane(workingVelocity, hit.normal);

            if (quickCapsuleCast(collisionVelocity, collisionVelocity.magnitude, out hit)){
                Vector3 cornerVector = Vector3.Cross(hit.normal, collisionNormal);

                collisionVelocity = Vector3.Project(workingVelocity, cornerVector);

                if (quickCapsuleCast(collisionVelocity, collisionVelocity.magnitude, out hit)){
                    collisionVelocity = -collisionVelocity * 0.5f;
                }
            }

            //had a collision
            currentVelocity = collisionVelocity / Time.deltaTime;
            return false;
        }
        
        return true;
    }
    bool isValidAngle(Vector3 normal){
        return Vector3.Angle(Vector3.up, normal) < maxSlopeAngle;
    }
    bool enoughVerticalSpace(Vector3 position){
        return true;
        //return quickSphereCast(position, Vector3.up, collisionHeight);
    }
    public override void setSettings(PhysicsSettings newSettings){
        physicsSettings = newSettings;
        collisionRadius = physicsSettings.collisionRadius;
        collisionHeight = physicsSettings.collisionHeight;
        stepUpMax = physicsSettings.stepUpMax;
        maxSlopeAngle = physicsSettings.maxStandingSlopeAngle;
        collidables = physicsSettings.collidables;
    }
    public override void setCapsules(Vector3[] offsets)
    {
        sphereOffsets = offsets;
        offsets[0] += Vector3.up * stepUpMax;
    }

    

}
