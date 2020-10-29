using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character 
{
    [SerializeField] public string displayName;

    //vessel
    //devotion

    [SerializeField] public int level;
    [SerializeField] public float exp;

    [SerializeField] public GameObject prefab;

    [SerializeField] public UnitStats unitStats;
    [SerializeField] public int maxHP;

    [SerializeField] public ActiveSkillData[] activeSkills;
    [SerializeField] public PassiveSkillData[] passiveSkills;


    public Character(string name)
    {
        displayName = name;
        level = 1;
        exp = 0;
    }
    public Character(string name, int level, float exp, UnitStats stats, ActiveSkillData[] actives, PassiveSkillData[] passives)
    {
        displayName = name;
        this.level = level;
        this.exp = exp;
        unitStats = stats;
        activeSkills = actives;
        passiveSkills = passives;
    }


}


