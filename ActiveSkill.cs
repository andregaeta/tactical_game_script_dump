using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Skill", menuName = "Skill/Active Skill")]
public class ActiveSkill : Skill
{
    [SerializeField] public SkillBehaviour targetting;
    [SerializeField] public List<SkillBehaviour> skillBehaviours;
    [SerializeField] public GameObject animationObject;
    [SerializeField] public GameObject secondaryAnimationObject;

    private void Awake()
    {
        type = SkillType.active;
        //DevotionHelper helper = new DevotionHelper();
        //color = helper.GetColorByDevotion(devotion);
    }
    public ActiveSkillType activeSkillType;
    public int manaCost;
    public ElementColor color;
    public int range;
    public bool takesAction;
    //skill type (ranged, melee, magic, buff)

 
}
public enum ElementColor
{
    blue,
    red,
    yellow
}

public enum ActiveSkillType
{
    meleeAttack,
    rangedAttack,
    spell,
    support,
    debuff
}