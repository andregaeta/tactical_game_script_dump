using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName ="Enemy")]
public class Enemy : ScriptableObject
{



    [SerializeField] public string displayName;

    //vessel
    //devotion

    [SerializeField] public int level;

    [SerializeField] public GameObject prefab;

    [SerializeField] public UnitStats unitStats;
    [SerializeField] public int maxHP;

    [SerializeField] public ActiveSkill[] activeSkills;
    [SerializeField] public PassiveSkill[] passiveSkills;


}