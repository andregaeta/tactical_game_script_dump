using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyAIObject enemyAI;

    /*

    EnemyBattle enemyBattle;

    public GameObject target;
    public GameObject targetTile;
    bool isTaunted;

    private void Awake()
    {
        enemyBattle = this.gameObject.GetComponent<EnemyBattle>();
    }

    public void ChooseTarget() //Later on, make it so units have visionRange and won't target anything that is too far away.
    {
        if (isTaunted) return;

        target = GetClosestUnit();

        //Test();
    }

    public void ChooseTargetTile()
    {
        Pathfinding pathfinder = new Pathfinding();
        List<GameObject> list = pathfinder.FindTilesAtDistance(target.GetComponent<GridObject>().baseTile.GetComponent<GridTile>(), enemyBattle.attackRange);
        targetTile = pathfinder.GetClosestTile(this.gameObject, list);
    }

    GameObject GetClosestUnit()
    {
        Pathfinding pathfinder = new Pathfinding();
        List<NearbyEntityData> unitList = pathfinder.GetUnitsAround(this.gameObject);
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

    public void Test()
    {
        Pathfinding pathfinder = new Pathfinding();
        List<GameObject> list = pathfinder.FindTilesAtDistance(target.GetComponent<GridObject>().baseTile.GetComponent<GridTile>(), 3);
        foreach (GameObject tile in list)
        {
            tile.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    */
}
