using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMovement : MonoBehaviour
{

    [SerializeField] GameObject startingTile;
    [SerializeField] GameObject currentTile;
    [SerializeField] bool isBeingDragged;
    [SerializeField] bool attempting;
    [SerializeField] float attemptTime;
    Vector3 mousePos;
    GridObject gridObject;
    float noHitCounter;
    Vector3 offset;
    Vector3 initialPos;
    private void Start()
    {
        gridObject = this.gameObject.GetComponent<GridObject>();
    }
    private void OnMouseDown()
    {
        GameState.Instance.TrySelectDisplayCharacter(this.gameObject);
        attempting = true;
        initialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //StartDragging();
    }

    private void StartDragging()
    {
        CharacterBattle charBattle = this.gameObject.GetComponent<CharacterBattle>();
        if(charBattle.movementCounter <= 0)
        {
            Debug.Log("This character has already moved this turn.");
            return;
        }

        if (!GameState.Instance.TryStartMinorState(GameState.minorStates.DraggingUnit))
            return;
        UnitMovementHandler.Instance.DisplayMoveTiles(this.gameObject);
        GetStartingTile();
        currentTile = startingTile;
        ChangeSelectedTile(startingTile);
        isBeingDragged = true;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        offset = this.gameObject.transform.position - mousePos;
    }

    private void OnMouseUp()
    {
        if (attempting)
        {
            //EventHandler.unitSelection.Invoke(new UnitSelectionData(this.gameObject));
            attemptTime = 0;
            attempting = false;
            return;
        }


        if (!GameState.Instance.TryStopMinorState(GameState.minorStates.DraggingUnit))
            return;


        EventHandler.unitMovement.Invoke(new UnitMovementData(this.gameObject, startingTile.GetComponent<GridObject>().baseTile, currentTile.GetComponent<GridObject>().baseTile));
        isBeingDragged = false;
        startingTile = null;
        UnitMovementHandler.Instance.DestroyMoveTiles();
    }
    void Update()
    {

        if (attempting)
        {
            attemptTime += Time.deltaTime;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            if (attemptTime > 0.5 || mousePos != initialPos)
            {
                attemptTime = 0;
                attempting = false;
                StartDragging();
            }
        }


        if (isBeingDragged)
        {
            if (Input.GetMouseButtonDown(1))
                CancelMovement();


            if (offset.magnitude > 0.1f)
                offset *= (1 - Time.deltaTime * 10);
            else if (offset.magnitude < 0.1f)
            {
                offset = Vector3.zero;
            }

            
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            this.gameObject.transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, 50);

            if (mousePos.x < initialPos.x)
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 0f);
            bool hasHitMoveTile = false;
            bool hasChangedTile = false;
            foreach (RaycastHit2D hit in hits)
            {
                /*
                if (hit.collider.gameObject.CompareTag("Grid Tile"))
                {
                    foreach (GameObject objectOnTop in hit.collider.gameObject.GetComponent<GridTile>().objectsOnTop)
                    {
                        if (objectOnTop.CompareTag("Highlight Tile"))
                        {
                            hasHitMoveTile = true;
                            GameObject newTile = objectOnTop;
                            if (newTile != currentTile && !hasChangedTile)
                            {
                                ChangeSelectedTile(newTile);
                                hasChangedTile = true;
                                noHitCounter = 0;
                            }
                        }
                    }
                    break;
                }
                */

                if (hit.collider.gameObject.CompareTag("Highlight Tile"))
                {
                    hasHitMoveTile = true;
                    GameObject newTile = hit.collider.gameObject;
                    if (newTile != currentTile && !hasChangedTile)
                    {
                        ChangeSelectedTile(newTile);
                        hasChangedTile = true;
                        noHitCounter = 0;
                    }
                    break;
                }
                


            }

            if (!hasHitMoveTile)
            {
                noHitCounter += Time.deltaTime;
                GameObject newTile = startingTile;
                if (newTile != currentTile && noHitCounter > 0.6f)
                {
                    ChangeSelectedTile(newTile);
                    noHitCounter = 0;
                }
            }

            if (hasHitMoveTile)
            {
                gridObject.SetLayerOrderByTile(currentTile.GetComponent<GridObject>().baseTile);
            }
            else
            {
                gridObject.SetLayerOrderMax();
            }
        }
    }
    private void CancelMovement()
    {
        if (!GameState.Instance.TryStopMinorState(GameState.minorStates.DraggingUnit))
            return;

        ChangeSelectedTile(startingTile);
        EventHandler.unitMovement.Invoke(new UnitMovementData(this.gameObject, startingTile.GetComponent<GridObject>().baseTile, currentTile.GetComponent<GridObject>().baseTile));
        isBeingDragged = false;
        startingTile = null;
        UnitMovementHandler.Instance.DestroyMoveTiles();
    }

    private void ChangeSelectedTile(GameObject newTile)
    {
        currentTile.GetComponent<SpriteRenderer>().color = Color.red;
        newTile.GetComponent<SpriteRenderer>().color = Color.green;
        currentTile = newTile;
    }

    private void GetStartingTile()
    {
        foreach (GameObject objectOnTop in this.gameObject.GetComponent<GridObject>().baseTile.GetComponent<GridTile>().objectsOnTop)
        {
            if (objectOnTop.CompareTag("Highlight Tile"))
            {
                startingTile = objectOnTop;
                break;
            }
        }
    }

}
