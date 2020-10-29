using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattle : UnitBattle
{
    Enemy enemy;
    public GameObject fearedBy;
    public GameObject tauntedBy;

    private void Awake()
    {
        unitType = UnitType.enemy;
        EventHandler.skillUse.AddListener(OnSkillUse);
        EventHandler.unitMovement.AddListener(OnMovement);
        EventHandler.unitStep.AddListener(OnStep);
    }

    void Start()
    {
        BattleManager.Instance.enemyList.Add(this.gameObject);
        attackRange = 1;
        moveRange = 3;
    }
   
    public void SetupBattleStats(Enemy ienemy)
    {
        enemy = ienemy;
        displayName = enemy.displayName;
        this.name = displayName;
        level = enemy.level;

        //stats = character.unitStats;
        stats.strenght.AddBaseStat(enemy.unitStats.strenght.GetFinalStat());
        stats.intelligence.AddBaseStat(enemy.unitStats.intelligence.GetFinalStat());
        stats.defense.AddBaseStat(enemy.unitStats.defense.GetFinalStat());
        stats.resistance.AddBaseStat(enemy.unitStats.resistance.GetFinalStat());
        stats.critical.AddBaseStat(enemy.unitStats.critical.GetFinalStat());
        stats.faith.AddBaseStat(enemy.unitStats.faith.GetFinalStat());

        statusEffectSlots.Clear();
        activeSkills = new List<ActiveSkillSlot>();
        passiveSkills = new List<PassiveSkillSlot>();
        foreach (ActiveSkill active in enemy.activeSkills)
        {
            activeSkills.Add(new ActiveSkillSlot(active));
        }
        foreach (PassiveSkill passive in enemy.passiveSkills)
        {
            passiveSkills.Add(new PassiveSkillSlot(passive));
            if(passive is ITriggerOnApplication)
            {
                ITriggerOnApplication trigger = passive as ITriggerOnApplication;
                trigger.TriggerOnApplication(new StatusEffectSlotData(this.gameObject));
            }
        }

        HP.maxHP = enemy.maxHP;
        HP.currentHP = HP.maxHP;
        healthBarUI.SetMaxHealth(HP.maxHP);
        healthBarUI.SetCurrentHealth(HP.currentHP);

        actionCounter = maxActions;
        actionBarUI.SetMaxActions(maxActions);
        actionBarUI.SetCurrentActions(actionCounter);

        MP = new ManaPoints();
        MP.maxMana = 3;
        MP.currentMana = 3;
        manaBarUI.SetCurrentMana(MP.currentMana);
    }

    private void OnMouseDown()
    {
        GameState.Instance.TrySelectDisplayEnemy(this.gameObject);
    }

}
