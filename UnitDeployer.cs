using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDeployer : MonoBehaviour
{
    [SerializeField] GameObject deployTile;
    [SerializeField] GameObject deployTileParent;
    [SerializeField] GameObject unitParent;
    bool deploying;
    GameObject selectedUnitPrefab;
    int selectedUnitID;

    private static UnitDeployer _instance;

    public static UnitDeployer Instance { get { return _instance; } }

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

    public void DeployUnit(GameObject tile)
    {
        if (!deploying)
        {
            FinishDeployment();
            return;
        }
        GameObject unit = GridManager.Instance.InstantiateAtTile(selectedUnitPrefab, tile, unitParent);
        CharacterBattle battleStats = unit.GetComponent<CharacterBattle>();
        battleStats.id = selectedUnitID;
        battleStats.SetupBattleStats();
        unit.GetComponent<GridObject>().SetupToTile(tile);
        EventHandler.unitMovement.Invoke(new UnitMovementData(unit, tile, tile));
        FinishDeployment();
    }

    public void SelectDeployUnitByID(GameObject unitPrefab, int id)
    {
        deploying = true;
        selectedUnitID = id;
        selectedUnitPrefab = unitPrefab;

        DestroyDeployTiles();
        DisplayDeployTiles();
    }
    /*
    public void SelectDeployUnit(GameObject unitPrefab)
    {
        DestroyDeployTiles();
        DisplayDeployTiles();

    }
    */
    public void DestroyDeployTiles()
    {
        foreach(Transform child in deployTileParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayDeployTiles()
    {
        foreach(GameObject tile in GridManager.Instance.Grid)
        {
            if (tile.GetComponent<GridTile>().occupiedBy == null)
            {
                GameObject instancedDeployTile = GridManager.Instance.InstantiateAtTile(deployTile, tile, deployTileParent);
                instancedDeployTile.GetComponent<DeployTile>().selectedUnit = selectedUnitPrefab;
            }
        }
    }

    public void FinishDeployment()
    {
        DestroyDeployTiles();
        selectedUnitPrefab = null;
        selectedUnitID = 0;
        deploying = false;
    }

}
