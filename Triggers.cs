using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerOnAction 
{
    void TriggerOnAction(SkillInfo info, SourceData sourceData);
}
public interface ITriggerOnDamage
{
    void TriggerOnDamage(SkillInfo info, SourceData sourceData);
}
public interface ITriggerOnApplication
{
    void TriggerOnApplication(StatusEffectSlotData data);
    void TriggerOnRemoval(StatusEffectSlotData data);
}

public interface ITriggerOnStack
{
    void TriggerOnStack(StatusEffectSlotData data, GameObject sourceGameObject);
}

public interface ITriggerOnTick
{
    void TriggerOnTick();
}

public interface ITriggerWhenTargettedByEnemy
{
    void TriggerWhenTargettedByEnemy(SkillInfo info, SourceData sourceData);
}
public interface ITriggerWhenDamaged
{
    void TriggerWhenDamaged(SkillInfo info, SourceData sourceData);
}

public interface ITriggerOnWalk
{
    void TriggerOnWalk(UnitMovementData data, SourceData sourceData);
}

public interface ITriggerOnNewTurn
{
    void TriggerOnNewTurn(GameObject target, SourceData data);
}
public enum AnimationType
{
    simple, 
    timedImpact,
    complex
}


