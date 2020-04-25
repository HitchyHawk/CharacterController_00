using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerController controls;
    
    public int checksPerFrame = 5;
    private float baseSpeed,sprintSpeed,jumpSpeed,speed,grav,drag;
   
    
    float rayOffset = 0.5f;
    float rayHeight = 0.75f;

    public bool isGrounded = false;
    public bool jumped = false;

    public Vector3 velocity = new Vector3(0,0,0);
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
        if (controls.reset) {
            transform.position = respawn;
            velocity = Vector3.zero+Vector3.up*0.01f;
            isGrounded = false;
        }
        if (controls.isSprint){
            speed = controls.baseSpeed * controls.sprintSpeed;
        }
        else speed = controls.baseSpeed;
        jumped = false;

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
        //calculate horizontal collision and slopes slopes due to velocity
        velocity = CollisionVelocityAdjustment(velocity);

        //This is where we will end up
        position += velocity;

        //Debug.DrawRay(transform.position, velocity * 10, new Color(1, 0, 0));
        transform.position = position;
    }
    Vector3 CollisionVelocityAdjustment(Vector3 v)
    {
        RaycastHit hit;
        Vector3 p1 = position + Vector3.up * rayHeight / 2;
        Vector3 p2 = position - Vector3.up * rayHeight / 2;

        //downwards cast to folow the slope down or up
        if (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y) + grav * Time.deltaTime) && !jumped) {
            int i = 0;
            do {
                //move collision sphere there to calculate how to deal with floor or ramp
                v -= Vector3.Dot(hit.normal, v) * hit.normal;
                i++;
            } while (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y) + grav * Time.deltaTime) && i < checksPerFrame);
            isGrounded = true;
        } 
        else {
            v += Vector3.down * controls.grav * Time.deltaTime;
            isGrounded = false;
        }

        //walking on slopes or walls
        if (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v))){
            int i = 0;
            do { 
                v -= Vector3.Dot(hit.normal, v) * hit.normal;
                i++;
            } while (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v)) && i < checksPerFrame) ;

            if (i == checksPerFrame) v *= 0;
        }
        return v;
    }
}
