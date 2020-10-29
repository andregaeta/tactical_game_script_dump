using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;

    public static GameState Instance { get { return _instance; } }

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

    private void Start()
    {
        MajorState = majorStates.PlayerTurn;
        MinorState = minorStates.None;
    }

    public GameObject SelectedMoveUnit;
    public GameObject SelectedDeployCharacter;
    public GameObject SelectedDisplayCharacter;
    public GameObject SelectedDisplayEnemy;
    public GameObject ActiveEnemy;
    public GameObject CameraTarget;
    public CharacterInventory activeCharacterInventory;
    public GameObject target;
    public bool mouseIsOnUI;
    public SkillDatabase passiveDatabase;
    public SkillDatabase activeDatabase;

    public enum majorStates
    {
        DeployPhase,
        PlayerTurn,
        EnemyTurn,
        Paused
    }
    public enum minorStates
    {
        DraggingUnit,
        MovingCamera,
        Animating,
        WaitingForInput,
        None
    }
    public majorStates MajorState;
    public minorStates MinorState;
    
   
    public bool TryStartMinorState(minorStates minorState)
    {
        if (MajorState != majorStates.PlayerTurn && minorState != minorStates.Animating)
            return false;
        switch (minorState)
        {
            case minorStates.DraggingUnit:
                if (MinorState != minorStates.None)
                    return false;
                else
                {
                    MinorState = minorState;
                    return true;
                }
            case minorStates.Animating:
                if (MinorState != minorStates.None)
                    return false;
                else
                {
                    EventHandler.animationStart.Invoke();
                    MinorState = minorState;
                    return true;
                }
            case minorStates.MovingCamera:
                if (MinorState != minorStates.None && MinorState != minorStates.WaitingForInput)
                    return false;
                else
                {
                    MinorState = minorState;
                    return true;
                }
            case minorStates.WaitingForInput:
                if (MinorState != minorStates.None)
                    return false;
                else
                {
                    MinorState = minorState;
                    return true;
                }
            case minorStates.None:
                Debug.Log("Use TrySTOPMinorState, dummy!");
                return false;
            default:
                return false;

        }
    }
    public bool TryStopMinorState(minorStates minorState)
    {
        if (MajorState != majorStates.PlayerTurn && minorState != minorStates.Animating)
            return false;
        if (MinorState != minorState)
            return false;
        MinorState = minorStates.None;
        return true;
    }

    public bool TrySelectDisplayCharacter(GameObject unit)
    {
        if (MajorState != majorStates.DeployPhase && MajorState != majorStates.PlayerTurn) return false;
        if (GameState.Instance.MinorState != GameState.minorStates.None) return false;
        EventHandler.characterSelection.Invoke(new UnitSelectionData(unit));
        SelectedDisplayCharacter = unit;
        return true;
    }
    public bool TryDeselectDisplayCharacter()
    {
        EventHandler.characterDeselection.Invoke();
        SelectedDisplayCharacter = null;
        return true;
    }
    public bool TrySelectDisplayEnemy(GameObject unit)
    {
        if (MajorState != majorStates.DeployPhase && MajorState != majorStates.PlayerTurn) return false;
        if (GameState.Instance.MinorState != GameState.minorStates.None) return false;
        EventHandler.enemySelection.Invoke(new UnitSelectionData(unit));
        SelectedDisplayEnemy = unit;
        return true;
    }
    public bool TryDeselectDisplayEnemy()
    {
        EventHandler.enemyDeselection.Invoke();
        SelectedDisplayEnemy = null;
        return true;
    }



    /*
    public bool TrySetDraggingUnit()
    {
        if (MajorState != majorStates.PlayerTurn)
            return false;
        if (MinorState != minorStates.None)
            return false;
        MinorState = minorStates.DraggingUnit;
        return true;
    }
  
    public bool TryStopDraggingUnit()
    {
        if (MajorState != majorStates.PlayerTurn)
            return false;
        if (MinorState != minorStates.DraggingUnit)
            return false;
        MinorState = minorStates.None;
        return true;
    }
    */

}
