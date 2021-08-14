using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitch_TC_Finder : MonoBehaviour
{
    public Hitch_TCompactor compactor;
    void Start(){
        compactor = GetComponentInChildren<Hitch_TCompactor>();
    }

    

}
