using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    Color rayColor = new Color(1, 0, 0);
    PlayerController controls;
    Collider coll;

    public float speed = 1;
    public float jumpSpeed = 2;
    public float drag = 0.2f;
    public float grav = 1;
    public float collisionRes = 5;
    public float rayOffset = 0.5f;
    public float rayHeight = 0.75f;

    public bool isGrounded = false;

    public Vector3 velocity = new Vector3(0,0,0);
    private Vector3 position;

    void Start()
    {
        controls = GetComponent<PlayerController>();
        coll = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        position = transform.position;
        if (Vector3.Magnitude(controls.HoVeInput) != 0)
        {
            velocity += (controls.HoVeInput * speed - new Vector3(velocity.x, 0, velocity.z) * drag) * Time.deltaTime;
        }
        else { 
            velocity += ( -new Vector3(velocity.x, 0, velocity.z) * drag) * Time.deltaTime;
        }

 
        velocity += Gravity(velocity.y);
        velocity = CollisionVelocityAdjustment(velocity);

        if (controls.isJump && isGrounded){
            isGrounded = false;
            velocity += Vector3.up * jumpSpeed;
        }

        Debug.DrawRay(position, velocity * 10, new Color(1, 0, 0));

        //Debug.DrawRay(position, velocity*10, new Color(0, 1, 0));



        transform.position += velocity;
           
    }

    Vector3 Gravity(float y) {
        RaycastHit hit;
        Ray ray;
        Vector3 origin;
        float yPrime = y - grav * Time.deltaTime;
        float length = Mathf.Abs(yPrime);
        float smallestValue = length;
            
        origin = position + rayHeight * Vector3.up*yPrime/length;

        for (int i = 1; i < collisionRes+1; i++) {
            for (int j = 0; j <= i*i; j++) {
                float p1 = Mathf.Cos(2 * Mathf.PI * j / (i*i)) * rayOffset * (i-1) / collisionRes;
                float p2 = Mathf.Sin(2 * Mathf.PI * j / (i*i)) * rayOffset * (i-1) / collisionRes;

                Debug.DrawRay(origin + new Vector3(p1, 0, p2), Vector3.up*yPrime/length, rayColor);
                Physics.Raycast(origin + new Vector3(p1, 0, p2), Vector3.up*yPrime/length, out hit);

                if (hit.distance < smallestValue) smallestValue = hit.distance; 
            }
        }

        if (smallestValue < 0.0075f)
        {
            Debug.Log("less than?");
            //if within small tolerence to, stop moving on the y axis. take y vel - y vel
            isGrounded = true;
            return -Vector3.up * y;
            
        }
        else if (smallestValue != length) {
            Debug.Log("new small?");
            //if you do collide with something within the next move. Slow yourself down so you dont collide
            //the distance you were going to go - hit distance = the extra bit that goes to far.
            isGrounded = true;
            return Vector3.up * (length - smallestValue)*Time.deltaTime;

        }
        Debug.Log("apply grav");
        isGrounded = false;
        return Vector3.down * grav * Time.deltaTime;
    }

    Vector3 CollisionVelocityAdjustment(Vector3 v) {
        RaycastHit hit;
        Vector3 norm;
        Vector3 p1 = position + Vector3.up * rayHeight / 2;
        Vector3 p2 = position - Vector3.up * rayHeight / 2;
        if (Mathf.Abs(v.y) < 0.01f) {
            if (Physics.CapsuleCast(p1, p2, rayOffset, Vector3.down, out hit, grav * Time.deltaTime * 2)) {
                norm = hit.normal;
                v -= Vector3.Dot(norm, v) * norm;
                isGrounded = true;
            }
            
        }

        if (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v))) {
            
            //Debug.Log("hit distance: " + hit.distance);
            norm = hit.normal;
            
            v -= Vector3.Dot(norm, v) * norm;
        }

        return v;

    }
}
