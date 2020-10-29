using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    float destroyTime = 1f;

    private void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }
}
