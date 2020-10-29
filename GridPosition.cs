using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] public int i;
    [SerializeField] public int j;
    [SerializeField] public float height;
    [SerializeField] public float yOffset;
    [SerializeField] public int baseLayerOrder;
    [SerializeField] public int z;

    public SpriteRenderer spriteRenderer;
    public int sortingOrder;
    private void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetZ()
    {
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, -z -sortingOrder/5);
    }

}
