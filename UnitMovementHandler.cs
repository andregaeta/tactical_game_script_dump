using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementHandler : MonoBehaviour
{

    private static UnitMovementHandler _instance;

    public static UnitMovementHandler Instance { get { return _instance; } }

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

    [SerializeField] List<Vector3> vectorPath = new List<Vector3>();
    [SerializeField] List<GameObject> tilePath = new List<GameObject>();
    [SerializeField] int pathIndex;
    [SerializeField] GameObject moveTile;
    [SerializeField] GameObject moveTileParent;
    [SerializeField] GameObject destinationTile;
    [SerializeField] GameObject initialTile;
    [SerializeField] GameObject selectedUnit;
    private bool updatedLayer;

    Vector3 currentPath;
    Vector3 targetPath;
    Vector3 moveVector;
    private float directionModifier;

    /*
    public void SelectUnitToMove(GameObject selectedUnit)
    {
        DisplayMoveTiles(selectedUnit);
        this.selectedUnit = selectedUnit;
        GameState.Instance.SelectedMoveUnit = selectedUnit;
    }
    */
    public IEnumerator Co_MoveEnemyToTile(GameObject enemy, GameObject tile)
    {
        if (selectedUnit != null)
            yield break;
        this.selectedUnit = enemy;
        GameState.Instance.TryStartMinorState(GameState.minorStates.Animating);
        MoveUnit(tile);

        while(selectedUnit != null || GameState.Instance.MinorState == GameState.minorStates.Animating)
        {
            yield return null;
        }
    }

    public void DisplayMoveTiles(GameObject selectedUnit)
    {
        DestroyMoveTiles();
        GridObject unitPos = selectedUnit.GetComponent<GridObject>();
        GridTile unitTilePos = unitPos.baseTile.GetComponent<GridTile>();
        /*
        GridObject unitPos = selectedUnit.GetComponent<GridObject>();
        GridTile unitTilePos = unitPos.baseTile.GetComponent<GridTile>();

        foreach (GameObject tile in GridManager.Instance.Grid)
        {
            GridTile tilePos = tile.GetComponent<GridTile>();
            if (Mathf.Abs(tilePos.i - unitPos.i) + Mathf.Abs(tilePos.j - unitPos.j) <=  6) //movementRange
            {
                if (tilePos.occupiedBy == null && Mathf.Abs(tilePos.height - unitTilePos.height) <= 0.5) //jumpRange
                {
                    GameObject instancedMoveTile = GridManager.Instance.InstantiateAtTile(moveTile, tile, moveTileParent);
                }
            }
        }
        */

        Pathfinding pathfinder = new Pathfinding();
        List<GameObject> tilesAround =  pathfinder.FindTilesAroundToWalkTo(unitTilePos, 3);
        foreach(GameObject tile in tilesAround)
        {
            GameObject instancedMoveTile = GridManager.Instance.InstantiateAtTile(moveTile, tile, moveTileParent);
            tile.GetComponent<GridTile>().objectsOnTop.Add(instancedMoveTile);
        }
    }

    public void DestroyMoveTiles()
    {
        foreach (Transform child in moveTileParent.transform)
        {
            child.GetComponent<GridObject>().baseTile.GetComponent<GridTile>().objectsOnTop.Remove(child.gameObject);
            Destroy(child.gameObject);
        }
    }

    private void MoveUnit(GameObject destinationTile)
    {
        this.destinationTile = destinationTile;
        this.initialTile = selectedUnit.GetComponent<GridObject>().baseTile;
        //GridTile destinationTilePos = destinationTile.GetComponent<GridTile>();
        //Debug.Log("Moving unit" + GameState.Instance.SelectedMoveUnit + "to position [" + destinationTilePos.i + ", " + destinationTilePos.j + "]");
        Pathfinding pathfinder = new Pathfinding();
        tilePath = pathfinder.FindTilePath(selectedUnit, destinationTile);
        int moveRange = selectedUnit.GetComponent<EnemyBattle>().moveRange;
        if (tilePath.Count - 1 > moveRange)
        {
            List<GameObject> newTilePath = new List<GameObject>();
            for (int i = 0; i < moveRange + 1; i++)
            {
                newTilePath.Add(tilePath[i]);
            }
            tilePath = newTilePath;
            this.destinationTile = tilePath[moveRange];
        }

        vectorPath = pathfinder.FindVectorPath(selectedUnit, tilePath);
        pathIndex = 0;
        updatedLayer = false;
        UpdateStepPath();
    }

    private void Update()
    {
        if (vectorPath == null)
        {
            return;
        }

        if (pathIndex >= vectorPath.Count - 1)
        {

            StopMovement();
            return;
        }
        


        Vector3 unitPosition = new Vector3(selectedUnit.transform.position.x, selectedUnit.transform.position.y, 0);


        selectedUnit.transform.position += moveVector * Time.deltaTime * 6 * directionModifier;

        //Switching from one tile layer order to another
        if (!updatedLayer)
        {
            if (Mathf.Abs(Vector3.Distance(unitPosition, currentPath) - Vector3.Distance(unitPosition, targetPath)) < 0.1f)
            {
                UpdateCurrentTile();
            }
        }

        //reached next tile
        if (Vector3.Distance(unitPosition, targetPath) <= 2f * 6 * Time.deltaTime * directionModifier)
        {
            pathIndex++;
            updatedLayer = false;
            selectedUnit.transform.position = new Vector3(targetPath.x, targetPath.y, selectedUnit.transform.position.z);
            UpdateStepPath();
            EventHandler.unitStep.Invoke(new UnitMovementData(selectedUnit, tilePath[pathIndex - 1], tilePath[pathIndex]));
        }

    }

    private void UpdateStepPath()
    {
        if (pathIndex >= vectorPath.Count - 1)
            return;
        currentPath = vectorPath[pathIndex];
        targetPath = vectorPath[pathIndex + 1];
        moveVector = targetPath - currentPath;
        if (moveVector.x == 0 || moveVector.y == 0)
            directionModifier = 1 / 1.4f;
        else
            directionModifier = 1;

        if(currentPath.x > targetPath.x)
        {
            selectedUnit.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            selectedUnit.GetComponent<SpriteRenderer>().flipX = false;
        }

    }
    private void UpdateCurrentTile()
    {
        GridObject unitPos = selectedUnit.GetComponent<GridObject>();
        unitPos.baseTile = tilePath[pathIndex + 1];
        unitPos.UpdateValues();
        unitPos.SetLayerOrder();
        unitPos.SetZ();
        updatedLayer = true;
    }

    private void StopMovement()
    {
        EventHandler.unitMovement.Invoke(new UnitMovementData(selectedUnit, initialTile, destinationTile));
        pathIndex = 0;
        vectorPath = null;
        destinationTile = null;
        selectedUnit = null;
        GameState.Instance.TryStopMinorState(GameState.minorStates.Animating);
    }

  
}
