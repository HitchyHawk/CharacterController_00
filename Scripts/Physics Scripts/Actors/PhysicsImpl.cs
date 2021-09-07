using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsImpl : PhysicsActor
{
    protected Vector3 currentVelocity = Vector3.zero; //in units per second, possible bug all things have the same velocity
    protected Transform body;
    protected PhysicsSettings physicsSettings;

    protected Vector3 topSphere, bottomSphere, mainSphere;
    protected float collisionRadius;
    protected LayerMask collidables;
    

    public Vector3 getVelocity() {
        return currentVelocity;
    }

    public float getVelocityMagnitude()
    {
        return currentVelocity.magnitude;
    }

    //are we grounded in the grounded walking state?
    //OR Airborne in the airborne
    public virtual bool inValidState()
    {
        throw new System.NotImplementedException();
    }

    public virtual void refresh(Vector3 influence)
    {
        throw new System.NotImplementedException();
    }
    public virtual void refresh(Vector2 influence)
    {
        throw new System.NotImplementedException();
    }

    public void setPhysicsObject(Transform newObject)
    {
        body = newObject;
    }
    public Transform getPhysicsObject()
    {
        return body;
    }

    public virtual void setSettings(PhysicsSettings newSettings)
    {
        physicsSettings = newSettings;
        collidables = physicsSettings.collidables;
    }

    public virtual void setTarget(Vector3 target)
    {
        throw new System.NotImplementedException();
    }

    public virtual void setCapsules(Vector3[] offsets)
    {
        throw new System.NotImplementedException();
    }

    public virtual void setSphere(Vector3 offset)
    {
        throw new System.NotImplementedException();
    }

    public virtual void activate()
    {
        throw new System.NotImplementedException();
    }


    public bool quickCapsuleCast(Vector3 direction, float distance, out RaycastHit hit){
        return Physics.CapsuleCast(topSphere, bottomSphere, collisionRadius, direction, out hit, distance, collidables);
    }

    public bool quickSphereCast(Vector3 P, Vector3 direction, float distance, float radius, out RaycastHit hit){
        //Debug.DrawRay(P, direction.normalized * distance, Color.black);
        return Physics.SphereCast(P, radius, direction, out hit, distance, collidables);
    }
    public bool quickSphereCast(Vector3 P, Vector3 direction, float distance, out RaycastHit hit){
        //Debug.DrawRay(P, direction.normalized * distance, Color.black);
        return Physics.SphereCast(P, collisionRadius, direction, out hit, distance, collidables);
    }
    public bool quickSphereCast(Vector3 P, Vector3 direction, float distance){
        RaycastHit hit;
        return quickSphereCast(P, direction, distance, out hit);
    }

    public void setVelocity(Vector3 newVelocity)
    {
        currentVelocity = newVelocity;
    }

    public virtual void reset()
    {
        throw new System.NotImplementedException();
    }
}
