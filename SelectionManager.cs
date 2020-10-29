using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] GameObject selectionTilePrefab;
    [SerializeField] GameObject selectionTileParent;

    private static SelectionManager _instance;

    public static SelectionManager Instance { get { return _instance; } }

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
    bool selectingUnit;
    UnitType unitType;
    List<GameObject> targetTiles;

    public IEnumerator Co_SelectUnit(List<GameObject> tiles, UnitType type)
    {
        if (!GameState.Instance.TryStartMinorState(GameState.minorStates.WaitingForInput))
        {
            Debug.LogError("Couldn't start minor state WaitingForInput.");
        }
        foreach(GameObject tile in tiles)
        {
            GridManager.Instance.InstantiateAtTile(selectionTilePrefab, tile, selectionTileParent);
        }

        unitType = type;
        selectingUnit = true;
        targetTiles = tiles;
        
        while(selectingUnit == true)
        {
            yield return null;
        }
    }
    void DestroySelectionTiles()
    {
        foreach (Transform child in selectionTileParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if (selectingUnit)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                GameState.Instance.TryStopMinorState(GameState.minorStates.WaitingForInput);
                GameState.Instance.target = null;
                selectingUnit = false;
                DestroySelectionTiles();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 0f);
                foreach(RaycastHit2D hit in hits)
                {
                    UnitBattle unitBattle = hit.collider.GetComponent<UnitBattle>();
                    if (unitBattle == null) return;
                    if (unitBattle.unitType != unitType) return;
                    GridObject obj = hit.collider.gameObject.GetComponent<GridObject>();
                    for (int i = 0; i < targetTiles.Count; i++)
                    {
                        if (targetTiles.Contains(obj.baseTile))
                        {
                            GameState.Instance.TryStopMinorState(GameState.minorStates.WaitingForInput);
                            GameState.Instance.target = hit.collider.gameObject;
                            selectingUnit = false;
                            DestroySelectionTiles();
                        }
                    }
                }
            }
        }

    }

}
