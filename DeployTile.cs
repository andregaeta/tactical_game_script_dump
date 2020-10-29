using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployTile : MonoBehaviour
{
    public GameObject selectedUnit;
    bool hasGhost;
    GameObject unitGhost;
    GameObject baseTile;

    private void Start()
    {
        baseTile = this.GetComponent<GridObject>().baseTile;
    }
    private void OnMouseDown()
    {
        UnitDeployer.Instance.DeployUnit(baseTile);
    }
    private void OnMouseOver()
    {
        if (hasGhost)
            return;
        hasGhost = true;
        unitGhost = GridManager.Instance.InstantiateAtTile(selectedUnit.GetComponent<GhostSprite>().ghostSpritePrefab, baseTile, this.gameObject);
        unitGhost.GetComponent<GridObject>().SetLayerOrder();
    }

    private void OnMouseExit()
    {
        hasGhost = false;
        Destroy(unitGhost);
    }
}
