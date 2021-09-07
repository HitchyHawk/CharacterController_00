using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : CameraImpl {

    float preservedTime = 0;
    [Range(0.001f, 5)] public float smoothness = 0.1f;
    [Range(0.001f, 5)] public float pSmoothness = 0.7f;
    [Range(0, 90)] public float restrictedDegrees = 10;
    [Range(0, 16)] public float MAX_Radius;
    [Range(0, 16)] public float ideal_radius;
    [Range(0, 4)] public float idealDistanceFromGround = 1.2f;
    float radius;
    [Range(1, 64)] public int avoidanceRays = 16;
    [Range(0.01f, 3)] public float avoidanceScan = 1.2f;
    
    static float turningFraction = (1 + Mathf.Sqrt(5)) / 2f;
    Vector3 prevIdeal, newIdeal, prevPivot, prevVelocity = Vector3.zero;
    Vector3 safeIdeal;

    Vector2 avoidanceOffset = Vector2.zero;

    
    public override void refresh(Vector2 newInfluence){
        preservedTime = 1 - Mathf.Pow(0.001f, Time.deltaTime);

        _pivot.position = new Vector3(_secondary.position.x, Mathf.Lerp(_pivot.position.y, _secondary.position.y + 2, preservedTime / pSmoothness), _secondary.position.z);
        _lookAt.position = _pivot.position + Vector3.up*0.5f;
        Vector3 offset = _pivot.position - prevPivot;
        prevIdeal += offset;
        //pivot already has offset.

        avoidanceOffset = obstructionInfluence(prevIdeal, _pivot.position)*0;
        if (newInfluence.magnitude > 0.25f)  avoidanceOffset = Vector2.zero;

        Vector2 totalInfluence = newInfluence + avoidanceOffset;

        Vector3 orbitOffset = orbitalInfluence(prevIdeal, _pivot.position, totalInfluence * Time.deltaTime * sensitivity, false); //users influence (want to go)
        newIdeal = prevIdeal + orbitOffset;


        //everything is set in motion, move the camera to the new point. 
        lerpCamera();
        rotateLerpCamera(_lookAt.position - cam.transform.position, preservedTime / smoothness);

        prevIdeal = newIdeal;
        prevPivot = _pivot.position;

    }

    
    Vector2 obstructionInfluence(Vector3 startPosition, Vector3 orbitPivot){
        RaycastHit hit;
        Vector3 p0 = startPosition - orbitPivot; //This also makes p0 our ideal location a direction from orbit to ideal.
        Vector2 averageInfluenceDirection = Vector2.zero;
        float offTheGroundInfluence = 0;
        Vector2 options;
        float rayDistance = p0.magnitude;
        Vector3 rayOffset;

        Vector3 X = Vector3.Cross(-p0, Vector3.up).normalized,
                Y = Vector3.Cross(X, -p0).normalized;

        int hits = 0;

        for (int i = 1; i <= avoidanceRays; i++){
            options = pointDistribution(i, avoidanceRays);
            options.y *= 0.25f;
            

            rayOffset = (X * options.x + Y * options.y) * avoidanceScan * ideal_radius;

            if (!Physics.Raycast(orbitPivot, p0 + rayOffset, out hit, rayDistance, cameraMask)){
                hits++;
                averageInfluenceDirection.x += 2 * options.x;
                //Debug.DrawLine(orbitPivot, orbitPivot + p0 + rayOffset, Color.green);
            } else{
                averageInfluenceDirection.x -= options.x;
                //Debug.DrawLine(orbitPivot, orbitPivot + p0 + rayOffset, Color.red);
            }
        }

        /*
        if (Physics.Raycast(startPosition, Vector3.down, out hit, 2*idealDistanceFromGround, cameraMask)){
            offTheGroundInfluence = idealDistanceFromGround - hit.distance;
        } else {

        }
        */

        

        if (hits > 0 && hits != avoidanceRays){
            averageInfluenceDirection /= (float)avoidanceRays;

            return averageInfluenceDirection * 2 + new Vector2(0, offTheGroundInfluence) / 4f;
        }

        return new Vector2(Mathf.Lerp(avoidanceOffset.x, 0, preservedTime / 5f), Mathf.Lerp(offTheGroundInfluence, 0, preservedTime));
        //return Vector2.Lerp(avoidanceOffset, Vector2.zero, preservedTime / 5f);
    }
    Vector3 orbitalInfluence(Vector3 startPosition, Vector3 orbitPivot, Vector2 influence, bool userOverride){
        //center everything on zero zero, for easier calculations
        Vector3 p0 = startPosition - orbitPivot,
                o0 = Vector3.zero;
        

        Vector3 X0 = Vector3.Cross(-p0, Vector3.up).normalized,
                Y0 = Vector3.Cross(X0, -p0).normalized,
                X1 = Vector3.Cross(cam.transform.forward, Vector3.up).normalized,
                Y1 = Vector3.Cross(X1, cam.transform.forward).normalized,
                X = (0*X0 + X1).normalized,
                Y = (0*Y0 + Y1).normalized;
                

        Vector3 rawOrbit = p0 + Y*influence.y;

        float xStrength = 1 - Vector3.Dot(rawOrbit.normalized, Vector3.ProjectOnPlane(rawOrbit, Vector3.up).normalized);
        xStrength = Mathf.Max(1 - Mathf.Pow(xStrength,2) , 0.1f);
        rawOrbit += X*influence.x * xStrength;

        float angleFromHorizontal = Vector3.Angle(Vector3.ProjectOnPlane(rawOrbit, Vector3.up), rawOrbit);
        if (userOverride){
            //raycast to adjust radius as needed. 
        } else {
             radius = Mathf.Pow(angleFromHorizontal / (90 - restrictedDegrees), 2) * (MAX_Radius - ideal_radius) + ideal_radius;
        }
        
        if (Vector3.Angle(Vector3.up, rawOrbit) < restrictedDegrees)          rawOrbit -= Y * (influence.y != 0? influence.y :  sensitivity * Time.deltaTime);
        else if ((Vector3.Angle(Vector3.down, rawOrbit) < restrictedDegrees)) rawOrbit -= Y * (influence.y != 0? influence.y : -sensitivity * Time.deltaTime);

        Vector3 orbitPosition = rawOrbit.normalized * radius;

        if (orbitPosition.magnitude > MAX_Radius) orbitPosition = rawOrbit.normalized * MAX_Radius;

        return orbitPosition - p0;
    }

    void calculateRadius(){
        RaycastHit hit;
        Vector3 direction = newIdeal - _pivot.position;

        if (Physics.SphereCast(_pivot.position, collisionRadius*3, direction, out hit, ideal_radius, cameraMask)){
            if (Physics.SphereCast(_pivot.position, collisionRadius, direction, out hit, hit.distance + collisionRadius*3, cameraMask)){
                radius = hit.distance;
            } else{
                radius = hit.distance + collisionRadius*5;  
            }
        }
    }
    void lerpCamera(){
        cam.transform.position = Vector3.Lerp(cam.transform.position, newIdeal, preservedTime / pSmoothness);
        {
        /*
        float maxDistance = Mathf.Sqrt(2) * ideal_radius;
        float weightedStrength = Mathf.Pow(1 - Mathf.Clamp(Vector3.Distance(cam.transform.position, newIdeal), 0 , maxDistance) / maxDistance, 0.5f); // 1 distance is zero, 0 when distance is max.
        weightedStrength = Mathf.Clamp(weightedStrength, 0.1f, 1);

        Vector3 lerpedPosition = Vector3.Lerp(cam.transform.position, newIdeal, preservedTime / pSmoothness);
        Vector3 velocityPosition = cam.transform.position + prevVelocity;

        Vector3 weightedPosition = lerpedPosition * weightedStrength + velocityPosition * (1 - weightedStrength);
        prevVelocity = weightedPosition - cam.transform.position;

        cam.transform.position = weightedPosition;
        */
        /*
        RaycastHit hit;
        Vector3 direction = newIdeal - cam.transform.position;

        if (Physics.Raycast(cam.transform.position, direction, out hit, direction.magnitude, cameraMask)){
            
            //if the safe is also compromised, just teleport to the newIdeal 
            if (Physics.Raycast(newIdeal, safeIdeal - newIdeal, (safeIdeal - newIdeal).magnitude, cameraMask)){
                cam.transform.position = newIdeal;
                newIdeal = safeIdeal;
                return;
            }

            //then string from newIdeal to safe Ideal. then safe ideal to camera.
            float distanceS2N = Vector3.Distance(newIdeal, safeIdeal);

            if (distanceS2N > direction.magnitude) cam.transform.position = newIdeal  + (safeIdeal -              newIdeal ).normalized * direction.magnitude;
            else                                   cam.transform.position = safeIdeal + (cam.transform.position - safeIdeal).normalized * (direction.magnitude - distanceS2N);
            
        } 
        else {
            cam.transform.position = Vector3.Lerp(cam.transform.position, newIdeal, preservedTime / pSmoothness);
            safeIdeal = newIdeal;
        }
        */
        }
    }
    
    float linearPointDistribution(int n, int max){
        return 2 * n / ((float) max) - 1;
    }
    Vector2 pointDistribution(int n, int max){
        float v = 2 * Mathf.PI * turningFraction * n;
        float distance = Mathf.Pow(n / ((float)max), 0.25f);
        float x = Mathf.Cos(v);
        float y = Mathf.Sin(v);

        return new Vector2(x, y)*distance;
    }
    public override void setCamera(Camera newCamera)
    {
        base.setCamera(newCamera);
        prevIdeal = newCamera.transform.position;
    }

    public override void teleportCamera(){
        cam.transform.position = newIdeal;
    }
}
