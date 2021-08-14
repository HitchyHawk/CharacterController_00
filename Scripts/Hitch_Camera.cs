using UnityEngine;
using System.Collections;

[System.Obsolete("Not used any more", true)]
public class Hitch_Camera
{
    //Manipulation and targets
    //saved in memory as Gameobjects for convience. 
    Camera camera;
    GameObject idealObject, lookAtTarget, pivotObject;
    Hitch_CharacterVariables vars;

    //whether we are on a set track or position;
    bool onTrack = false, badCamera = true;
    Vector3 referencePoint, top, anchor;
    float preservedTime, NEAR_CLIPPING;


    public void refresh(Vector2 playerInput, Vector3 playerOffset) 
    {
        preservedTime = 1 - Mathf.Pow(0.001f, Time.deltaTime);
        lerpCamera();
        

        //we dont need to normalize for direction
        rotateCameraTowards(lookAtTarget.transform.position - camera.transform.position);
        Vector3 viewDirection = camera.transform.forward;

        Vector3 localUp = Vector3.ProjectOnPlane(Vector3.up, viewDirection).normalized;
        Vector3 localRight = Vector3.Cross(viewDirection, localUp).normalized;

        Vector3 influence = localUp * playerInput.y + localRight * playerInput.x;
        influence *= Time.deltaTime * vars.cameraSensitivity;

        Vector3 newIdealPosition = Vector3.zero;
        //possible calculate vector of movement hear to reduce duplicate code.


        if (onTrack)
        {
            onTrackMovement(influence);
        }
        else {
            newIdealPosition = orbitalMovement(influence);
            newIdealPosition += addPlayerOffset(playerOffset);

            newIdealPosition = idealObject.transform.position + collisionCheck(newIdealPosition - idealObject.transform.position);

            newIdealPosition = (pathTrace(newIdealPosition) + newIdealPosition) / 2f;
            
            if (!badCamera) newIdealPosition = idealObject.transform.position + collisionCheck(newIdealPosition - idealObject.transform.position);

            
        }

        idealObject.transform.position = newIdealPosition;
    }
    void lerpCamera()
    {
        RaycastHit hit;
        camera.transform.position = Vector3.Lerp(camera.transform.position, idealObject.transform.position, preservedTime / vars.cameraPositionSmoothness);

        Vector3 origin = idealObject.transform.position;
        Vector3 direction = camera.transform.position - origin;

        if (Physics.Raycast(origin, direction, out hit, direction.magnitude, vars.cameraMask)){
            camera.transform.position = origin + direction.normalized * (hit.distance - NEAR_CLIPPING);
        }

    }
    void onTrackMovement(Vector3 influence) 
    { 
        
    }
    Vector3 orbitalMovement(Vector3 influence) 
    {
        float radius = Mathf.Lerp(getCameraRadius(), vars.cameraRadius, preservedTime * 4);

        Vector3 ideal = idealObject.transform.position;
        Vector3 pivot = pivotObject.transform.position;
        Vector3 moddedUp = Vector3.Project(ideal - pivot, Vector3.up).normalized;
        top =  moddedUp * vars.cameraRadius + pivot;
        
        referencePoint = (ideal - top).normalized * vars.orbitalMax + top;

        //has the additional check to see if player inputs make the ideal skip the top.
        if (Vector3.Distance(ideal, top) < Vector3.Distance(referencePoint, top)){
            ideal = referencePoint;
        } else if (intersectsWithSphere(ideal, ideal + influence, top, vars.orbitalMax)){
            
            influence = Vector3.ProjectOnPlane(influence, ideal - top);
        } 
        ideal += influence;
        ideal = (ideal - pivot).normalized * radius + pivot;

        return ideal;
    }
    Vector3 addPlayerOffset(Vector3 offset)
    {
        return offset * Time.deltaTime;
    }
    Vector3 collisionCheck(Vector3 direction)
    {
        
        Vector3 output = direction;
        RaycastHit hit;

        //same size as near clipping plane * sqrt(2)
        if (Physics.SphereCast(idealObject.transform.position, NEAR_CLIPPING, output, out hit, output.magnitude, vars.cameraMask)){
            output = Vector3.ProjectOnPlane(output, hit.normal);

            if (Physics.SphereCast(idealObject.transform.position, NEAR_CLIPPING, output, out hit, output.magnitude, vars.cameraMask)){
                output = Vector3.Reflect(output, hit.normal);
            }
        }
        return output;

    }
    Vector3 pathTrace(Vector3 currentPosition)
    {
        //if (anchors.Count == 0) anchors.Add(pivotObject.transform.position);
        float tracedRadius = 0;
        Vector3 pivot = pivotObject.transform.position;
        RaycastHit hit;
        bool didHit = false;
        (didHit, hit) = quickSpherecast(pivot, currentPosition, NEAR_CLIPPING);
        if (didHit && hit.distance < vars.cameraRadius){

            (didHit, hit) = quickRaycast(pivot, anchor);

            if (didHit){
                //was not a safe anchor
                currentPosition = (anchor - pivot).normalized * hit.distance + pivot;
                anchor = pivot;
                badCamera = true;
            }else{

                badCamera = false;
                tracedRadius = Vector3.Distance(pivot, anchor);
                if (tracedRadius > vars.cameraRadius){
                    currentPosition = (anchor - pivot).normalized * vars.cameraRadius + pivot;
                    anchor = pivot;
                    badCamera = false;
                    return currentPosition;
                }

                Vector3 idealNew = (currentPosition - anchor).normalized * (vars.cameraRadius - tracedRadius) + anchor;

                (didHit, hit) = quickSpherecast(anchor, idealNew, NEAR_CLIPPING);
                if (didHit){
                    currentPosition = (currentPosition - anchor).normalized * hit.distance + anchor;
                }else{
                    (didHit, hit) = quickRaycast(anchor, currentPosition);
                    if (didHit){
                        idealNew = (currentPosition - anchor).normalized*(hit.distance - NEAR_CLIPPING) + anchor;
                        badCamera = true;
                    }

                    currentPosition = idealNew;
                }
            }
            
        }else{
            //all clear
            badCamera = false;
            anchor = pivotObject.transform.position;
        }

        return currentPosition;
    }
    void rotateCameraTowards(Vector3 idealDirection) 
    { 
        Vector3 currentDirection = camera.transform.forward;
        float x = camera.transform.eulerAngles.x;
        float y = camera.transform.eulerAngles.y;

        float yaw = Vector3.SignedAngle( Vector3.ProjectOnPlane(currentDirection, Vector3.up).normalized,
                                         Vector3.ProjectOnPlane(idealDirection, Vector3.up).normalized,
                                         Vector3.up);

        float pitch = Vector3.SignedAngle(  Vector3.ProjectOnPlane(currentDirection, Vector3.Cross(Vector3.up, idealDirection)).normalized,
                                            idealDirection.normalized,
                                            Vector3.Cross(Vector3.up, idealDirection));

        camera.transform.eulerAngles = new Vector3( Mathf.LerpAngle(x, x + pitch, preservedTime / vars.cameraAngleSmoothness), 
                                                    Mathf.LerpAngle(y, y + yaw  , preservedTime / vars.cameraAngleSmoothness),
                                                    0);
    }
    bool intersectsWithSphere(Vector3 p1, Vector3 p2, Vector3 p3, float radius)
    {
        if (Vector3.Dot((p2 - p1).normalized, (p3 - p1).normalized) <= 0) return false;

        float a = Mathf.Pow(p2.x - p1.x,2) + Mathf.Pow(p2.y - p1.y, 2) + Mathf.Pow(p2.z - p1.z, 2);
        float b = 2 * ((p2.x - p1.x) * (p1.x - p3.x) + (p2.y - p1.y) * (p1.y - p3.y) + (p2.z - p1.z) * (p1.z - p3.z));
        float c = (p3.x * p3.x + p3.y * p3.y + p3.z * p3.z) + (p1.x * p1.x + p1.y * p1.y + p1.z * p1.z) - 2 * (p3.x * p1.x + p3.y * p1.y + p3.z * p1.z) - radius * radius;

        return (b * b - 4 * a * c > 0);
    }
    float getCameraRadius(){
        return Vector3.Distance(camera.transform.position, pivotObject.transform.position);
    }
    private (bool, RaycastHit) quickRaycast(Vector3 origin, Vector3 destination)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 direction = destination - origin;
        if (Physics.Raycast(origin,direction, out hit, direction.magnitude, vars.cameraMask)){
            return (true, hit);
        }

