using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject
{
    public string displayName;
    public SkillType type;
    public Sprite icon;
    [TextArea(15, 20)]
    public string description;
    public int cooldown;
    public Devotion devotion;


}

public enum SkillType
{
    active,
    passive
}
public enum DamageType
{
    physical,
    magical
}

[System.Serializable]
public class SkillSlot
{
    public int cooldown;
    public void AddCooldown(int amount)
    {
        cooldown += amount;
        if (cooldown < 0)
            cooldown = 0;
    }
    
}
[System.Serializable]
public class ActiveSkillSlot : SkillSlot
{
    public ActiveSkill skill;
    public ActiveSkillSlot(ActiveSkill iskill)
    {
        skill = iskill;
        cooldown = 0;
    }
    public void PutOnCooldown()
    {
        cooldown = skill.cooldown;
    }

    public IEnumerator Co_PlayAnimations(SkillInfo skillInfo)
    {
        if (skill.animationObject == null)
        {
            Debug.LogError("Skill animation not found.");
            yield break;
        }

        //check direction
        bool flipDirection = false;
        if (skillInfo.primaryTarget.target != null)
        {
            //GridObject sourceGrid = skillInfo.source.GetComponent<GridObject>();
            //GridObject targetGrid = skillInfo.primaryTarget.target.GetComponent<GridObject>();
            if (skillInfo.source.transform.position.x > skillInfo.primaryTarget.target.transform.position.x)
                flipDirection = true;
        }
        if (flipDirection)
        {
            skillInfo.source.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            skillInfo.source.GetComponent<SpriteRenderer>().flipX = false;

        if (skill.secondaryAnimationObject == null)
        {
            if (skill.animationObject.GetComponent<GodAnimation>().targetsEnemy)
            {
                PlayAnimation(skillInfo.primaryTarget.target, flipDirection);
                
            }
            else
            {
                PlayAnimation(skillInfo.source, flipDirection);
                
            }
            
        }
        else
        {

            float waitTime = skillInfo.baseSkill.PlaySecondaryAnimation(skillInfo.source, flipDirection);
            yield return new WaitForSeconds(waitTime);

            skillInfo.baseSkill.PlayAnimation(skillInfo.primaryTarget.target, flipDirection);
        }
        yield return null;
    }
    private void PlayAnimation(GameObject source, bool flipDirection)
    {
        if(skill.animationObject == null)
        {
            Debug.LogError("Skill animation not found.");
            return;
        }
        GameState.Instance.CameraTarget = source;
        Vector3 targetPos = source.transform.position;
        targetPos.y += 1f;
        GameObject animObj = GameObject.Instantiate(skill.animationObject, targetPos, Quaternion.identity);
        if (!skill.animationObject.GetComponent<GodAnimation>().noFlip && flipDirection)
            animObj.GetComponent<SpriteRenderer>().flipX = true;
    }

    private float PlaySecondaryAnimation(GameObject source, bool flipDirection)
    {
        if (skill.secondaryAnimationObject == null)
        {
            return 0;
        }
        GameState.Instance.CameraTarget = source;
        Vector3 targetPos = source.transform.position;
        targetPos.y += 1f;
        GameObject animObj = GameObject.Instantiate(skill.secondaryAnimationObject, targetPos, Quaternion.identity);
        if (!skill.animationObject.GetComponent<GodAnimation>().noFlip && flipDirection)
            animObj.GetComponent<SpriteRenderer>().flipX = true;
        return animObj.GetComponent<GodAnimation>().duration;

    }

    public IEnumerator Co_OnSkillUse(GameObject source) //dont forget the one below
    {
        UnitBattle unitBattle = source.GetComponent<UnitBattle>();

        if (skill.takesAction && unitBattle.actionCounter <= 0)
        {
            Debug.Log("This character has already acted this turn.");
            yield break;
        }

        if (unitBattle.MP.currentMana < skill.manaCost)
        {
            Debug.Log("This character doesn't have enough mana.");
            yield break;
        }

        if (cooldown > 0)
        {
            Debug.Log("This skill is on cooldown.");
            yield break;
        }

        SkillInfo skillInfo = new SkillInfo(source, this);
      

        yield return SkillHandler.Instance.StartCoroutine(SkillHandler.Instance.Co_HandleBehaviour(skillInfo));
    }

    public IEnumerator Co_OnSkillUse(AIActionInfo info)
    {
        UnitBattle unitBattle = info.source.GetComponent<UnitBattle>();

        if (skill.takesAction && unitBattle.actionCounter <= 0)
        {
            Debug.Log("This character has already acted this turn.");
            yield break;
        }


        if (unitBattle.MP.currentMana < skill.manaCost)
        {
            Debug.Log("This character doesn't have enough mana.");
            yield break;
        }



        SkillInfo skillInfo = new SkillInfo(info);
        yield return SkillHandler.Instance.StartCoroutine(SkillHandler.Instance.Co_HandleBehaviour(skillInfo));
    }
}

[System.Serializable]
public class PassiveSkillSlot : SkillSlot
{
    public PassiveSkill skill;
    public PassiveSkillSlot(PassiveSkill iskill)
    {
        skill = iskill;
        cooldown = 0;
    }
}

[System.Serializable]
public class ActiveSkillData
{
    public int id;
    public ActiveSkill skill;
    public bool isInherited;
    public ActiveSkillData(int skillId, ActiveSkill aSkill, bool inherited)
    {
        this.id = skillId;
        this.skill = aSkill;
        this.isInherited = inherited;
    }
}



[System.Serializable]
public class PassiveSkillData
{
    public int id;
    public PassiveSkill skill;
    public bool isInherited;
    public PassiveSkillData(int skillId, PassiveSkill pSkill, bool inherited)
    {
        this.id = skillId;
        this.skill = pSkill;
        this.isInherited = inherited;
    }
}



