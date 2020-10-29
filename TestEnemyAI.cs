using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New TestEnemyAI", menuName ="AI/Test Enemy AI")]
public class TestEnemyAI : EnemyAIObject
{
    public override AIActionInfo CalculateAction(GameObject source)
    {
        AIActionInfo info = new AIActionInfo();
        info.source = source;
        UnitBattle sourceBattle = info.source.GetComponent<UnitBattle>();

        info.target = GetClosestUnit(info);
        if(info.target == null)
        {
            info.success = false;
            return info;
        }
        info.skill = sourceBattle.activeSkills[0];
        info.range = info.skill.skill.range;
        info.targetTile = ChooseTargetTile(info);
        info.success = true;

        return info;
    }
}
