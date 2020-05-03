using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialMaths;

public class CameraPlayerCue : MonoBehaviour
{
    PlayerController controls;

    public AngleSpace theta;
    public AngleSpace phi;

    [HideInInspector]public Vector3 rotation = Vector3.zero; 

    public Vector3 position;
    [HideInInspector]public float radius = 0;

    void Start() {
        theta = new AngleSpace(0, Mathf.PI * 2, 0, true);
        phi = new AngleSpace(0, Mathf.PI * 0.5f, -Mathf.PI * 0.5f, false);
        controls = GetComponent<PlayerController>();
    }

    void Update()
    {
        radius = controls.cameraRadius;
        //Debug.Log(theta.GetFloat());
        theta.AddAngle  (Mathf.Atan(controls.camSpeed.x/radius)   * controls.cameraSensitivity);
        phi.AddAngle    (Mathf.Atan(controls.camSpeed.y / radius) * controls.cameraSensitivity);

        //so the camera doesnt rotate upside down and slows down when it approaches the top;
        // used arctan(controls.cam""Speed/ controls.cameraBgR) but I think I was having floating point errors with it.

        position = cameraCollision(new Vector3( Mathf.Sin(phi.GetFloat() - Mathf.PI * 0.5f) * Mathf.Sin(theta.GetFloat()), 
                                                Mathf.Cos(phi.GetFloat() - Mathf.PI * 0.5f), 
                                                Mathf.Sin(phi.GetFloat() - Mathf.PI * 0.5f) * Mathf.Cos(theta.GetFloat())) * radius + controls.cameraOffset);
        rotation.x = phi.GetFloat();
        rotation.y = theta.GetFloat();
    }

    Vector3 cameraCollision(Vector3 direction) {
        RaycastHit hit;

        if (Physics.SphereCast(controls.transform.position, 0.5f, direction, out hit, Vector3.Magnitude(direction),controls.collisionMask))
            return Vector3.Normalize(direction) * hit.distance;
        else return direction;
    }

    public void SetTheta(float a2) {
        theta.SetTheta(a2);
    }
    public void SetPhi(float a1) {
        phi.SetTheta(a1);
    }
}
