using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBattle : MonoBehaviour
{
    [SerializeField] public string displayName;
    [SerializeField] public int level;
    [SerializeField] public UnitStats stats;
    [SerializeField] public HealthPoints HP;
    [SerializeField] public ManaPoints MP;

    [SerializeField] public List<ActiveSkillSlot> activeSkills;
    [SerializeField] public List<PassiveSkillSlot> passiveSkills;

    [SerializeField] public List<StatusEffectSlot> statusEffectSlots;

    [SerializeField] public UnitType unitType;

    public int maxActions;
    public int maxMoves;
    [SerializeField] public int actionCounter;
    [SerializeField] public int movementCounter;

    public int attackRange;
    public int moveRange;

    public HealthBarUI healthBarUI;
    public ManaBarUI manaBarUI;
    public ActionBarUI actionBarUI;
    public MoveBarUI moveBarUI;


    public void OnSkillUse(SkillInfo skillInfo)
    {
        if (this.gameObject != skillInfo.source) return;
        if (skillInfo.baseSkill.skill.takesAction)
        {
            actionCounter--;
            actionBarUI.SetCurrentActions(actionCounter);
        }
        AddMana(-skillInfo.manaCost);
    }
    public void OnMovement(UnitMovementData data)
    {
        if (this.gameObject != data.unit) return;
        if (data.initialTile != data.finalTile)
            AddMovementCounter(-1);

        foreach (PassiveSkillSlot passiveSkillSlot in passiveSkills)
        {
            if(passiveSkillSlot.skill is ITriggerOnWalk)
            {
                Debug.Log("passive found: " + passiveSkillSlot.skill);
                ITriggerOnWalk trigger = passiveSkillSlot.skill as ITriggerOnWalk;
                SourceData sourceData = new SourceData();
                sourceData.sourceGameObject = this.gameObject;
                sourceData.sourceSkillSlot = passiveSkillSlot;
                trigger.TriggerOnWalk(data, sourceData);
            }
        }
    }

    public void OnStep(UnitMovementData data)
    {
        if (this.gameObject != data.unit) return;
        Debug.Log(data.initialTile + " -> " + data.finalTile);
        foreach (PassiveSkillSlot passiveSkillSlot in passiveSkills)
        {
            if(passiveSkillSlot.skill is ITriggerOnWalk)
            {
                Debug.Log("passive found: " + passiveSkillSlot.skill);
                ITriggerOnWalk trigger = passiveSkillSlot.skill as ITriggerOnWalk;
                SourceData sourceData = new SourceData();
                sourceData.sourceGameObject = this.gameObject;
                sourceData.sourceSkillSlot = passiveSkillSlot;
                trigger.TriggerOnWalk(data, sourceData);
            }
        }
    }
    public void AddMana(int amount)
    {
        MP.currentMana += amount;
        if (MP.currentMana < 0)
        {
            MP.currentMana = 0;
            Debug.LogError("Mana went below 0");
        }
        if (MP.currentMana > MP.maxMana)
        {
            MP.currentMana = MP.maxMana;
        }
        manaBarUI.SetCurrentMana(MP.currentMana);
        DynamicText.Instance.DisplayText("Mana -" + amount.ToString(), Color.blue, this.gameObject.transform.position);
    }

    public void AddManaNewTurn()
    {
        if (MP.currentMana < 3)
            AddMana(Mathf.Clamp(3 - MP.currentMana, 0, MP.maxMana - MP.currentMana));
        else if (MP.currentMana >= 3 && MP.currentMana < 6)
            AddMana(Mathf.Clamp(6 - MP.currentMana, 0, MP.maxMana - MP.currentMana));
        else if (MP.currentMana >= 6)
            AddMana(Mathf.Clamp(9 - MP.currentMana, 0, MP.maxMana - MP.currentMana));
        if (MP.currentMana > MP.maxMana)
            MP.currentMana = MP.maxMana;
    }

    public void AddActionCounter(int amount)
    {
        actionCounter += amount;
        actionBarUI.SetCurrentActions(actionCounter);
    }
    public void AddMovementCounter(int amount)
    {
        movementCounter += amount;
        moveBarUI.SetCurrentMoves(movementCounter);
    }
    public void AddCooldowns(int amount)
    {
        foreach (ActiveSkillSlot slot in activeSkills)
        {
            if (slot.cooldown > 0)
            {
                slot.AddCooldown(amount);
            }

        }

        foreach (PassiveSkillSlot slot in passiveSkills)
        {
            if (slot.cooldown > 0)
            {
                slot.AddCooldown(amount);
            }
        }
    }

    public void AddHealth(int damage, Color color)
    {
        HP.currentHP += damage;
        if(HP.currentHP < 0)
        {
            HP.currentHP = 0;
        }
        if(HP.currentHP > HP.maxHP)
        {
            HP.currentHP = HP.maxHP;
        }
        healthBarUI.SetCurrentHealth(HP.currentHP);
        
        if (damage > 0)
            color = Color.green;
        
        DynamicText.Instance.DisplayText(damage.ToString(), color, this.gameObject.transform.position);
    }
    private void AddStatusEffectToList(StatusEffectSlot effectSlot)
    {
        this.statusEffectSlots.Add(effectSlot);
        if (effectSlot.statusEffect is ITriggerOnApplication)
        {
            ITriggerOnApplication trigger = effectSlot.statusEffect as ITriggerOnApplication;
            trigger.TriggerOnApplication(effectSlot.data);
        }
    }
    public void AddStatusEffect(StatusEffectSlot effectSlot)
    {
        StatusEffectSlot alreadyApplied = null;


        foreach (StatusEffectSlot statusEffectSlot in statusEffectSlots)
        {
            if (statusEffectSlot.statusEffect == effectSlot.statusEffect)
            {
                alreadyApplied = statusEffectSlot;
            }
        }

        if (alreadyApplied == null)
        {
            AddStatusEffectToList(effectSlot);
            return;
        }

        switch (effectSlot.statusEffect.stackingType)
        {
            case StatusEffectStackingType.horizontal:
                {
                    AddStatusEffectToList(effectSlot);
                    break;
                }
            //create function that also triggers duration and stack increases
            case StatusEffectStackingType.nothing:
                {
                    if (effectSlot.data.duration <= alreadyApplied.data.duration) return;
                    alreadyApplied.data.duration = effectSlot.data.duration;
                    break;
                }
            case StatusEffectStackingType.vertical:
                {
                    alreadyApplied.data.duration += effectSlot.data.duration;
                    break;
                }
            case StatusEffectStackingType.stackCount:
                {
                    alreadyApplied.data.stacks += effectSlot.data.stacks;
                    if (alreadyApplied.statusEffect is ITriggerOnStack)
                    {
                        var trigger = alreadyApplied.statusEffect as ITriggerOnStack;
                        trigger.TriggerOnStack(alreadyApplied.data, effectSlot.data.source);
                    }
                    break;
                }

        }


    }

    public void SetupNewTurn()
    {
        foreach (StatusEffectSlot effect in statusEffectSlots.ToArray())
        {
            if (effect.statusEffect.isPermanent == true) continue;
            effect.ReduceDuration(1);
        }

        foreach(ActiveSkillSlot slot in activeSkills)
        {
            if (slot.cooldown > 0)
            {
                slot.AddCooldown(-1);
            }

        }

        foreach(PassiveSkillSlot slot in passiveSkills)
        {
            if (slot.cooldown > 0)
            {
                slot.AddCooldown(-1);
            }
        }

        foreach(TileEffectSlot slot in this.gameObject.GetComponent<GridObject>().baseTile.GetComponent<GridTile>().tileEffects)
        {
            if (slot.effect is ITriggerOnNewTurn)
            {
                var trigger = slot.effect as ITriggerOnNewTurn;
                var sourceData = new SourceData();
                trigger.TriggerOnNewTurn(this.gameObject, slot.sourceData);
            }
        }


        AddManaNewTurn();
        manaBarUI.SetCurrentMana(MP.currentMana);
        movementCounter = maxMoves;
        moveBarUI.SetCurrentMoves(movementCounter);
        actionCounter = maxActions;
        actionBarUI.SetCurrentActions(actionCounter);

    }

    public bool HasSkill(Skill skill)
    {
        foreach(PassiveSkillSlot slot in passiveSkills)
        {
            if (slot.skill == skill)
            {
                return true;
            }
        }
        foreach(ActiveSkillSlot slot in activeSkills)
        {
            if (slot.skill == skill)
            {
                return true;
            }
        }
        return false;
    }

    

}

public enum UnitType
{
    character,
    enemy,
    both
}