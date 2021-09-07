using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hitch Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public float sensitivity = 1;
    public ToggleType FOV;     
    public ToggleType nearClippingPlane; 
    public LayerMask cameraMask;
    [HideInInspector] public float collisionRadius;

    void OnValidate() { 
        /*
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
        */
    }

    

    void updateValues() {
        
    }
}
