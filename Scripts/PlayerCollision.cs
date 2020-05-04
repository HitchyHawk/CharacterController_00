using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerController controls;

    public int      checksPerFrame = 5;
    public float    smooth = 5;
    private float   speed, grav, drag;
    public float    slopeMaxDeg = 60;
    private float jumpTimer = 0;
      

    float rayOffset = 0.5f;
    float rayHeight = 1f;

    public bool isGrounded = false;
    private bool jumped = false;
    public bool isSliding = false;

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

        if (isGrounded || isSliding) {
            velocity += (controls.HoVeInput * speed) * Time.deltaTime;
            velocity -= new Vector3(velocity.x, 0, velocity.z) / controls.drag ;
        }
        else {
            velocity += (controls.HoVeInput * speed / controls.airSpeedDiv) * Time.deltaTime;
            velocity -= new Vector3(velocity.x, 0, velocity.z) / (controls.airDrag);
        }

        if (jumpTimer >= controls.jumpCooldown) {
            jumpTimer = controls.jumpCooldown;
            if (isGrounded || isSliding){
                if (controls.isJump){
                    if (isSliding){
                        velocity = new Vector3(velocity.x * 1.5f, controls.jumpSpeed * Time.deltaTime / 1.3f, velocity.z * 1.5f);
                    } else velocity += Vector3.up * controls.jumpSpeed * Time.deltaTime;
                    isGrounded = false;
                    jumped = true;
                    isSliding = false;
                    jumpTimer = 0;
                }
            }
        }
        

        if (Vector3.Magnitude(velocity) > controls.maxSpeed) velocity = Vector3.Normalize(velocity) * controls.maxSpeed;

        //calculate horizontal collision and slopes slopes due to velocity
        velocity = CollisionVelocityAdjustment(velocity);

        position += velocity;
        jumpTimer += Time.deltaTime;
    }

    void Update(){
        transform.position = (position - transform.position) / smooth + transform.position;
    }
    Vector3 CollisionVelocityAdjustment(Vector3 v)
    {
        RaycastHit hit;
        Vector3 p1 = position + Vector3.up * rayHeight / 2;
        Vector3 p2 = position - Vector3.up * rayHeight / 2;

        if (isSliding) {
            if (Physics.SphereCast(p2, rayOffset, -v, out hit, Vector3.Magnitude(v) * 2, controls.collisionMask))
            {
                v = Vector3.ProjectOnPlane(v, hit.normal);
            }
            else if (Physics.SphereCast(p2, rayOffset, v, out hit, Vector3.Magnitude(v) * 2, controls.collisionMask)) {
                v -= Vector3.ProjectOnPlane(Vector3.up, hit.normal) * grav * Time.deltaTime * 2.5f;
            }
            
        }

        if (Physics.SphereCast(p2, rayOffset, Vector3.up * (v.y - grav) / Mathf.Abs(v.y - grav), out hit, Mathf.Abs(v.y) + grav * Time.deltaTime, controls.collisionMask) && !jumped) {
            if (Vector3.Angle(hit.normal, Vector3.up) > slopeMaxDeg) {
                isSliding = true;
                if (Vector3.Dot(v,hit.normal) < 0) v -= Vector3.Dot(hit.normal, v) * hit.normal;
            } 
            else {
                isSliding = false;
                int i = 0;
                do {
                    v -= Vector3.Dot(hit.normal, v) * hit.normal;
                    i++;
                } while (Physics.SphereCast(p2, rayOffset, Vector3.down, out hit, Mathf.Abs(v.y), controls.collisionMask) && i < checksPerFrame);
                isGrounded = true;
            }
        }
        else {
            isGrounded = false;
            isSliding = false;
        }
        

        if (!isGrounded || isSliding) v += Vector3.down * controls.grav * Time.deltaTime;

        //walking on slopes or walls
        if (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v), controls.collisionMask)) {
            int i = 0;
            do {
                v -= Vector3.Dot(hit.normal, v) * hit.normal;
                i++;
            } while (Physics.CapsuleCast(p1, p2, rayOffset, v, out hit, Vector3.Magnitude(v),controls.collisionMask) && i < checksPerFrame);

            if (i == checksPerFrame) v *= 0;

            jumped = false;
        }
        return v;
    }

   
}