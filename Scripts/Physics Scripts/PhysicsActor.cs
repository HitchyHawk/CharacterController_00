using UnityEngine;

public interface PhysicsActor
{    
    void refresh(Vector3 influence);
    void refresh(Vector2 influence);

    void setTarget(Vector3 target);
    void activate();
    void reset();
    void setCapsules(Vector3[] offsets);
    void setSphere(Vector3 offset);

    void setVelocity(Vector3 newVelocity);
    Vector3 getVelocity();
    float getVelocityMagnitude();

    void setPhysicsObject(Transform newObject);
    Transform getPhysicsObject();

    bool inValidState();
    void setSettings(PhysicsSettings newSettings);
}
