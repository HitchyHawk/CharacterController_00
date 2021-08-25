using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraImpl : MonoBehaviour, CameraActor
{
    protected Camera cam;
    protected Transform _lookAt, _pivot, _secondary;
    protected float sensitivity;
    static protected float collisionRadius, hFOV;

    protected LayerMask cameraMask;


    
    ///<summary> Override if on a track AND SAY SO!!<summary>
    public virtual bool isTrack() => false;

    //made virtuall so it can be overriden
    public virtual void refresh(Vector2 newInfluence) {
        throw new System.NotImplementedException();
    }
    public virtual void setCamera(Camera newCamera) => cam = newCamera;
    public virtual void setCameraSettings(CameraSettings newSettings){
        //If naming convention changes, no heavy changes are needed.
        collisionRadius = newSettings.collisionRadius;
        sensitivity = newSettings.sensitivity;
        hFOV = (float)newSettings.FOV;
        cameraMask = newSettings.cameraMask;
    }
    public virtual void setLookAt(Transform newLookAt) => _lookAt = newLookAt;
    public virtual void setPivot(Transform newPivot) => _pivot = newPivot;
    public virtual void setSecondary(Transform newSecondary) => _secondary = newSecondary;
    
    ///<summary>
    ///Rotates the screen vectors to match the view and normal plane.
    ///</summary>
    public Vector3 transformToView(Vector2 newDirection, Vector3 normal)
    {
        Vector3 forwards = Vector3.ProjectOnPlane(cam.transform.forward, normal).normalized;
        Vector3 left = Vector3.Cross(normal, forwards).normalized;

        return newDirection.x * left + newDirection.y * forwards;
    }

    ///<summary>
    ///rotates the cameras forward direction to the ideal direction, this is done by lerping with smoothness
    ///</summary>
    ///<param name="t">used to lerp from current direction to idealDirection</param>
    protected void rotateLerpCamera(Vector3 idealDirection, float t)  { 
        Vector3 currentDirection = cam.transform.forward;
        float x = cam.transform.eulerAngles.x;
        float y = cam.transform.eulerAngles.y;

        float yaw = Vector3.SignedAngle( Vector3.ProjectOnPlane(currentDirection, Vector3.up).normalized,
                                         Vector3.ProjectOnPlane(idealDirection, Vector3.up).normalized,
                                         Vector3.up);

        float pitch = Vector3.SignedAngle(  Vector3.ProjectOnPlane(currentDirection, Vector3.Cross(Vector3.up, idealDirection)).normalized,
                                            idealDirection.normalized,
                                            Vector3.Cross(Vector3.up, idealDirection));

        cam.transform.eulerAngles = new Vector3( Mathf.LerpAngle(x, x + pitch, t), 
                                                 Mathf.LerpAngle(y, y + yaw  , t),
                                                 0);
    }

    protected float getCameraRadius() => Vector3.Distance(cam.transform.position, _pivot.position);

    public virtual void teleportCamera()
    {
        throw new System.NotImplementedException();
    }
}
