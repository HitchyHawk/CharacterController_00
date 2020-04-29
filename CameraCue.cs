using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCue : MonoBehaviour
{
    [HideInInspector] public float theta;
    [HideInInspector] public float phi;
    [HideInInspector] public Vector3 position;

    [Header("Rotation 1")]
    [Range(-Mathf.PI* 2    , Mathf.PI * 2   )] public float theta1;
    [Range(Mathf.PI * -0.5f, Mathf.PI * 0.5f)] public float phi1;
    [Space(0)]

    [Header("Position 1")]
    public Vector3 p1;

    [Header("Rotation 2")]
    [Range(-Mathf.PI * 2, Mathf.PI * 2)] public float theta2;
    [Range(Mathf.PI * -0.5f, Mathf.PI * 0.5f)] public float phi2;
    [Space(0)]

    [Header("Position 2")]
    public Vector3 p2;

    [Header("Smoothness")]
    [Range(50, 300)] public float CueSmoothness = 100;
    [Range(0, 20)] public float timeTran = 10;

    [HideInInspector]public float currentTime = 0;
    
    public bool activate;

    private Vector3 direction;

    private void Start()
    {
        position = p1;
    }

    private void OnDrawGizmos()
    {

        direction = new Vector3(Mathf.Sin(phi1 + Mathf.PI * 1.5f) * Mathf.Sin(theta1 + Mathf.PI),
                                Mathf.Cos(phi1 + Mathf.PI * 1.5f),
                                Mathf.Sin(phi1 + Mathf.PI * 1.5f) * Mathf.Cos(theta1 + Mathf.PI));

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p1,1);
        Gizmos.DrawRay(p1, direction*10);


        direction = new Vector3(Mathf.Sin(phi2 + Mathf.PI * 1.5f) * Mathf.Sin(theta2 + Mathf.PI),
                                Mathf.Cos(phi2 + Mathf.PI * 1.5f),
                                Mathf.Sin(phi2 + Mathf.PI * 1.5f) * Mathf.Cos(theta2 + Mathf.PI));

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p2, 1);
        Gizmos.DrawRay(p2, direction * 10);
    }

    private void Update()
    {
        if (activate && currentTime < timeTran)
        {
            currentTime += Time.deltaTime;
            position = (p2 - p1) * currentTime / timeTran + p1;
            theta = (theta2 - theta1) * currentTime / timeTran + theta1;
            phi = (phi2 - phi) * currentTime / timeTran + phi1;
        }
        else {
            activate = false;
        }
        
    }

}
