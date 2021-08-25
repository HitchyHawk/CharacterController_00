using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTraversal : PhysicsImpl {

    RaycastHit groundHit;
    Vector3 groundNormal, influence = Vector3.zero, nextPoint, topSphere, bottomSphere;
    float maxSlopeAngle = 60, collisionRadius = 0.1f, collisionHeight, stepUpMax = 1, /*minHeadHeight = 1.5f,*/ preservedTime;
    LayerMask collidables;

    bool isValidState = true;
    public override bool inValidState() {
       return isValidState;
    }

    public override void refresh(Vector3 newInfluence) {
        preservedTime = 1 - Mathf.Pow(0.001f, Time.deltaTime);
        influence = newInfluence;

        isValidState = calculateGroundInformation() && Vector3.Angle(groundNormal, Vector3.up) < maxSlopeAngle;

        topSphere = physicsObject.position + Vector3.up * (collisionRadius + collisionHeight);
        bottomSphere = physicsObject.position + Vector3.up * collisionRadius;

        float strength = 1;
        while (!calculateNextPoint(strength)){
            strength *= 0.5f;
            if (strength < 0.01f){
                isValidState = false;
                nextPoint = physicsObject.position;
                break;
            }
        }

        Vector3 newPoint = lerpedPosition(physicsObject.position, currentVelocity, physicsObject.position, nextPoint - physicsObject.position, preservedTime);
        
        currentVelocity = (newPoint - physicsObject.position) / Time.deltaTime;
        physicsObject.position = newPoint;
    }

    Vector3 lerpedPosition(Vector3 A, Vector3 A_velocity, Vector3 B, Vector3 B_velocity, float t){
        float t0 = Mathf.Clamp01(t);
        
        Vector3 A0 = Vector3.Lerp(A, A + A_velocity, t0);
        Vector3 B0 = Vector3.Lerp(B - B_velocity, B, t0);
        Vector3 output = Vector3.Lerp(A0, B0, t0);

        return output;
    }

    bool calculateGroundInformation(){
        if (quickSphereCast(topSphere, Vector3.down, collisionHeight + collisionRadius + stepUpMax, out groundHit)){ 
                //find average "soft" normal

                groundNormal = groundHit.normal;
                return true;
            }

        return false;
    }

    bool calculateNextPoint(float strength){

        RaycastHit hit;     
        Vector3 collisionPoint;   

        if (quickSphereCast(topSphere, influence.normalized, influence.magnitude * strength, out hit)){
            collisionPoint = topSphere + influence.normalized * hit.distance;
        } else {
            collisionPoint = topSphere + influence.normalized * influence.magnitude * strength;
        }

        if (quickSphereCast(collisionPoint, Vector3.down, collisionHeight + stepUpMax, out hit)){
            if (collisionRadius + collisionHeight - hit.distance < stepUpMax){
                nextPoint = collisionPoint + Vector3.down * hit.distance;
                /*
                Checks to see if theres enough head height
                if (quickSphereCast(nextPoint + Vector3.up*collisionRadius, Vector3.up, collisionHeight + collisionRadius, out hit)){
                    if (hit.distance < minHeadHeight) return false;
                }
                */
                return true;
            } 
        }
        
        return false;
    }

    bool quickSphereCast(Vector3 P, Vector3 direction, float distance, out RaycastHit hit){
        return (Physics.SphereCast(P, collisionRadius, direction, out hit, distance, collidables));
    }

    public override void setSettings(PhysicsSettings newSettings){
        physicsSettings = newSettings;
        collisionRadius = physicsSettings.collisionRadius;
        collisionHeight = physicsSettings.collisionHeight;
        stepUpMax = physicsSettings.stepUpMax;
        maxSlopeAngle = physicsSettings.maxStandingSlopeAngle;
    }
    


}
