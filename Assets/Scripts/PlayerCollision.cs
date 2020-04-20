using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    Color rayColor = new Color(1, 0, 0);
    Vector3 pos;
    public Vector3 endLocal = new Vector3(0,1,0);
    Vector3 cont = new Vector3(0,1.5f,0);
    Vector3 norm = new Vector3(0,1,0);
    Vector3 vel = new Vector3(0, -1, 0);

    public float nDotg = 1;

    void Start()
    {
        pos = transform.position;
    }

    void FixedUpdate()
    {
        pos = transform.position;
        nDotg = Vector3.Dot(vel, norm);
        Debug.DrawLine(pos, cont, rayColor);

    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //this will give position of collision
            cont = contact.point;
            //this gives normal of surface
            norm = contact.normal;
        }
    }
}
