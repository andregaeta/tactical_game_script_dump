using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUI : MonoBehaviour
{
    public Slider slider;

    public void SetMaxActions(int value)
    {
        slider.maxValue = value;
    }

    public void SetCurrentActions(int value)
    {
        slider.value = value;
    }
}
