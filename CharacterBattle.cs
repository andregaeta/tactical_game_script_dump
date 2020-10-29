using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : UnitBattle
{

    CharacterInventory inventory;
    Character character;

    public int id;

    //devotion
    //vessel
    //element
    //buffs
    //debuffs
    private void Awake()
    {
        unitType = UnitType.character;
        EventHandler.skillUse.AddListener(OnSkillUse);
        EventHandler.unitMovement.AddListener(OnMovement);
        EventHandler.unitStep.AddListener(OnStep);

        inventory = GameState.Instance.activeCharacterInventory;
        attackRange = 3;
        moveRange = 3;

    }

    public void SetupBattleStats()
    {
        

        character = inventory.characters[id];
        displayName = character.displayName;
        this.name = displayName;
        level = character.level;

        //stats = character.unitStats;
        stats.strenght.AddBaseStat(character.unitStats.strenght.GetFinalStat());
        stats.intelligence.AddBaseStat(character.unitStats.intelligence.GetFinalStat());
        stats.defense.AddBaseStat(character.unitStats.defense.GetFinalStat());
        stats.resistance.AddBaseStat(character.unitStats.resistance.GetFinalStat());
        stats.critical.AddBaseStat(character.unitStats.critical.GetFinalStat());
        stats.faith.AddBaseStat(character.unitStats.faith.GetFinalStat());

        statusEffectSlots.Clear();
        activeSkills = new List<ActiveSkillSlot>();
        passiveSkills = new List<PassiveSkillSlot>();
        foreach(ActiveSkillData active in character.activeSkills)
        {
            activeSkills.Add(new ActiveSkillSlot(active.skill));
        }
        foreach(PassiveSkillData passive in character.passiveSkills)
        {
            passiveSkills.Add(new PassiveSkillSlot(passive.skill));
            if (passive.skill is ITriggerOnApplication)
            {
                Debug.Log("triggering passive application");
                ITriggerOnApplication trigger = passive.skill as ITriggerOnApplication;
                trigger.TriggerOnApplication(new StatusEffectSlotData(this.gameObject));
            }
        }

        HP.maxHP = character.maxHP;
        HP.currentHP = HP.maxHP;
        healthBarUI.SetMaxHealth(HP.maxHP);
        healthBarUI.SetCurrentHealth(HP.currentHP);

        actionCounter = maxActions;
        actionBarUI.SetMaxActions(maxActions);
        actionBarUI.SetCurrentActions(actionCounter);

        movementCounter = maxMoves;
        moveBarUI.SetMaxMoves(maxMoves);
        moveBarUI.SetCurrentMoves(movementCounter);

        MP = new ManaPoints();
        MP.maxMana = 3;
        MP.maxMana += (int)(stats.faith.GetFinalStat() / 10);
        MP.currentMana = 3;
        manaBarUI.SetCurrentMana(MP.currentMana);


    }




}
