using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerController controls;

    public int checksPerFrame = 5;
    public float smooth = 5;
    private float speed, grav, drag;


    float rayOffset = 0.5f;
    float rayHeight = 0.75f;

    private bool isGrounded = false;
    private bool jumped = false;

    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 respawn;
    private Vector3 position;

    void Start()
    {
        respawn = transform.position;
        controls = GetComponent<PlayerController>();
        speed = controls.baseSpeed;
        drag = controls.drag;
        grav = controls.grav;
    }

    void FixedUpdate()
    {
        //respawn position
        if (controls.reset)
        {
            transform.position = respawn;
            velocity = Vector3.zero + Vector3.up * 0.01f;
            isGrounded = false;
        }
        if (controls.isSprint)
        {
            speed = controls.baseSpeed * controls.sprintSpeed;
        }
        else speed = controls.baseSpeed;

        //sets our origin for calulating movement
        position = transform.position;
        velocity += (controls.HoVeInput * speed - new Vector3(velocity.x, 0, velocity.z) * controls.drag) * Time.deltaTime;

        if (isGrounded)
        {
            if (controls.isJump)
            {
                isGrounded = false;
                jumped = true;
                velocity += Vector3.up * controls.jumpSpeed * Time.deltaTime;
            }
        }

        if (Vector3.Magnitude(velocity) > controls.maxSpeed) velocity = Vector3.Normalize(velocity) * controls.maxSpeed;

        //calculate horizontal collision and slopes slopes due to velocity
        velocity = CollisionVelocityAdjustment(velocity);

        position += velocity;
    }

    //ghetto method to fix problem
    //when switching my collision detection to update, it breaks. It needs the fixed time step.
    //The fix is that we simply smooth between the two points it needs to go to. Simple fix since the intial and final have nothing inbetween them. So its always safe.
    void Update()
    {
        transform.position = (position - transform.position) / smooth + transform.position;
    }
    Vector3 CollisionVelocityAdjustment(Vector3 v)
    {
        RaycastHit hit;
        Vector3 p1 = position + Vector3.up * rayHeight / 2;
        Vector3 p2 = position - Vector3.up * rayHeight / 2;

        //downwards cast to folow the slope down or up
        if (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y) + grav * Time.deltaTime, controls.collisionMask) && !jumped)
        {
            int i = 0;
            do
            {
                //move collision sphere there to calculate how to deal with floor or ramp
                v -= Vector3.Dot(hit.normal, v) * hit.normal;
                i++;
            } while (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y) + grav * Time.deltaTime, controls.collisionMask) && i < checksPerFrame);
            isGrounded = true;
        }
        else
        {
            v += Vector3.down * controls.grav * Time.deltaTime;
            isGrounded = false;
        }

        //walking on slopes or walls
        if (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v), controls.collisionMask))
        {
            int i = 0;
            do
            {
                v -= Vector3.Dot(hit.normal, v) * hit.normal;
                i++;
            } while (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v)) && i < checksPerFrame);

            if (i == checksPerFrame) v *= 0;

            jumped = false;
        }
        return v;
    }
}