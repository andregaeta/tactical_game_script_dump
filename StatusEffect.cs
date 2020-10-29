using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : ScriptableObject
{
    [SerializeField] public string displayName;
    [SerializeField] Sprite icon;

    [SerializeField] StatusEffectType statusEffectType;
    [SerializeField] public StatusEffectStackingType stackingType;
    [SerializeField] public bool isPermanent;
    [SerializeField] public bool isStack;

    [TextArea(15, 20)]
    public string description;


}
public enum StatusEffectStackingType
{
    vertical,
    horizontal,
    stackCount,
    nothing
}

public enum StatusEffectType
{
    buff,
    debuff,
    neutral
}

[System.Serializable]
public class StatusEffectSlot
{
    [SerializeField] public StatusEffect statusEffect;
    [SerializeField] public StatusEffectSlotData data;

    public StatusEffectSlot(StatusEffect effect, StatusEffectSlotData idata)
    {
        statusEffect = effect;
        data = idata;
    }

    public void ReduceDuration(int amount)
    {
        data.duration -= amount;
        if (data.duration < 0)
        {
            data.duration = 0;
        }
        if (data.duration == 0)
        {
            RemoveStatus();
        }
        if(this.statusEffect is ITriggerOnTick)
        {
            ITriggerOnTick trigger = this.statusEffect as ITriggerOnTick;
            trigger.TriggerOnTick();
        }
    }
    public void RemoveStatus()
    {
        if (this.statusEffect is ITriggerOnApplication)
        {
            ITriggerOnApplication trigger = this.statusEffect as ITriggerOnApplication;
            trigger.TriggerOnRemoval(data);
        }
        UnitBattle unitBattle = data.user.GetComponent<UnitBattle>();
        unitBattle.statusEffectSlots.Remove(this);
    }
}
[System.Serializable]
public class StatusEffectSlotData
{
    public int stacks;
    public int duration;
    public bool isPermanent;
    public GameObject source;
    public GameObject user;
    public StatusEffectSlotData(int istacks, int iduration, bool ipermanent, GameObject isource, GameObject iuser)
    {
        stacks = istacks;
        duration = iduration;
        isPermanent = ipermanent;
        source = isource;
        user = iuser;
    }
    public StatusEffectSlotData(GameObject iuser)
    {
        user = iuser;
    }

}