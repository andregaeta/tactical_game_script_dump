using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ActiveSkillSlot skill;
    [SerializeField] GameObject tooltip;
    public Text cooldownText;

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
        if (skill.skill == null)
            return;
        tooltip.SetActive(true);
        tooltip.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 60, 0);
        SkillTooltipUI tooltipScript = tooltip.GetComponent<SkillTooltipUI>();
        tooltipScript.skillName.text = skill.skill.displayName;
        tooltipScript.skillDescription.text = skill.skill.description;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skill == null)
            return;
        StartCoroutine(skill.Co_OnSkillUse(GameState.Instance.SelectedDisplayCharacter));
    }
}



