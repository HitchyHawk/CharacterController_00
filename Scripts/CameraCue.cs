using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCue : MonoBehaviour
{
    [HideInInspector] public float theta;
    [HideInInspector] public float phi;
    [HideInInspector] public Vector3 position;
    public Vector3[] points = new Vector3[3] {
        new Vector3(0f,1f,0f),
        new Vector3(0f,2f,0f),
        new Vector3(0f,3f,0f)
    };
    [Range(0             ,Mathf.PI*2   )]public float[] targetTheta  = new float[3] {0f,0f,0f};
    [Range(-Mathf.PI*0.5f,Mathf.PI*0.5f)]public float[] targetPhi    = new float[3] {0f,0f,0f};
    
    /// <summary>
    /// Conditional Statements for custom shtuff
    /// </summary>
    [Header("Smoothness")]
    [Range(50, 300)] public float CueSmoothness = 100;
    [Range(0.1f, 20)] public float timeTran = 10;
    [Range(0.1f, 20)] public float timeMiddle = 5;

    public bool allowedToMove = true;
    public bool lockMovement = false;
    public bool removeCueAfter = false;

    [HideInInspector]public float currentTime = 0;
    [HideInInspector]public bool activate;

    private Vector3 direction;

    public void Reset()
    {
        points = new Vector3[3] {
            new Vector3(0f,1f,0f),
            new Vector3(0f,2f,0f),
            new Vector3(0f,3f,0f)
        };

        targetTheta = new float[3] { 0f, 0f, 0f };
        targetPhi = new float[3] { 0f, 0f, 0f };
    }
    private void Start() {
        position = points[0];
    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(GetPoint(points[0], points[1], points[2], t));
    }
    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
    }

    private void Update()
    {
        if (activate && currentTime < timeTran)
        {
            currentTime += Time.deltaTime;
            position    = GetPoint(currentTime/timeTran);

            for (int i = 0; i <= 2; i++) {
                if (targetTheta[i] < 0         ) targetTheta[i] += Mathf.PI * 2;
                if (targetTheta[i] > Mathf.PI*2) targetTheta[i] -= Mathf.PI * 2;
            }

            if (currentTime < timeMiddle)
            {
                if      (Mathf.Abs(targetTheta[1] - targetTheta[0]) > Mathf.Abs(targetTheta[1] - (targetTheta[0] + Mathf.PI * 2))) targetTheta[1] -= Mathf.PI * 2;
                else if (Mathf.Abs(targetTheta[1] - targetTheta[0]) > Mathf.Abs(targetTheta[1] - (targetTheta[0] - Mathf.PI * 2))) targetTheta[1] += Mathf.PI * 2;

                theta   = (targetTheta[1] - targetTheta[0]) * currentTime / timeMiddle + targetTheta[0];
                phi     = (targetPhi[1]   - targetPhi[0])   * currentTime / timeMiddle + targetPhi[0];
            }
            else {
                if      (Mathf.Abs(targetTheta[2] - targetTheta[1]) > Mathf.Abs(targetTheta[2] - (targetTheta[1] + Mathf.PI * 2))) targetTheta[2] -= Mathf.PI * 2;
                else if (Mathf.Abs(targetTheta[2] - targetTheta[1]) > Mathf.Abs(targetTheta[2] - (targetTheta[1] - Mathf.PI * 2))) targetTheta[2] += Mathf.PI * 2;

                theta   = (targetTheta[2] - targetTheta[1]) * (currentTime-timeMiddle) / (timeTran - timeMiddle) + targetTheta[1];
                phi     = (targetPhi[2]   - targetPhi[1])   * (currentTime-timeMiddle) / (timeTran - timeMiddle) + targetPhi[1];
            }
            
        }
        else {
            activate = false;
            if (currentTime > timeTran) {
                allowedToMove = true;
                if (removeCueAfter) Destroy(gameObject);
            }
        }
    }



    private void OnDrawGizmos()
    {
        for (int i = 0; i <= 2; i++) {
            direction = new Vector3(Mathf.Sin(targetPhi[i] + Mathf.PI * 1.5f) * Mathf.Sin(targetTheta[i] + Mathf.PI),
                                    Mathf.Cos(targetPhi[i] + Mathf.PI * 1.5f),
                                    Mathf.Sin(targetPhi[i] + Mathf.PI * 1.5f) * Mathf.Cos(targetTheta[i] + Mathf.PI));
            Gizmos.color = new Color(1 - i *0.5f, 1 - i*0.5f,1 - i*0.5f);
            Gizmos.DrawSphere(points[i] + transform.position, 0.5f);
            Gizmos.DrawRay(points[i] + transform.position, direction * 4);
        }
    }
}
