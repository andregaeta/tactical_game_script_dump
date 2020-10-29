using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public StatusEffectSlot statusEffectSlot;
    [SerializeField] public GameObject tooltip;
    [SerializeField] public Text valueText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
    public void DisplayTooltip()
    {
        if (statusEffectSlot == null)
            return;
        tooltip.SetActive(true);
        tooltip.transform.position = new Vector3(this.gameObject.transform.position.x + 100, this.gameObject.transform.position.y - 130, 0);
        StatusEffectTooltipUI tooltipScript = tooltip.GetComponent<StatusEffectTooltipUI>();
        tooltipScript.effectName.text = statusEffectSlot.statusEffect.displayName;
        tooltipScript.effectDescription.text = statusEffectSlot.statusEffect.description;
    }
}
