using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitch_CameraPlayer : CameraImpl
{
    float preservedTime = 0, smoothness = 0.5f, pSmoothness = 0.7f;
    bool badCamera = true;
    
    Vector3 ideal, anchor;
    public override void refresh(Vector2 newInfluence) {
        preservedTime = 1 - Mathf.Pow(0.001f, Time.deltaTime);
        lerpCamera();
        

        //we dont need to normalize for direction
        rotateLerpCamera(_lookAt.position - cam.transform.position, preservedTime / smoothness);
        Vector3 viewDirection = cam.transform.forward;

        Vector3 localUp = Vector3.ProjectOnPlane(Vector3.up, viewDirection).normalized;
        Vector3 localRight = Vector3.Cross(viewDirection, localUp).normalized;

        Vector3 influence = localUp * newInfluence.y + localRight * newInfluence.x;
        influence *= Time.deltaTime * sensitivity;

        Vector3 newIdealPosition = Vector3.zero;
        //possible calculate vector of movement hear to reduce duplicate code.
        newIdealPosition = orbitalMovement(influence);
        //newIdealPosition += addPlayerOffset(newInfluence);

        newIdealPosition = ideal + collisionCheck(newIdealPosition - ideal);
        newIdealPosition = (pathTrace(newIdealPosition) + newIdealPosition) / 2f;
        
        if (!badCamera) newIdealPosition = ideal + collisionCheck(newIdealPosition - ideal);

   
        ideal = newIdealPosition;
    }
    void lerpCamera()
    {
        RaycastHit hit;
        cam.transform.position = Vector3.Lerp(cam.transform.position, ideal, preservedTime / pSmoothness);

        Vector3 origin = ideal;
        Vector3 direction = cam.transform.position - origin;

        if (Physics.Raycast(origin, direction, out hit, direction.magnitude, cameraMask)){
            cam.transform.position = origin + direction.normalized * (hit.distance - collisionRadius);
        }

    }
    Vector3 orbitalMovement(Vector3 influence) 
    {
        float radius = Mathf.Lerp(getCameraRadius(), ideal_radius, preservedTime * 4);

        Vector3 pivot = _pivot.position;
        Vector3 moddedUp = Vector3.Project(ideal - pivot, Vector3.up).normalized;
        Vector3 top =  moddedUp * ideal_radius + pivot;
        Vector3 referencePoint = (ideal - top).normalized * ideal_radius + top;

        //has the additional check to see if player inputs make the ideal skip the top.
        if (Vector3.Distance(ideal, top) < Vector3.Distance(referencePoint, top)){
            ideal = referencePoint;
        } else if (intersectsWithSphere(ideal, ideal + influence, top, ideal_radius)){
            
            influence = Vector3.ProjectOnPlane(influence, ideal - top);
        } 
        ideal += influence;
        ideal = (ideal - pivot).normalized * radius + pivot;

        return ideal;
    }
    Vector3 pathTrace(Vector3 currentPosition) {
        //if (anchors.Count == 0) anchors.Add(pivotObject.transform.position);
        float tracedRadius = 0;
        Vector3 pivot = _pivot.position;
        RaycastHit hit;
        bool didHit = false;
        (didHit, hit) = quickSpherecast(pivot, currentPosition, collisionRadius);
        if (didHit && hit.distance < ideal_radius){

            (didHit, hit) = quickRaycast(pivot, anchor);

            if (didHit){
                //was not a safe anchor
                currentPosition = (anchor - pivot).normalized * hit.distance + pivot;
                anchor = pivot;
                badCamera = true;
            }else{

                badCamera = false;
                tracedRadius = Vector3.Distance(pivot, anchor);
                if (tracedRadius > ideal_radius){
                    currentPosition = (anchor - pivot).normalized * ideal_radius + pivot;
                    anchor = pivot;
                    badCamera = false;
                    return currentPosition;
                }

                Vector3 idealNew = (currentPosition - anchor).normalized * (ideal_radius - tracedRadius) + anchor;

                (didHit, hit) = quickSpherecast(anchor, idealNew, collisionRadius);
                if (didHit){
                    currentPosition = (currentPosition - anchor).normalized * hit.distance + anchor;
                }else{
                    (didHit, hit) = quickRaycast(anchor, currentPosition);
                    if (didHit){
                        idealNew = (currentPosition - anchor).normalized*(hit.distance - collisionRadius) + anchor;
                        badCamera = true;
                    }

                    currentPosition = idealNew;
                }
            }
            
        }else{
            //all clear
            badCamera = false;
            anchor = _pivot.position;
        }

        return currentPosition;
    }

    Vector3 collisionCheck(Vector3 direction)
    {
        
        Vector3 output = direction;
        RaycastHit hit;

        //same size as near clipping plane * sqrt(2)
        if (Physics.SphereCast(ideal, collisionRadius, output, out hit, output.magnitude, cameraMask)){
            output = Vector3.ProjectOnPlane(output, hit.normal);

            if (Physics.SphereCast(ideal, collisionRadius, output, out hit, output.magnitude, cameraMask)){
                output = Vector3.Reflect(output, hit.normal);
            }
        }
        return output;

    }
    private (bool, RaycastHit) quickSpherecast(Vector3 origin, Vector3 destination, float radius) {
        RaycastHit hit = new RaycastHit();
        Vector3 direction = destination - origin;
        if (Physics.SphereCast(origin, radius, direction, out hit, direction.magnitude, cameraMask)){
            return (true, hit);
        }

        return (false, hit);
    }
    private (bool, RaycastHit) quickRaycast(Vector3 origin, Vector3 destination) {
        RaycastHit hit = new RaycastHit();
        Vector3 direction = destination - origin;
        if (Physics.Raycast(origin,direction, out hit, direction.magnitude, cameraMask)){
            return (true, hit);
        }

        return (false, hit);
    }
    bool intersectsWithSphere(Vector3 p1, Vector3 p2, Vector3 p3, float radius) {
        if (Vector3.Dot((p2 - p1).normalized, (p3 - p1).normalized) <= 0) return false;

        float a = Mathf.Pow(p2.x - p1.x,2) + Mathf.Pow(p2.y - p1.y, 2) + Mathf.Pow(p2.z - p1.z, 2);
        float b = 2 * ((p2.x - p1.x) * (p1.x - p3.x) + (p2.y - p1.y) * (p1.y - p3.y) + (p2.z - p1.z) * (p1.z - p3.z));
        float c = (p3.x * p3.x + p3.y * p3.y + p3.z * p3.z) + (p1.x * p1.x + p1.y * p1.y + p1.z * p1.z) - 2 * (p3.x * p1.x + p3.y * p1.y + p3.z * p1.z) - radius * radius;

        return (b * b - 4 * a * c > 0);
    }
}
