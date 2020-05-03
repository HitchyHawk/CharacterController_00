using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialMaths;

public class CameraCue : MonoBehaviour
{

    [HideInInspector] public AngleSpace theta;
    [HideInInspector] public AngleSpace phi;

    //[HideInInspector] public float theta;                                                           //The final Theta
    //[HideInInspector] public float phi;                                                             //The final Phi
    [HideInInspector] public Vector3 position;                                                      //The final position
    public Vector3[] points = new Vector3[3] {
        new Vector3(0f,1f,0f),
        new Vector3(0f,2f,0f),
        new Vector3(0f,3f,0f)
    };                                                    //Our camera rail guides
    [Range(0             ,Mathf.PI*2   )]public float[] targetTheta  = new float[3] {0f,0f,0f};     //What each theta is for each guide
    [Range(-Mathf.PI*0.5f,Mathf.PI*0.5f)]public float[] targetPhi    = new float[3] {0f,0f,0f};     //What each Phi is for each guide

    /// <summary>
    /// Conditional Statements for custom shtuff
    /// </summary>
    [Header("Smoothness")]
    [Range(50, 300)] public float CueSmoothness = 100;
    [Range(0.1f, 20)] public float timeEnd = 5;
    [Range(0.1f, 20)] public float timeMiddle = 5;

    public bool allowedToMove = true;       //Is the player allowed to move while cue animation
    public bool lockMovement = false;       //Should we lock the movement to the frame before entering the cue
    public bool removeCueAfter = false;     //Delete the cue after animation or leaving?
    public bool makeGlobal = false;         //Once you enter the cue you dont have to stay in it for the animation to finish
    [Range(0,20)]public float timeStay = 0;
    public bool positionBased = false;      //Instead of relying on time for the animation, will it depend on the players position?

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

        theta = new AngleSpace(0, Mathf.PI * 2, -0, true);
        phi = new AngleSpace(0, Mathf.PI * 0.5f, -Mathf.PI * 0.5f, false);
    }
    private void Start() {
        position = points[0];
        theta = new AngleSpace(0, Mathf.PI * 2, -0, true);
        phi = new AngleSpace(0, Mathf.PI * 0.5f, -Mathf.PI * 0.5f, false);
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
        if (activate) {
            if (currentTime < (timeMiddle + timeEnd))
            {
                currentTime += Time.deltaTime;
                position = GetPoint(currentTime / (timeMiddle+timeEnd));

                if (currentTime < timeMiddle){
                    theta.LinearLerp(targetTheta[0], targetTheta[1], currentTime, timeMiddle);
                    phi.LinearLerp  (targetPhi[0]  , targetPhi[1]  , currentTime, timeMiddle);
                }
                else{   
                    theta.LinearLerp(targetTheta[1], targetTheta[2], (currentTime - timeMiddle), timeEnd);
                    phi.LinearLerp  (targetPhi[1]  , targetPhi[2]  , (currentTime - timeMiddle), timeEnd);
                }

            }
            else{
                currentTime += Time.deltaTime;
                if (currentTime > (timeMiddle+timeEnd) + timeStay){
                    activate = false;
                    allowedToMove = true;
                    if (removeCueAfter) Destroy(gameObject);
                }
            }
        }
        
    }

    public float GetTheta() { return theta.GetFloat(); }
    public float GetPhi() { return phi.GetFloat(); }

    private void OnDrawGizmos()
    {
        for (int i = 0; i <= 2; i++) {
            direction = new Vector3(Mathf.Sin(targetPhi[i] + Mathf.PI * 1.5f) * Mathf.Sin(targetTheta[i] + Mathf.PI),
                                    Mathf.Cos(targetPhi[i] + Mathf.PI * 1.5f),
                                    Mathf.Sin(targetPhi[i] + Mathf.PI * 1.5f) * Mathf.Cos(targetTheta[i] + Mathf.PI));
            Gizmos.color = new Color(1 - i *0.5f, 1 - i*0.5f,1 - i*0.5f);
            Gizmos.DrawSphere(GetPoint(i *0.5f) , 0.5f);
            Gizmos.DrawRay(GetPoint(i*0.5f) , direction * 4);
        }
    }
}
