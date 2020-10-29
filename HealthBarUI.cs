using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(int value)
    {
        slider.maxValue = value;
    }

    public void SetCurrentHealth(int value)
    {
        slider.value = value;
    }
}
