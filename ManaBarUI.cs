using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManaBarUI : MonoBehaviour
{
    public Slider slider;



    public void SetCurrentMana(int value)
    {
        slider.value = value;
    }
}