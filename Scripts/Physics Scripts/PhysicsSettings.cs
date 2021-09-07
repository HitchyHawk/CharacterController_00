using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hitch Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
    [Range(0.1f, 1)] public float collisionRadius = 0.32f;
    [Range(0.1f, 1)] public float collisionHeight = 0.55f;  
    [Range(0.1f, 3)] public float stepUpMax = 0.75f; 
    [Range(0, 90)] public float maxStandingSlopeAngle = 60;
    [Range(0.01f, 0.5f)] public float timeMax = 0.19f;

    [Range(0.01f, 2)] public float timeToApex = 0.75f;
    [Range(0, 4)] public float apexHeight = 1.3f;

    public LayerMask collidables;
}
