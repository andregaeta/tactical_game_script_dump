using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PassiveSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PassiveSkill skill;
    [SerializeField] GameObject tooltip;

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
        if (skill == null)
            return;
        tooltip.SetActive(true);
        tooltip.transform.position = new Vector3(this.gameObject.transform.position.x + 100, this.gameObject.transform.position.y - 160, 0);
        PassiveTooltipUI tooltipScript = tooltip.GetComponent<PassiveTooltipUI>();
        tooltipScript.passiveName.text = skill.displayName;
        tooltipScript.passiveDescription.text = skill.description;
    }
}
