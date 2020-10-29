using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventHandler 
{
    public static MovementEvent unitStep = new MovementEvent();
    public static MovementEvent unitMovement = new MovementEvent();
    public static UnitSelectionEvent characterSelection = new UnitSelectionEvent();
    public static UnitSelectionEvent enemySelection = new UnitSelectionEvent();
    public static UnityEvent characterDeselection = new UnityEvent();
    public static UnityEvent enemyDeselection = new UnityEvent();
    public static UnityEvent animationStart = new UnityEvent();
    public static SkillUseEvent skillUse = new SkillUseEvent();
}

public class SkillUseEvent : UnityEvent<SkillInfo> { }
public class UnitSelectionEvent : UnityEvent<UnitSelectionData> { }
public class MovementEvent : UnityEvent<UnitMovementData> { }
public class UnitMovementData
{
    public GameObject unit;
    public GameObject initialTile;
    public GameObject finalTile;

    public UnitMovementData(GameObject unit, GameObject initialTile, GameObject finalTile)
    {
        this.unit = unit;
        this.initialTile = initialTile;
        this.finalTile = finalTile;
    }
}

public class UnitSelectionData
{
    public GameObject selectedUnit;

    public UnitSelectionData(GameObject unit)
    {
        selectedUnit = unit;
    }
}
