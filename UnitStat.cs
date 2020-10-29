using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStat 
{

    [SerializeField] private int baseStat;
    [SerializeField] private int finalStat;
    public List<PercentStatBonus> percentStatBonus;
    public List<FlatStatBonus> flatStatBonus;

    public UnitStat(int value)
    {
        baseStat = value;
    }

    public void AddBaseStat(int value)
    {
        baseStat += value;
        CalculateFinalStat();
    }

    public void AddFlatStatBonus(FlatStatBonus bonus)
    {
        flatStatBonus.Add(bonus);
        CalculateFinalStat();
    }
    public void RemoveFlatStatBonus(FlatStatBonus bonus)
    {
        flatStatBonus.Remove(bonus);
        CalculateFinalStat();
    }

    public void AddPercentStatBonus(PercentStatBonus bonus)
    {
        percentStatBonus.Add(bonus);
        CalculateFinalStat();
    }
    public void RemovePercentStatBonus(PercentStatBonus bonus)
    {
        percentStatBonus.Remove(bonus);
        CalculateFinalStat();
    }

    private void CalculateFinalStat()
    {
        float calculatedStat = baseStat;

        foreach (FlatStatBonus bonus in flatStatBonus)
        {
            calculatedStat += bonus.value;
        }

        foreach (PercentStatBonus bonus in percentStatBonus)
        {
            calculatedStat *= 1 + bonus.value;
        }

        if (calculatedStat < 0)
            calculatedStat = 0;

        finalStat = (int) calculatedStat;
    }

    public int GetFinalStat()
    {
        CalculateFinalStat();
        return finalStat;
    }
    public int GetBaseStat()
    {
        return baseStat;
    }


}
[System.Serializable]
public class UnitStats
{
    [SerializeField] public UnitStat strenght;
    [SerializeField] public UnitStat intelligence;
    [SerializeField] public UnitStat defense;
    [SerializeField] public UnitStat resistance;
    [SerializeField] public UnitStat critical;
    [SerializeField] public UnitStat faith;

    public void AddFlatModifiers(FlatStatusModifiers flatModifiers)
    {
        strenght.AddFlatStatBonus(flatModifiers.flatStrenght);
        intelligence.AddFlatStatBonus(flatModifiers.flatIntelligence);
        defense.AddFlatStatBonus(flatModifiers.flatDefense);
        resistance.AddFlatStatBonus(flatModifiers.flatResistance);
        critical.AddFlatStatBonus(flatModifiers.flatCritical);
        faith.AddFlatStatBonus(flatModifiers.flatFaith);
    }

    public void AddPercentModifiers(PercentStatusModifiers percentModifiers)
    {
        strenght.AddPercentStatBonus(percentModifiers.percentStrenght);
        intelligence.AddPercentStatBonus(percentModifiers.percentIntelligence);
        defense.AddPercentStatBonus(percentModifiers.percentDefense);
        resistance.AddPercentStatBonus(percentModifiers.percentResistance);
        critical.AddPercentStatBonus(percentModifiers.percentCritical);
        faith.AddPercentStatBonus(percentModifiers.percentFaith);
    }

    public void RemoveFlatModifiers(FlatStatusModifiers flatModifiers)
    {
        strenght.RemoveFlatStatBonus(flatModifiers.flatStrenght);
        intelligence.RemoveFlatStatBonus(flatModifiers.flatIntelligence);
        defense.RemoveFlatStatBonus(flatModifiers.flatDefense);
        resistance.RemoveFlatStatBonus(flatModifiers.flatResistance);
        critical.RemoveFlatStatBonus(flatModifiers.flatCritical);
        faith.RemoveFlatStatBonus(flatModifiers.flatFaith);
    }

    public void RemovePercentModifiers(PercentStatusModifiers percentModifiers)
    {
        strenght.RemovePercentStatBonus(percentModifiers.percentStrenght);
        intelligence.RemovePercentStatBonus(percentModifiers.percentIntelligence);
        defense.RemovePercentStatBonus(percentModifiers.percentDefense);
        resistance.RemovePercentStatBonus(percentModifiers.percentResistance);
        critical.RemovePercentStatBonus(percentModifiers.percentCritical);
        faith.RemovePercentStatBonus(percentModifiers.percentFaith);
    }

}

public enum StatType
{
    strenght,
    intelligence,
    defense,
    resistance,
    critical,
    faith
}

[System.Serializable]
public class FlatStatBonus
{
    public int value;

    public FlatStatBonus(int ivalue)
    {
        this.value = ivalue;
    }
}
[System.Serializable]
public class PercentStatBonus
{
    public float value;

    public PercentStatBonus(float ivalue)
    {
        this.value = ivalue;
    }
}

[System.Serializable]
public class FlatStatusModifiers
{
    public FlatStatBonus flatStrenght;
    public FlatStatBonus flatIntelligence;
    public FlatStatBonus flatDefense;
    public FlatStatBonus flatResistance;
    public FlatStatBonus flatCritical;
    public FlatStatBonus flatFaith;
}

[System.Serializable]
public class PercentStatusModifiers
{
    public PercentStatBonus percentStrenght;
    public PercentStatBonus percentIntelligence;
    public PercentStatBonus percentDefense;
    public PercentStatBonus percentResistance;
    public PercentStatBonus percentCritical;
    public PercentStatBonus percentFaith;
}


