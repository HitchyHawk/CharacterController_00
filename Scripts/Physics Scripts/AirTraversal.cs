using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTraversal : PhysicsImpl {
    public override bool inValidState()
    {
       //check to see if we are grounded
       return true;
    }

    public override void refresh(Vector3 influence)
    {
        throw new System.NotImplementedException();
    }
}
