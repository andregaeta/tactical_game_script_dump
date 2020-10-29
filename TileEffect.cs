using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffect : ScriptableObject
{
    [SerializeField] public GameObject visualEffect;
}
[System.Serializable]
public class TileEffectSlot
{
    public TileEffect effect;
    public SourceData sourceData;
    public GameObject visualEffect;
    public UnitType unitTypeAffected;
    public TileEffectSlot(TileEffect ieffect, SourceData idata, UnitType iunitTypeAffected)
    {
        effect = ieffect;
        sourceData = idata;
        unitTypeAffected = iunitTypeAffected;
    }
}
