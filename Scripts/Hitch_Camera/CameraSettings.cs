using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hitch Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public float sensitivity = 1;
    [Tooltip("if the camera is farther than this then NAH Just teleport that shit\nor move it faster, it depends on the CameraActor")] 
    public float MAX_radius = 5;
    public float ideal_radius = 2;
    public ToggleType FOV;     
    public ToggleType nearClippingPlane; 
    public LayerMask cameraMask;
    [HideInInspector] public float collisionRadius;

    void OnValidate() { 
        FOV.defaultValue = Camera.VerticalToHorizontalFieldOfView(Camera.main.fieldOfView, Camera.main.aspect);
        nearClippingPlane.defaultValue = Camera.main.nearClipPlane;
        

        if (FOV.isActive) Camera.main.fieldOfView = Camera.HorizontalToVerticalFieldOfView(FOV.value, Camera.main.aspect);
        if (nearClippingPlane.isActive) Camera.main.nearClipPlane = (float)nearClippingPlane.value;

        FOV.defaultValue = Camera.VerticalToHorizontalFieldOfView(Camera.main.fieldOfView, Camera.main.aspect);
        nearClippingPlane.defaultValue = Camera.main.nearClipPlane;
        FOV.refresh();
        nearClippingPlane.refresh();

        collisionRadius = (float)nearClippingPlane / (Mathf.Cos((float)FOV * Mathf.Deg2Rad));
        updateValues(); 
    }

    

    void updateValues() {
        
    }
}
