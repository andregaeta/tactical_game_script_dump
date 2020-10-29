using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    public SkillBehaviour standardRulesBehaviour;

    private static SkillHandler _instance;

    public static SkillHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public bool countingTime;
    public float elapsedTime;
    [SerializeField] TileEffectWhenAttacked tileProtectionEffect;

    public IEnumerator Co_HandleTrigger(SkillInfo skillInfo)
    {
        Debug.Log(skillInfo.source + " triggers " + skillInfo.baseSkill.skill + " on " + skillInfo.primaryTarget);
        skillInfo.isTrigger = true;
        yield return StartCoroutine(Co_HandleBehaviour(skillInfo));
    }

    public IEnumerator Co_HandleBehaviour(SkillInfo skillInfo)
    {
        if(skillInfo.baseSkill.skill.targetting != null)
        {
            yield return StartCoroutine(skillInfo.baseSkill.skill.targetting.Co_Behave(skillInfo, null));
            if (skillInfo.primaryTarget.target == null)
            {
                if (skillInfo.secondaryTargets.Count == 0)
                {
                    Debug.Log("Targetting failed");
                    yield break;
                }
                else if (skillInfo.secondaryTargets[0].target == null)
                {
                    Debug.Log("Targetting failed");
                    yield break;
                }
            }
        }

        //triggers were here

        //PrintBehaviourList(skillInfo);

        if (!GameState.Instance.TryStartMinorState(GameState.minorStates.Animating))
        {
            Debug.Log("Couldn't start animating");
        }

        GameObject circle = null;
        if (!skillInfo.isTrigger)
        {
            DarkFog.Instance.FadeIn(skillInfo.GetHighlightTargets());
            circle = SummonCircle(skillInfo);
        }

        yield return StartCoroutine(skillInfo.baseSkill.Co_PlayAnimations(skillInfo));
        SetupTimeCount();


        GameObject protector = isProtectedBy(skillInfo.primaryTarget.target);
        if (protector != null)
        {
            SourceData sourceData = new SourceData();
            sourceData.sourceGameObject = skillInfo.primaryTarget.target;
            sourceData.secondarySourceGameObject = protector;
            SkillBehaviourSlot slot = new SkillBehaviourSlot(tileProtectionEffect.behaviour, sourceData);
            skillInfo.AddBehaviour(slot);

            skillInfo.primaryTarget.target = protector;
            
        }

        if (!skillInfo.isTrigger) //attention to this, test later and check what skills trigger on action vs damage
        {
            GetSourceBehaviours(skillInfo, skillInfo.source);
            GetSourceTileBehaviours(skillInfo, skillInfo.source);
            GetTargetBehaviours(skillInfo, skillInfo.primaryTarget.target);
            GetTargetTileBehaviours(skillInfo, skillInfo.primaryTarget.target);
            skillInfo.AddBehaviour(new SkillBehaviourSlot(standardRulesBehaviour, null));
        }

        GameState.Instance.CameraTarget = skillInfo.primaryTarget.target;

        foreach (SkillBehaviourSlot behaviourSlot in skillInfo.behaviours.ToArray()) // before 0
        {
            if (behaviourSlot.SkillBehaviour.order >= 0) continue;
            yield return StartCoroutine(behaviourSlot.SkillBehaviour.Co_Behave(skillInfo, behaviourSlot.source));
        }
        while (elapsedTime < skillInfo.animationInfo.timeUntilImpact)
            yield return null;
        foreach (SkillBehaviourSlot behaviourSlot in skillInfo.behaviours.ToArray()) // exactly 0
        {
            if (behaviourSlot.SkillBehaviour.order != 0) continue;
            yield return StartCoroutine(behaviourSlot.SkillBehaviour.Co_Behave(skillInfo, behaviourSlot.source));
            yield return new WaitForSeconds(0.083f);
        }
        foreach (SkillBehaviourSlot behaviourSlot in skillInfo.behaviours.ToArray()) // after 0
        {
            if (behaviourSlot.SkillBehaviour.order <= 0) continue;
            if (behaviourSlot.SkillBehaviour.order > 5) continue;
            yield return StartCoroutine(behaviourSlot.SkillBehaviour.Co_Behave(skillInfo, behaviourSlot.source));
        }
        while (elapsedTime < skillInfo.animationInfo.timeUntilFinish)
            yield return null;
        foreach (SkillBehaviourSlot behaviourSlot in skillInfo.behaviours.ToArray()) // after 5 (Standard Rules)
        {
            if (behaviourSlot.SkillBehaviour.order < 5) continue;
            yield return StartCoroutine(behaviourSlot.SkillBehaviour.Co_Behave(skillInfo, behaviourSlot.source));
        }

        StopTimeCount();

        if (!skillInfo.isTrigger)
        {
            circle.GetComponent<SmoothSpawn>().FadeOut();
            yield return new WaitForSeconds(0.3f);
            DarkFog.Instance.FadeOut(skillInfo.GetHighlightTargets());
        }

        GameState.Instance.CameraTarget = null;
        GameState.Instance.TryStopMinorState(GameState.minorStates.Animating);
        EventHandler.skillUse.Invoke(skillInfo);
    }

    void SetupTimeCount()
    {
        elapsedTime = 0;
        countingTime = true;
    }
    void StopTimeCount()
    {
        countingTime = false;
        elapsedTime = 0;
    }
    private void Update()
    {
        if (countingTime)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    void GetSourceBehaviours(SkillInfo skillInfo, GameObject target)
    {
        if (target == null)
            return;
        UnitBattle unitBattle = target.GetComponent<UnitBattle>();
        foreach (PassiveSkillSlot passiveSlot in unitBattle.passiveSkills)
        {
            if (passiveSlot.skill is ITriggerOnAction)
            {
                ITriggerOnAction trigger = passiveSlot.skill as ITriggerOnAction;
                SourceData sourceData = new SourceData();
                sourceData.sourceSkillSlot = passiveSlot;
                sourceData.sourceGameObject = skillInfo.source;
                trigger.TriggerOnAction(skillInfo, sourceData);
            }
        }
        foreach (StatusEffectSlot effect in unitBattle.statusEffectSlots.ToArray())
        {
            if (effect.statusEffect is ITriggerOnAction)
            {
                ITriggerOnAction trigger = effect.statusEffect as ITriggerOnAction;
                SourceData sourceData = new SourceData();
                sourceData.sourceEffectSlot = effect;
                sourceData.sourceGameObject = skillInfo.source;
                trigger.TriggerOnAction(skillInfo, sourceData);
            }
        }
    }
    void GetSourceTileBehaviours(SkillInfo skillInfo, GameObject target)
    {
        if (target == null)
            return;
        GridObject gridObject = target.GetComponent<GridObject>();
        GridTile gridTile = gridObject.baseTile.GetComponent<GridTile>();
        foreach(TileEffectSlot slot in gridTile.tileEffects)
        {
            if (slot.effect is ITriggerOnAction)
            {
                if (slot.unitTypeAffected != UnitType.both && slot.unitTypeAffected != target.GetComponent<UnitBattle>().unitType)
                    continue;
                ITriggerOnAction trigger = slot.effect as ITriggerOnAction;
                SourceData sourceData = new SourceData();
                sourceData.sourceTileEffectSlot = slot;
                sourceData.sourceGameObject = target;
                sourceData.secondarySourceGameObject = slot.sourceData.sourceGameObject;
                trigger.TriggerOnAction(skillInfo, sourceData);
            }
        }
    }

    void GetTargetBehaviours(SkillInfo skillInfo, GameObject target)
    {
        if (target == null)
            return;
        if (skillInfo.baseSkill.skill.activeSkillType == ActiveSkillType.support ||
            skillInfo.baseSkill.skill.activeSkillType == ActiveSkillType.debuff
            )
            return;

        UnitBattle unitBattle = target.GetComponent<UnitBattle>();
        foreach (PassiveSkillSlot passiveSlot in unitBattle.passiveSkills)
        {
            if (passiveSlot.skill is ITriggerWhenTargettedByEnemy)
            {
                ITriggerWhenTargettedByEnemy trigger = passiveSlot.skill as ITriggerWhenTargettedByEnemy;
                SourceData sourceData = new SourceData();
                sourceData.sourceSkillSlot = passiveSlot;
                sourceData.sourceGameObject = target;
                trigger.TriggerWhenTargettedByEnemy(skillInfo, sourceData);
            }
        }
        foreach (StatusEffectSlot effect in unitBattle.statusEffectSlots.ToArray())
        {
            if (effect.statusEffect is ITriggerWhenTargettedByEnemy)
            {
                ITriggerWhenTargettedByEnemy trigger = effect.statusEffect as ITriggerWhenTargettedByEnemy;
                SourceData sourceData = new SourceData();
                sourceData.sourceEffectSlot = effect;
                sourceData.sourceGameObject = target;
                trigger.TriggerWhenTargettedByEnemy(skillInfo, sourceData);
            }
        }
    }

    void GetTargetTileBehaviours(SkillInfo skillInfo, GameObject target)
    {
        if (target == null)
            return;
        GridObject gridObject = target.GetComponent<GridObject>();
        GridTile gridTile = gridObject.baseTile.GetComponent<GridTile>();
        foreach (TileEffectSlot slot in gridTile.tileEffects)
        {
            if (slot.effect is ITriggerWhenTargettedByEnemy)
            {
                if (slot.unitTypeAffected != UnitType.both && slot.unitTypeAffected != target.GetComponent<UnitBattle>().unitType)
                    continue;
                ITriggerWhenTargettedByEnemy trigger = slot.effect as ITriggerWhenTargettedByEnemy;
                SourceData sourceData = new SourceData();
                sourceData.sourceTileEffectSlot = slot;
                sourceData.sourceGameObject = target;
                sourceData.secondarySourceGameObject = slot.sourceData.sourceGameObject;
                trigger.TriggerWhenTargettedByEnemy(skillInfo, sourceData);
            }
        }
    }

    void PrintBehaviourList(SkillInfo skillInfo)
    {
        Debug.Log("Skill Behaviours:");
        for (int i = 0; i < skillInfo.behaviours.Count; i++)
        {
            Debug.Log("[" + i + "] " + skillInfo.behaviours[i].SkillBehaviour + ".");
        }
    }


    
    public GameObject isProtectedBy(GameObject obj)
    {
        if (obj == null) return null;

        foreach (TileEffectSlot effectSlot in obj.GetComponent<GridObject>().baseTile.GetComponent<GridTile>().tileEffects)
        {
            if (effectSlot.effect == tileProtectionEffect)
            {
                if (effectSlot.unitTypeAffected == obj.GetComponent<UnitBattle>().unitType)
                {
                    return effectSlot.sourceData.sourceGameObject;
                }
            }
        }
        return null;
    }

    public GameObject SummonCircle(SkillInfo skillInfo)
    {
        GameObject summoningCircle = ColorHelper.Instance.GetSummoningCircle(skillInfo.baseSkill.skill.color);
        GameObject circle = Instantiate(summoningCircle, skillInfo.source.transform.position, Quaternion.identity, skillInfo.source.transform);
        circle.GetComponent<SpriteRenderer>().sortingOrder = skillInfo.source.GetComponent<SpriteRenderer>().sortingOrder - 1;
        return circle;
    }

}

