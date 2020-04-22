using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    Color rayColor = new Color(1, 0, 0);
    PlayerController controls;
    Collider coll;

    
    public float grav = 1;
    public float collisionRes = 5;
    public float rayOffset = 0.5f;

    private float spacing;

    public Vector3 velocity = new Vector3(0,0,0);
    private Vector3 position;

    void Start()
    {
        controls = GetComponent<PlayerController>();
        coll = GetComponent<Collider>();
        spacing = 2 * rayOffset / collisionRes;
    }

    void FixedUpdate()
    {
        position = transform.position;

        velocity += Gravity(velocity.y);
        transform.position += velocity;
           
    }

    Vector3 Gravity(float y) {
        RaycastHit hit;
        Ray ray;
        Vector3 origin = position + new Vector3(-1,-1,-1)* rayOffset;
        float yMag = Mathf.Abs(y) + grav*Time.deltaTime;
        float smallestValue = yMag;
        
        for (int i = 0; i <= collisionRes; i++) {
            for (int j = 0; j <= collisionRes; j++) {
                ray = new Ray(origin + (new Vector3(j, 0, i)*spacing), Vector3.down);
                Physics.Raycast(ray, out hit);
                if (hit.distance < smallestValue) smallestValue = hit.distance;

                Debug.DrawRay (origin + (new Vector3(j, 0, i) * spacing), Vector3.down, rayColor);
            }
        }
        Debug.Log("smallest Value: "+ smallestValue + "\t yMag:" + yMag);

        if (smallestValue < 0.001f)
        {
            return Vector3.down * -1 * Mathf.Abs(y);
        }
        else if (smallestValue != yMag) {
            return Vector3.down * (smallestValue - yMag);
        }

        return Vector3.down * grav*Time.deltaTime;
    }
    
    Vector3 Velocity()
}
