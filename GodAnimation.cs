using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodAnimation : MonoBehaviour
{
    public float duration;
    public int impactFrame;
    public float durationToImpact;
    public bool targetsEnemy;
    public bool noFlip;

    private void Awake()
    {
        durationToImpact = impactFrame * 0.8333f;
        Animator anim = GetComponent<Animator>();
        duration = anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(this.gameObject, duration - 0.1f);
    }
}