public class GodAnimationInfo
{
    public float timeUntilImpact;
    public float timeUntilFinish;
}

public class SkillTargetInfo
{
    public GameObject target;
    public bool isCrit;
    public bool isBlock;
    public int physicalDamageTaken;
    public int magicalDamageTaken;
    public bool blocked;
    public SkillTargetInfo(GameObject obj)
    {
        target = obj;
    }
    public SkillTargetInfo()
    {

    }
}



public class SkillInfo
{
    public ActiveSkillSlot baseSkill;
    public GameObject source;
    public List<SkillBehaviourSlot> behaviours;
    public SkillTargetInfo primaryTarget;
    public List<SkillTargetInfo> secondaryTargets;
    public int manaCost;
    public bool success;
    public GodAnimationInfo animationInfo;
    public SkillDataLog dataLog;
    public bool isTrigger;
    public SkillInfo(GameObject isource, ActiveSkillSlot skill, GameObject target)
    {
        baseSkill = skill;
        source = isource;
        List<SkillBehaviourSlot> behaviourSlots = new List<SkillBehaviourSlot>();
        foreach (SkillBehaviour behaviour in skill.skill.skillBehaviours)
        {
            SourceData sourceData = new SourceData();
            sourceData.sourceGameObject = isource;
            SkillBehaviourSlot slot = new SkillBehaviourSlot(behaviour, sourceData);
            behaviourSlots.Add(slot);
        }
        SetupBehaviours(behaviourSlots);
        secondaryTargets = new List<SkillTargetInfo>();
        primaryTarget = new SkillTargetInfo(target);
        manaCost = skill.skill.manaCost;
        dataLog = new SkillDataLog();
        animationInfo = new GodAnimationInfo();
        if (skill.skill.animationObject == null) return;
        GodAnimation godAnimation = skill.skill.animationObject.GetComponent<GodAnimation>();
        animationInfo.timeUntilImpact = godAnimation.durationToImpact;
        animationInfo.timeUntilFinish = godAnimation.duration;

        
    }
    public SkillInfo(GameObject isource, ActiveSkillSlot skill)
    {
        baseSkill = skill;
        source = isource;
        List<SkillBehaviourSlot> behaviourSlots = new List<SkillBehaviourSlot>();
        foreach(SkillBehaviour behaviour in skill.skill.skillBehaviours)
        {
            SourceData sourceData = new SourceData();
            sourceData.sourceGameObject = isource;
            SkillBehaviourSlot slot = new SkillBehaviourSlot(behaviour, sourceData);
            behaviourSlots.Add(slot);
        }
        SetupBehaviours(behaviourSlots);
        primaryTarget = new SkillTargetInfo();
        secondaryTargets = new List<SkillTargetInfo>();
        dataLog = new SkillDataLog();
        manaCost = skill.skill.manaCost;

        animationInfo = new GodAnimationInfo();
        if (skill.skill.animationObject == null) return;
        GodAnimation godAnimation = skill.skill.animationObject.GetComponent<GodAnimation>();
        animationInfo.timeUntilImpact = godAnimation.durationToImpact;
        animationInfo.timeUntilFinish = godAnimation.duration;

    }
    public SkillInfo(AIActionInfo info)
    {
        baseSkill = info.skill;
        source = info.source;
        List<SkillBehaviourSlot> behaviourSlots = new List<SkillBehaviourSlot>();
        foreach (SkillBehaviour behaviour in info.skill.skill.skillBehaviours)
        {
            SourceData sourceData = new SourceData();
            sourceData.sourceGameObject = info.source;
            SkillBehaviourSlot slot = new SkillBehaviourSlot(behaviour, sourceData);
            behaviourSlots.Add(slot);
        }
        SetupBehaviours(behaviourSlots);
        secondaryTargets = new List<SkillTargetInfo>();
        primaryTarget = new SkillTargetInfo(info.target);
        dataLog = new SkillDataLog();
        manaCost = info.skill.skill.manaCost;

        animationInfo = new GodAnimationInfo();
        if (info.skill.skill.animationObject == null) return;
        GodAnimation godAnimation = info.skill.skill.animationObject.GetComponent<GodAnimation>();
        animationInfo.timeUntilImpact = godAnimation.durationToImpact;
        animationInfo.timeUntilFinish = godAnimation.duration;

    }

