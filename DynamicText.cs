using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicText : MonoBehaviour
{
    private static DynamicText _instance;

    public static DynamicText Instance { get { return _instance; } }

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


    [SerializeField] GameObject textPopupPrefab;

    public void DisplayText(string text, Color color, Vector3 targetPos)
    {
        float randX = Random.Range(-0.5f, 0.5f);
        float randY = Random.Range(-0.5f, 0.5f);
        targetPos.x += randX;
        targetPos.y += randY;
        GameObject popup = Instantiate(textPopupPrefab, targetPos, Quaternion.identity);
        popup.transform.position = targetPos;
        Text popupText = popup.GetComponentInChildren<Text>();
        popupText.color = color;
        popupText.text = text;

    }
}
