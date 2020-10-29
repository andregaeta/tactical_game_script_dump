using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAIObject : ScriptableObject
{
    public abstract AIActionInfo CalculateAction(GameObject source);

    public GameObject GetClosestUnit(AIActionInfo info)
    {
        Pathfinding pathfinder = new Pathfinding();
        List<NearbyEntityData> unitList = pathfinder.GetUnitsAround(info.source);
        if (unitList.Count <= 0) return null;
        NearbyEntityData closestUnitData = unitList[0];
        foreach (NearbyEntityData entityData in unitList)
        {
            if (entityData.distance < closestUnitData.distance)
            {
                closestUnitData = entityData;
            }
        }
        return closestUnitData.entity;
    }
    public GameObject ChooseTargetTile(AIActionInfo info)
    {
        Pathfinding pathfinder = new Pathfinding();
        List<GameObject> list = pathfinder.FindTilesAtDistance(info.target.GetComponent<GridObject>().baseTile.GetComponent<GridTile>(), info.range);
        if (list == null)
            return null;
        GameObject targetTile = pathfinder.GetClosestTile(info.source, list);
        return targetTile;
    }
    public bool CheckIfInRange(AIActionInfo info)
    {
        Pathfinding pathfinder = new Pathfinding();
        GridObject source =  info.source.GetComponent<GridObject>();
        GridObject target = info.target.GetComponent<GridObject>();
        int startI = source.i;
        int startJ = source.j;
        int endI = target.i;
        int endJ = target.j;

        int distance = pathfinder.CalculateDistance(startI, startJ, endI, endJ);
        return distance > info.range ? false : true;
    }
}




public class AIActionInfo
{
    public GameObject source;
    public GameObject target;
    public ActiveSkillSlot skill;
    public int range;
    public GameObject targetTile;
    public bool success;
}
