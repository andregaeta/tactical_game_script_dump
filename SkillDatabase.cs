using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Skill Database", menuName ="Database/Skill Database")]
public class SkillDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public List<Skill> skills;
    public Dictionary<Skill, int> GetID = new Dictionary<Skill, int>();
    public Dictionary<int, Skill> GetSkill = new Dictionary<int, Skill>();


    public void OnAfterDeserialize()
    {
        GetID = new Dictionary<Skill, int>();
        GetSkill = new Dictionary<int, Skill>();
        for (int i = 0; i < skills.Count; i++)
        {
            GetID.Add(skills[i], i);
            GetSkill.Add(i, skills[i]);
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void AddElement(Skill skill)
    {

        if (!skills.Contains(skill))
        {
            skills.Add(skill);
        }
    }

}
