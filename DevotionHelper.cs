using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevotionHelper 
{
    public ElementColor GetColorByDevotion(Devotion devotion)
    {
        switch (devotion)
        {
            case Devotion.RedSamurai:
                return ElementColor.red;
            case Devotion.YellowPaladin:
                return ElementColor.yellow;

            case Devotion.None:
                Debug.LogError("No devotion.");
                return ElementColor.blue;
            default:
                Debug.LogError("Devotion not found.");
                return ElementColor.blue;
        }
    }

}

public enum Devotion
{
    RedSamurai,
    YellowPaladin,
    BlueLancer,
    None
}
