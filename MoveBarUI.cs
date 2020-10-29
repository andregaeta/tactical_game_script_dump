using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoveBarUI : MonoBehaviour
{
    public Slider slider;

    public void SetMaxMoves(int value)
    {
        slider.maxValue = value;
    }

    public void SetCurrentMoves(int value)
    {
        slider.value = value;
    }
}
