using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsImpl : MonoBehaviour, PhysicsActor
{
    protected static Vector3 currentVelocity; //in units per second, possible bug all things have the same velocity
    protected Transform physicsObject;
    protected PhysicsSettings physicsSettings;

    

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
        physicsObject = newObject;
    }
    public Transform getPhysicsObject()
    {
        return physicsObject;
    }

    public virtual void setSettings(PhysicsSettings newSettings)
    {
        physicsSettings = newSettings;
    }
}
