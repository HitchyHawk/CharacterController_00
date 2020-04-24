using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    Color rayColor = new Color(1, 0, 0);
    PlayerController controls;
    Collider coll;
    public LayerMask collisionMask = 0;
    public float tollerace = 0.01f;
    public float speed = 1;
    public float jumpSpeed = 2;
    public float drag = 0.2f;
    public float grav = 1;
    public float collisionRes = 5;
    public float rayOffset = 0.5f;
    public float rayHeight = 0.75f;

    public bool isGrounded = false;

    public Vector3 velocity = new Vector3(0,0,0);
    private Vector3 respawn;
    private Vector3 position;

    private float temp = 0;

    void Start()
    {
        respawn = transform.position;
        controls = GetComponent<PlayerController>();
        coll = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        //respawn position
        if (controls.reset) {
            transform.position = respawn;
            velocity = Vector3.zero;
            isGrounded = false;
        }

        //sets our origin for calulating movement
        position = transform.position;
        velocity += (Vector3.down*grav + Vector3.Normalize(controls.HoVeInput) * speed - new Vector3(velocity.x, 0, velocity.z) * drag) * Time.deltaTime;

        //calculate horizontal collision and slopes slopes due to velocity
        velocity = CollisionVelocityAdjustment(velocity);
        //This is where we will end up
        position += velocity;

        //are we grounded?
        //else apply appropriate gravity
        if (isGrounded){
            if (controls.isJump){
                isGrounded = false;
                position += Vector3.up * jumpSpeed*Time.deltaTime;
                velocity += Vector3.up * jumpSpeed *Time.deltaTime;
            }

        }
       
        Debug.DrawRay(transform.position, velocity * 10, new Color(1, 0, 0));
        transform.position = position;
        Debug.Log("END OF CYCLE---------------------------");
    }
    Vector3 CollisionVelocityAdjustment(Vector3 v)
    {
        RaycastHit hit;
        Vector3 p1 = position + Vector3.up * rayHeight / 2;
        Vector3 p2 = position - Vector3.up * rayHeight / 2;

        Debug.DrawRay(p2, Vector3.up, new Color(0, 1, 1));
        //downwards cast to folow the slope down or up
        Debug.Log("Collide with floor, for proper terrain Movement?");
        if (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y))) {
            p1 += Vector3.Normalize(v) * hit.distance;
            p2 += Vector3.Normalize(v) * hit.distance;
            //move collision sphere there to calculate how to deal with floor or ramp
            v -= Vector3.Dot(hit.normal, v) * hit.normal;
            Debug.Log("Normal floor hit:" + hit.normal);
            Debug.DrawRay(p2, Vector3.up, new Color(0, 1, 0));

            isGrounded = true;
        } else {
            Debug.Log("No ground below to map movement to properly");
            isGrounded = false;
        }

        Debug.Log("Checking this horizontal slope wall check");
        //horizontal cast to follow the slope if about to crash into it
        if (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v))) {
            v -= Vector3.Dot(hit.normal, v) * hit.normal;
            Debug.Log("Yes slope of wall/ramp, normal:" + hit.normal);
        }

        return v;
    }
}
