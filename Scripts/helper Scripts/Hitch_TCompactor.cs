using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitch_TCompactor : MonoBehaviour
{
    public Transform[] transforms;
    int lastIndex;
    string lastName;
    bool searchedPositive = false;
    void Start()
    {
        transforms = GetComponentsInChildren<Transform>();
    }

    public Transform GetTransform(string name){
        int index;
        
        //demorgans law, sorry if confusing
        //essentially, if we found the item before, we have no need to search for it again. 
        //However double check that we are looking for the right thing
        if (!searchedPositive || !lastName.Equals(name)){
            searchedPositive = false;
            index = find(name);
            if (index == -1) {
                
                return null;
            } 
            //else not needed

            searchedPositive = true;
            lastIndex = index;
            lastName = name;
        }

        return transforms[lastIndex];
    }
    int find(string name){
        for (int i = 0; i < transforms.Length; i++){
            if (transforms[i].name.Equals(name)) {
                lastName = name;
                return i;
            }
        }
        return -1;
    }
}
