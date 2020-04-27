using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject cameraObject;
    PlayerController controls;
    public Vector3 cameraDirection = new Vector3(0, 0, -1);
    public float theta = 0; //y rot
    public float phi   = 0; //x rot

    private float preTheta = 0;
    private float prePhi   = 0;

    [Range(-7,7)]
    public float thetaRate;
    [Range(-7,7)]
    public float phiRate;

    [Range(0,Mathf.PI*0.5f)]
    public float phiCutOff = Mathf.PI * 0.5f - 0.1f;

    private Vector3 direction;
    private Vector3 position ;

    public float radius = 0;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<PlayerController>();
        cameraObject.transform.localPosition = position = controls.cameraOffset;
    }

    // Update is called once per frame
    /// <summary>
    /// I NEED TO CAP THE SENSITIVY IF THE CAMERA IS HELLA FAR AWAYS SO IT DOESNT SWING LIKE CRAZY!!!!!!!!!!
    /// </summary>
    void Update()
    {
        preTheta = theta;
        prePhi = phi;
        theta += controls.camHSpeed / controls.cameraBgR;
        if (Mathf.Abs(theta) > 2 * Mathf.PI) theta -= 2 * Mathf.PI * theta / Mathf.Abs(theta);  //to minimize floating point errors
        
        //so the camera doesnt rotate upside down and slows down when it approaches the top;
        // used arctan(controls.cam""Speed/ controls.cameraBgR) but I think I was having floating point errors with it.
        if (phi * controls.camVSpeed > 0 )  phi +=  (Mathf.Pow(phi / (phiCutOff), 10) +1 ) * controls.camVSpeed / controls.cameraBgR;
        else                                phi +=                                           controls.camVSpeed / controls.cameraBgR;

        if (phi > phiCutOff) phi = phiCutOff;
        else if (phi < -phiCutOff) phi = -phiCutOff;


        radius = (controls.cameraSmllR + radius * (controls.cameraSmooth - 1)) / controls.cameraSmooth;

        direction = new Vector3(Mathf.Sin(phi) * Mathf.Sin(theta), Mathf.Cos(phi), Mathf.Sin(phi) * Mathf.Cos(theta));

        position = new Vector3(Mathf.Sin(phi) * Mathf.Sin(theta), Mathf.Cos(phi), Mathf.Sin(phi) * Mathf.Cos(theta)) * radius;
        position.x += controls.cameraOffset[0] * Mathf.Cos(-theta) - controls.cameraOffset[2] * Mathf.Sin(-theta);
        position.z += controls.cameraOffset[0] * Mathf.Sin(-theta) + controls.cameraOffset[2] * Mathf.Cos(-theta);
        cameraObject.transform.eulerAngles = new Vector3(phi, theta, 0)*Mathf.Rad2Deg;
        cameraObject.transform.localPosition = position;

        Debug.DrawRay(cameraObject.transform.position, direction*4, new Color(0, 0, 1));
        thetaRate = (theta - preTheta) / Time.deltaTime;
        phiRate = (phi - prePhi) / Time.deltaTime;
    }

}
