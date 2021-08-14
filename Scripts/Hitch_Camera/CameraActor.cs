using System;
using UnityEngine;
///<summary>  
///A camera interface which holds all required methods.
///To be used for more wide spread and standardized use, while remaining flexible
///
///</summary>
public interface CameraActor {
    void refresh(Vector2 newInfluence);
    void setCamera(Camera newCamera);
    
    void setLookAt(Transform newLookAt);
    void setPivot(Transform newPivot);
    void setSecondary(Transform newSecondary);

    void setCameraSettings(CameraSettings newSettings);
    Vector3 transformToView(Vector2 newDirection, Vector3 normal);

    bool isTrack();
} 
