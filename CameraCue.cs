using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCue : MonoBehaviour
{
    [Range(-Mathf.PI*2,Mathf.PI*2)]
    public float theta;
    [Range(Mathf.PI * -0.5f, Mathf.PI * 0.5f)]
    public float phi;
    public Vector3 position;

    private Vector3 direction;



    private void OnDrawGizmos()
    {
        direction = new Vector3(Mathf.Sin(phi - Mathf.PI * 0.5f) * Mathf.Sin(-theta),
                                            Mathf.Cos(phi - Mathf.PI * 0.5f),
                                            Mathf.Sin(phi - Mathf.PI * 0.5f) * Mathf.Cos(-theta));
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position,1);
        Gizmos.DrawRay(position, direction*10);
    }

}