        return (false, hit);
    }
    private (bool, RaycastHit) quickSpherecast(Vector3 origin, Vector3 destination, float radius)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 direction = destination - origin;
        if (Physics.SphereCast(origin, radius, direction, out hit, direction.magnitude, vars.cameraMask)){
            return (true, hit);
        }

        return (false, hit);
    }
    public void setTarget(GameObject target)
    {
        lookAtTarget = target;
    }
    public void setIdeal(GameObject ideal)
    {
        idealObject = ideal;
    }
    public void setCamera(Camera cam)
    {
        camera = cam;
        NEAR_CLIPPING = camera.nearClipPlane * Mathf.Sqrt(2) * (1 + camera.focalLength / 90f);
    }
    public void setPivot(GameObject newPivot) 
    {
        pivotObject = newPivot;
    }
    public void setVars(Hitch_CharacterVariables var)
    {
        vars = var;
    }
    public Vector3 localViewAxis(Vector2 input)
    {
        Vector3 forwards = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
        Vector3 left = Vector3.Cross(Vector3.up, forwards).normalized;

        return input.x * left + input.y * forwards;
    }
    public Vector3 getRef() 
    {
        Vector3 output = referencePoint;
        return output;
    }
    public Vector3 getTop()
    {
        Vector3 output = top;
        return top;

    }
    public Vector3 getAnchor()
    {
        Vector3 output = anchor;
        return anchor;
    }
}
