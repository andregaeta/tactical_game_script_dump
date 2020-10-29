using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : GridPosition
{

    [SerializeField] public GameObject occupiedBy;
    [SerializeField] public bool isWalkable;
    [SerializeField] public List<GameObject> objectsOnTop;
    [SerializeField] public List<TileEffectSlot> tileEffects;

    public void SetLayerOrder()
    {
        sortingOrder = (int)((this.gameObject.transform.position.y) * -10 + baseLayerOrder);
        spriteRenderer.sortingOrder = sortingOrder;
    }

    public TileEffectSlot FindEffect(TileEffect effect, GameObject source)
    {
        //Debug.Log("Searching for " + effect + " from " + source);
        foreach(TileEffectSlot slot in tileEffects)
        {
            if (slot.effect == effect)
            {
                if (slot.sourceData.sourceGameObject == source)
                {   

                    return slot;
                }
            }
        }
        return null;
    }
    public void AddEffect(TileEffectSlot effectSlot)
    {
        bool alreadyContained = false;
        foreach (TileEffectSlot slot in tileEffects)
        {
            if (slot.effect == effectSlot.effect)
            {
                if (slot.sourceData.sourceGameObject == effectSlot.sourceData.sourceGameObject)
                {
                    alreadyContained = true;
                    break;
                }
            }
        }
        if (alreadyContained) return;

        tileEffects.Add(effectSlot);
        if (effectSlot.effect.visualEffect != null)
        {
            GameObject visualEffect = GridManager.Instance.InstantiateAtTile(effectSlot.effect.visualEffect, this.gameObject, this.gameObject);
            effectSlot.visualEffect = visualEffect;
        }
    }
    public void RemoveEffect(TileEffect effect, GameObject source)
    {
        foreach(TileEffectSlot slot in tileEffects)
        {
            if(slot.effect == effect)
            {
                if(slot.sourceData.sourceGameObject == source)
                {
                    if (slot.visualEffect != null)
                    {
                        Destroy(slot.visualEffect);
                        slot.visualEffect = null;
                    }
                    tileEffects.Remove(slot);
                    
                    return;
                }
            }
        }
    }

    private void Start()
    {
        tileEffects = new List<TileEffectSlot>();
    }
}
