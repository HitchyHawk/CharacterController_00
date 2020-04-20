using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int[] HoVeInput = {0,0};
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if      (Input.GetKey(KeyCode.D))   HoVeInput[0] = 1;
        else if (Input.GetKey(KeyCode.A))   HoVeInput[0] = -1;
        else                                HoVeInput[0] = 0;

        if      (Input.GetKey(KeyCode.W))   HoVeInput[1] = 1;
        else if (Input.GetKey(KeyCode.S))   HoVeInput[1] = -1;
        else                                HoVeInput[1] = 0;

        //tells us what keys are pressed, in a form that is easy to math with.
        Debug.Log(HoVeInput[0] + ", " + HoVeInput[1]);

    }
}
