using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;

    public static BattleManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public List<GameObject> charList;
    public List<GameObject> enemyList;
    IEnumerator Co_StartEnemyTurn()
    {
        Debug.Log("Enemy Turn");
        foreach (GameObject enemy in enemyList)
        {
            UnitBattle unitBattle = enemy.GetComponent<UnitBattle>();
            unitBattle.SetupNewTurn();
        }

        GameState.Instance.MajorState = GameState.majorStates.EnemyTurn;
        foreach (GameObject enemy in enemyList)
        {
            
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            AIActionInfo info = enemyAI.enemyAI.CalculateAction(enemy);
            if (info.success == false)
            {
                continue;
            }
            GameState.Instance.ActiveEnemy = enemy;
            GameState.Instance.CameraTarget = enemy;
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(UnitMovementHandler.Instance.Co_MoveEnemyToTile(enemy, info.targetTile));

            if (!enemyAI.enemyAI.CheckIfInRange(info)) continue;

            yield return StartCoroutine(info.skill.Co_OnSkillUse(info));
            //SkillInfo skillInfo = new SkillInfo(info);
            //yield return StartCoroutine(SkillHandler.Instance.Co_HandleBehaviour(skillInfo));
        }
        GameState.Instance.ActiveEnemy = null;
        GameState.Instance.CameraTarget = null;
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        Debug.Log("Player Turn");
        foreach (GameObject character in charList)
        {
            UnitBattle unitBattle = character.GetComponent<UnitBattle>();
            unitBattle.SetupNewTurn();
        }
        GameState.Instance.MajorState = GameState.majorStates.PlayerTurn;
    }



    public void StartEnemyTurn()
    {
        StartCoroutine(Co_StartEnemyTurn());
    }

    /*
    IEnumerator Co_OldEnemyAI()
    {
        GameState.Instance.MajorState = GameState.majorStates.EnemyTurn;
        foreach (GameObject enemy in enemyList)
        {

            EnemyAI enemyComponent = enemy.GetComponent<EnemyAI>();
            enemyComponent.ChooseTarget();
            if (enemyComponent.target == null)
                continue;
            GameState.Instance.ActiveEnemy = enemy;
            enemyComponent.ChooseTargetTile();

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(UnitMovementHandler.Instance.Co_MoveEnemyToTile(enemy, enemyComponent.targetTile));
        }
        GameState.Instance.ActiveEnemy = null;
        GameState.Instance.MajorState = GameState.majorStates.PlayerTurn;
    }
    */
}
