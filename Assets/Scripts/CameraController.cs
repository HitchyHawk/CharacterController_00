using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject cameraObject;
    PlayerController controls;

    public float radius;
    public float rads;

    public float height = 0;


    public Vector3 originPos = new Vector3(0,0,-1);
    Vector3 position;
    //for calculating collisions
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<PlayerController>();
        radius = Vector3.Magnitude(cameraObject.transform.position);
        position = originPos;
    }

    // Update is called once per frame
    /// <summary>
    /// I NEED TO CAP THE SENSITIVY IF THE CAMERA IS HELLA FAR AWAYS SO IT DOESNT SWING LIKE CRAZY!!!!!!!!!!
    /// </summary>
    void FixedUpdate()
    {
        rads = controls.theta;
        height = -controls.h;


        radius = (controls.baseRadius + radius * (controls.cameraSmooth - 1)) / controls.cameraSmooth;

        

        position[0] = originPos[0] * Mathf.Cos(-rads) - originPos[2] * Mathf.Sin(-rads);
        position[1] = height+controls.baseHeight;
        position[2] = originPos[0] * Mathf.Sin(-rads) + originPos[2] * Mathf.Cos(-rads);
        

        //originPos = new Vector3(originPos[0] * Mathf.Cos(-rads) - originPos[2] * Mathf.Sin(-rads), 0, originPos[0] * Mathf.Sin(-rads) + originPos[2] * Mathf.Cos(-rads));
        

        radius = cameraCollisionR(position, radius);
        position.x *= radius;
        position.z *= radius;

        cameraObject.transform.localEulerAngles = new Vector3(-height / (controls.heightLimit+controls.baseHeight) * controls.cameraXRot, Mathf.Rad2Deg * rads, 0);
        cameraObject.transform.localPosition = position;
    }

    float cameraCollisionR(Vector3 pos, float r)
    {

        Vector3 origin = controls.transform.position;

        RaycastHit hit;

        if (Physics.SphereCast(origin, controls.cameraSize, new Vector3(pos.x * r, 0, pos.z * r), out hit))
        {
            Debug.DrawRay(origin, pos * r, new Color(0, 1, 1));
            r = hit.distance - controls.cameraSize;
            if (r > controls.baseRadius) r = controls.baseRadius;
        }

        return r;
    }
}
