using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationInfo
{

    public GameObject animationObject;
    public AnimationType type;
    public bool targetsSelf;
    public float timeToImpact;
    public bool noFlip;
}

