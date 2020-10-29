using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Inventory", menuName ="Inventory")]
public class CharacterInventory : ScriptableObject, ISerializationCallbackReceiver
{
    public SkillDatabase passiveSkillDatabase;
    public SkillDatabase activeSkillDatabase;
    public List<Character> characters;

    public void OnAfterDeserialize()
    {
        UpdateSkillsByID();
    }

    public void UpdateSkillsByID()
    {
        foreach (Character character in characters){
            foreach (ActiveSkillData skill in character.activeSkills)
            {
                if (!activeSkillDatabase.GetSkill.ContainsKey(skill.id))
                    return;
                skill.skill = (ActiveSkill) activeSkillDatabase.GetSkill[skill.id];
            }
            foreach (PassiveSkillData skill in character.passiveSkills)
            {
                if (!passiveSkillDatabase.GetSkill.ContainsKey(skill.id))
                    return;
                skill.skill = (PassiveSkill)passiveSkillDatabase.GetSkill[skill.id];
            }
        }
    }

    public void OnBeforeSerialize()
    {

    }



}
