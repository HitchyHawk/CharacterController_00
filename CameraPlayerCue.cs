using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerCue : MonoBehaviour
{
    PlayerController controls;
    public float theta = 0; //y rot
    public float phi = 0; //x rot

    [Range(0, Mathf.PI * 0.5f)]
    public float phiCutOff = Mathf.PI * 0.5f - 0.1f;

    public Vector3 position;

    public float radius = 0;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    /// <summary>
    /// I NEED TO CAP THE SENSITIVY IF THE CAMERA IS HELLA FAR AWAYS SO IT DOESNT SWING LIKE CRAZY!!!!!!!!!!
    /// </summary>
    void Update()
    {
        radius = controls.cameraRadius;
        theta += Mathf.Atan(controls.camSpeed.x/radius) * controls.cameraSensitivity;


        //so the camera doesnt rotate upside down and slows down when it approaches the top;
        // used arctan(controls.cam""Speed/ controls.cameraBgR) but I think I was having floating point errors with it.
        if (phi * controls.camSpeed.y > 0)  phi += (Mathf.Pow(phi / (phiCutOff), 10) + 1) * Mathf.Atan(controls.camSpeed.y / radius) * controls.cameraSensitivity;
        else                                phi +=                                          Mathf.Atan(controls.camSpeed.y / radius) * controls.cameraSensitivity;

        if (phi > phiCutOff)        phi = phiCutOff;
        else if (phi < -phiCutOff)  phi = -phiCutOff;

        position = cameraCollision(new Vector3( Mathf.Sin(phi-Mathf.PI * 0.5f) * Mathf.Sin(theta), 
                                                Mathf.Cos(phi-Mathf.PI * 0.5f), 
                                                Mathf.Sin(phi-Mathf.PI * 0.5f) * Mathf.Cos(theta)) * radius + controls.cameraOffset);

    }
    Vector3 cameraCollision(Vector3 direction) {
        RaycastHit hit;

        if (Physics.SphereCast(controls.transform.position, 0.5f, direction, out hit, Vector3.Magnitude(direction)))
            return Vector3.Normalize(direction) * hit.distance;
        else return direction;
    }
}
