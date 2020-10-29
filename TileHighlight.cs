using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{

    [SerializeField] GameObject highlightTile;
    public GameObject instancedHighlightTile;
    public bool isHighlighted;
    [SerializeField] GameObject parent;

    private void Start()
    {
        parent = GameObject.Find("Highlight Tiles");
    }

    private void OnMouseOver()
    {
        if (GameState.Instance.MinorState == GameState.minorStates.MovingCamera || GameState.Instance.MinorState == GameState.minorStates.DraggingUnit)
        {
            if (isHighlighted)
                DestroyTiles();
            return;
        }
        if (isHighlighted)
            return;


        isHighlighted = true;

        instancedHighlightTile = GridManager.Instance.InstantiateAtTile(highlightTile, this.gameObject, parent);
    }
    private void OnMouseExit()
    {
        isHighlighted = false;
        DestroyTiles();
    }

    private void DestroyTiles()
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
