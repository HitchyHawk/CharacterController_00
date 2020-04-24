using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 HoVeInput = new Vector3(0,0,0);
    public bool isJump = false;
    public bool reset = false;

    

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if      (Input.GetKey(KeyCode.D))   HoVeInput[0] = 1;
        else if (Input.GetKey(KeyCode.A))   HoVeInput[0] = -1;
        else                                HoVeInput[0] = 0;

        if      (Input.GetKey(KeyCode.W))   HoVeInput[2] = 1;
        else if (Input.GetKey(KeyCode.S))   HoVeInput[2] = -1;
        else                                HoVeInput[2] = 0;

        if (Input.GetKey(KeyCode.Space))    isJump = true;
        else                                isJump = false;
        */
        if (Input.GetKey(KeyCode.R))        reset = true;
        //else                                reset = false;


        //tells us what keys are pressed, in a form that is easy to math with.
        //Debug.Log(HoVeInput[0] + ", " + HoVeInput[2]);

    }
}
