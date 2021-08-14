using System;
using UnityEngine;

[Serializable]
public class Hitch_AnimationMeta
{
    public string animationName;
    public int frameAmount = 1;
    public float distancePerCycle = 1;
    public bool isStationary = false;
    public bool loops = true;

    Hitch_AnimationMeta(string Name, int frames, float distance, bool stationary, bool loop)
    {
        animationName = Name;
        frameAmount = frames;
        distancePerCycle = distance;
        isStationary = stationary;
        loops = loop;
    }
}
