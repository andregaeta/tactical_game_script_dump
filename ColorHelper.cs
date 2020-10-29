using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHelper : MonoBehaviour
{
    #region Singleton
    private static ColorHelper _instance;

    public static ColorHelper Instance { get { return _instance; } }

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
    #endregion

    [SerializeField] GameObject summoningCircleBlue;
    [SerializeField] GameObject summoningCircleRed;
    [SerializeField] GameObject summoningCircleYellow;

    public GameObject GetSummoningCircle(ElementColor color)
    {
        switch (color) 
        {
            case ElementColor.blue:
                return summoningCircleBlue;
            case ElementColor.red:
                return summoningCircleRed;
            case ElementColor.yellow:
                return summoningCircleYellow;
            default:
                return summoningCircleBlue;
        }
    }

}