    public void AddBehaviour(SkillBehaviourSlot behaviourSlot)
    {
        //Debug.Log("adding behaviour " + behaviourSlot.SkillBehaviour);
        if (behaviours == null)
            behaviours = new List<SkillBehaviourSlot>();
        if (behaviours.Count == 0)
        {
            behaviours.Add(behaviourSlot);
            return;
        } 
        for (int i = 0; i < behaviours.Count; i++)
        {
            if(behaviourSlot.SkillBehaviour.order < behaviours[i].SkillBehaviour.order)
            {
                behaviours.Insert(i, behaviourSlot);
                return;
            }
        }
        behaviours.Add(behaviourSlot);
    }

    void SetupBehaviours(List<SkillBehaviourSlot> list)
    {
        foreach(SkillBehaviourSlot item in list)
        {
            AddBehaviour(item);
        }
    }

    public SkillTargetInfo FindTarget(GameObject obj)
    {
        if (primaryTarget.target == obj)
        {
            return primaryTarget;
        }
        foreach (SkillTargetInfo target in secondaryTargets)
        {
            if (target.target == obj)
            {
                return target;
            }
        }
        return null;
    }

    public List<GameObject> GetHighlightTargets()
    {
        List<GameObject> list = new List<GameObject>();
        list.Add(this.source);
        if (this.primaryTarget != null) list.Add(primaryTarget.target);
        foreach(SkillTargetInfo targetInfo in this.secondaryTargets)
        {
            if (targetInfo.target == null) continue;
            list.Add(targetInfo.target);
        }
        return list;
    }
}

