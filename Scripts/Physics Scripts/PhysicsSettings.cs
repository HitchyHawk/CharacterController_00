using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSettings : ScriptableObject
{
    [Range(0.1f, 3)] public float collisionRadius = 1f;
    [Range(0.1f, 5)] public float collisionHeight = 2f;  
    [Range(0.1f, 5)] public float stepUpMax = 1f; 
    [Range(0, 90)] public float maxStandingSlopeAngle = 60;

    public LayerMask collidables;
}
