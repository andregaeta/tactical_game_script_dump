using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : GridPosition
{

    [SerializeField] public GameObject baseTile;
    [SerializeField] public bool occupiesTile;
    [SerializeField] public float standingOffset;

    GridTile baseGridTile;

    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool snapping;
    public void UpdateValues()
    {
        baseGridTile = baseTile.GetComponent<GridTile>();
        this.i = baseGridTile.i;
        this.j = baseGridTile.j;
        this.yOffset = baseGridTile.yOffset + baseGridTile.height + standingOffset;
    }


    private void Start()
    {
        EventHandler.unitMovement.AddListener(UpdateOnMovement);  
    }
    public void SetLayerOrder()
    {
        sortingOrder = (int)((baseTile.transform.position.y) * -10 + baseLayerOrder);
        spriteRenderer.sortingOrder = sortingOrder;
    }
    public void SetLayerOrderByTile(GameObject tile)
    {
        sortingOrder = (int)((tile.transform.position.y) * -10 + baseLayerOrder);
        spriteRenderer.sortingOrder = sortingOrder;
    }
    public void SetLayerOrderOnHover()
    {
        sortingOrder = (int)((this.gameObject.transform.position.y - 6) * -10 + baseLayerOrder);
        spriteRenderer.sortingOrder = sortingOrder;
    }

    public void SetLayerOrderMax()
    {
        spriteRenderer.sortingOrder = 500;
    }

    private void UpdateOnMovement(UnitMovementData data)
    {
        if (this.gameObject != data.unit)
            return;

        data.initialTile.GetComponent<GridTile>().occupiedBy = null;
        SetupToTile(data.finalTile);
    }
    private void SnapToTile()
    {
        snapping = true;
        targetPosition = GridManager.Instance.CalculatePositionAtTile(this.gameObject, baseTile);
        offset = targetPosition - this.gameObject.transform.position;
    }
    public void SetupToTile(GameObject tile)
    {
        tile.GetComponent<GridTile>().occupiedBy = this.gameObject;
        baseTile = tile;
        UpdateValues();
        SnapToTile();
    }


    private void Update()
    {
        if (snapping)
        {
            if (offset.magnitude == 0)
            {
                this.gameObject.transform.position = targetPosition;
                snapping = false;
                SetLayerOrder();
                SetZ();
            }
            else if (offset.magnitude > 0.1f)
            {
                this.gameObject.transform.position = targetPosition - offset;
                offset *= (1 - Time.deltaTime * 10);
            }
            else if (offset.magnitude < 0.1f)
            {
                offset = Vector3.zero;
            }
        }
    }

}