[System.Serializable]
public abstract class SkillBehaviour : ScriptableObject
{
    public abstract IEnumerator Co_Behave(SkillInfo skillInfo, SourceData sourceData);
    public int order;
    public AnimationInfo animation;
    public IEnumerator Co_PlayAnimation(SkillInfo skillInfo, SourceData sourceData)
    {
        if (animation.animationObject == null) yield return null;

        GameObject targetObject;
        if (animation.targetsSelf)
            targetObject = sourceData.sourceGameObject;
        else
            targetObject = sourceData.secondarySourceGameObject;

        //check direction
        bool flipDirection = false;
        if (targetObject.transform.position.x > skillInfo.source.transform.position.x)
            flipDirection = true;
        if (flipDirection)
        {
            targetObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            targetObject.GetComponent<SpriteRenderer>().flipX = false;
        }


        if (animation.type == AnimationType.simple)
        {
            InstantiateAnimationObject(targetObject, flipDirection);
        }
        else if (animation.type == AnimationType.timedImpact)
        {
            float remainingTime = skillInfo.animationInfo.timeUntilImpact - SkillHandler.Instance.elapsedTime;
            yield return new WaitForSeconds(remainingTime - animation.timeToImpact);
            InstantiateAnimationObject(targetObject, flipDirection);
        }
    }
    private void InstantiateAnimationObject(GameObject target, bool flipDirection)
    {
        Vector3 targetPos = target.transform.position;
        targetPos.y += 1f;
        GameObject animObj = GameObject.Instantiate(animation.animationObject, targetPos, Quaternion.identity);
        SpriteRenderer animRenderer = animObj.GetComponent<SpriteRenderer>();
        SpriteRenderer sourceRenderer = target.GetComponent<SpriteRenderer>();
        animRenderer.sortingOrder = sourceRenderer.sortingOrder + 1;
        if (!animation.noFlip && flipDirection)
            animObj.GetComponent<SpriteRenderer>().flipX = true;
    }

