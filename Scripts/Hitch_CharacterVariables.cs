using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hitch Character Settings")]
public class Hitch_CharacterVariables : ScriptableObject
{
    ///CAMERA CONTROLS
    ///==================================
    [Header("==Camera Controls==")]
    //public GameObject cameraTracker;
    [Range(0, 3)] public float orbitalMax = 1;
    [Range(0, 16)] public float cameraRadius = 8;
    [Range(0, 50)] public float cameraSensitivity = 10;

    [Space(5)]
    [Range(0, 3)] public float cameraPositionSmoothness = 1.6f;
    [Range(0, 3)] public float cameraAngleSmoothness = 1.1f;
    public LayerMask cameraMask;

    /*
    [Space(5)]
    [Range(0.1f, 5)] public float ignoreWidth = 1;
    [Range(1, 64)]   public int ignoreResolution = 10;
    [Range(0, 1)]    public float ignoreRatio = 0.75f;
    */
    [Space(15)]
    [Header("=======================================")]
    [Header("==Character Movement==")]
    [Header("jump based")]
    [Range(0, 3)] public float jumpApex = 2;
    [Range(0, 2)] public float time2Apex = 0.325f;
    [Range(0, 100)] public float terminalVelocity = 75;

    [HideInInspector] public float jumpSpeed, gravity;

    [Header("walk / run based")]
    [Range(0, 36)]      public float maxRunSpeed = 17;
    [Range(0, 3)]       public float time2MaxRunSpeed = 1;
    [Range(0, 1)]       public float walkRatio = 0.2f;
    [Range(0, 1.5f)]    public float time2Stop = 0.7f;

    [HideInInspector]  public float runAcceleration, walkAcceleration, maxWalkSpeed;


    [Header("collision physics")]
    [Range(0, 90)] public float maxStandingAngle = 60;
    [Range(0,  1)] public float lifterRadius    = 0.5f;
    [Range(0,  1)] public float bodyRadius      = 0.75f;
    [Range(0,  1)] public float stickiness      = 0.2f;
    [Range(1,  5)] public int   collisionChecks = 3;
    public LayerMask mask;

    [HideInInspector] public Vector3 lOffset = Vector3.zero, bOffset1 = Vector3.zero, bOffset2 = Vector3.zero;

    [Space(15)]
    [Header("=======================================")]
    [Header("==Animation==")]
    //public Animator animator;
    [Range(0, 10)] public float turnSpeed = 5;
    [Range(0, 3)] public float forwardsRotation = 0.4f;
    [Range(0, 30)] public float sidewaysRotation = 10;

    [Header("animation metas")]
    public Hitch_AnimationMeta sprintMeta;
    public Hitch_AnimationMeta walkMeta;
    public Hitch_AnimationMeta idleMeta;
    public Hitch_AnimationMeta sLJumpMeta;
    public Hitch_AnimationMeta sRJumpMeta;
    public Hitch_AnimationMeta fallingMeta;

    void Awake() { updateValues(); }
    void OnValidate() { updateValues(); }

    void updateValues() {
        jumpSpeed = 2 * jumpApex / (time2Apex == 0? 1 : time2Apex);
        gravity = -jumpSpeed / (time2Apex == 0? 1 : time2Apex);

        //dingus proofing
        runAcceleration = 2 * maxRunSpeed / (time2MaxRunSpeed == 0? 1 :time2MaxRunSpeed * time2MaxRunSpeed);
        walkAcceleration = runAcceleration * walkRatio;

        maxWalkSpeed = maxRunSpeed * walkRatio;

        lOffset  = Vector3.up *  lifterRadius;
        bOffset1 = Vector3.up * (lifterRadius +     bodyRadius);
        bOffset2 = Vector3.up * (lifterRadius + 2 * bodyRadius);
       
    }
}
