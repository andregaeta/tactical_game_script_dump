using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassiveSkill : Skill
{

    private void Awake()
    {
        type = SkillType.passive;
    }

}