    public List<SkillTargetInfo> SelectAllTargets(SkillInfo skillInfo, SourceData sourceData)
    {
        List<SkillTargetInfo> list = new List<SkillTargetInfo>();
        if (skillInfo.primaryTarget.target != null)
        {
            if(!list.Contains(skillInfo.primaryTarget))
                list.Add(skillInfo.primaryTarget);
        }
        foreach (SkillTargetInfo secondaryTargetInfo in skillInfo.secondaryTargets)
        {
            if (secondaryTargetInfo.target == null) continue;
            if (list.Contains(secondaryTargetInfo)) continue;
            list.Add(secondaryTargetInfo);
        }
        return list;
    }

    public SkillTargetInfo GetSourceTargetInfo(SkillInfo skillInfo, SourceData sourceData)
    {
        SkillTargetInfo sourceTargetInfo = null;
        if (skillInfo.primaryTarget.target == sourceData.sourceGameObject)
            sourceTargetInfo = skillInfo.primaryTarget;

        foreach (SkillTargetInfo targetInfo in skillInfo.secondaryTargets)
        {
            if (targetInfo.target == sourceData.sourceGameObject)
                sourceTargetInfo = targetInfo;
        }
        return sourceTargetInfo;
    }


}

public class SkillBehaviourSlot
{
    public SkillBehaviour SkillBehaviour;
    public SourceData source;

    public SkillBehaviourSlot(SkillBehaviour iSkillBehaviour, SourceData isource)
    {
        SkillBehaviour = iSkillBehaviour;
        source = isource;
    }
}
[System.Serializable]
public class SourceData
{
    public GameObject sourceGameObject;
    public GameObject secondarySourceGameObject;
    public SkillSlot sourceSkillSlot;
    public StatusEffectSlot sourceEffectSlot;
    public TileEffectSlot sourceTileEffectSlot;
}

public class SkillDataLog
{
    public bool isCrit;
    public bool isMiss;
    public bool isUnblockable;
    public int physicalDamageDealt;
    public int magicalDamageDealt;
    public int buffsApplied;
    public int buffsRemoved;
    public int debuffsApplied;
    public int debuffsRemoved;
    public int healAmount;
    public SkillDataLog()
    {
        isCrit = false;
        isMiss = false;
        isUnblockable = false;
        physicalDamageDealt = 0;
        magicalDamageDealt = 0;
        buffsApplied = 0;
        buffsRemoved = 0;
        debuffsApplied = 0;
        debuffsRemoved = 0;
        healAmount = 0;
    }
}