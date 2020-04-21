using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerController controls;
    Color rayColor = new Color(1, 0, 0);
    Vector3 pos;
    public Vector3 endLocal = new Vector3(0,1,0);
    Vector3 cont = new Vector3(0,1.5f,0);
    Vector3 norm = new Vector3(0,1,0);
    Vector3 vel = new Vector3(0, -1, 0);

    Vector3 debugRay;

    public float nDotg = 1;

    public Collider coll;

    void Start()
    {
        controls = GetComponent<PlayerController>();
        pos = transform.position;
        coll = GetComponent<Collider>();
        coll.attachedRigidbody.useGravity = true;
    }

    void FixedUpdate()
    {
        pos = transform.position;
        nDotg = Vector3.Dot(vel, norm);
        Debug.DrawLine(pos, pos+2*cont, rayColor);

    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //this will give position of collision
            cont = contact.point-pos;
            //this gives normal of surface
            norm = contact.normal;
        }
        
    }

    private void OnCollisionEnter() { coll.attachedRigidbody.useGravity = false; }
    private void OnCollisionExit() { coll.attachedRigidbody.useGravity = true;  }
  

}
