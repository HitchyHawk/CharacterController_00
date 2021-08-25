using UnityEngine;

public interface PhysicsActor
{    
    void refresh(Vector3 influence);
    void refresh(Vector2 influence);

    Vector3 getVelocity();
    float getVelocityMagnitude();

    void setPhysicsObject(Transform newObject);
    Transform getPhysicsObject();

    bool inValidState();

    void setSettings(PhysicsSettings newSettings);
}
