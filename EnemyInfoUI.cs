using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyInfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject activeUnit;
    [SerializeField] GameObject skillBar;
    [SerializeField] GameObject passives;
    [SerializeField] GameObject portrait;


    [SerializeField] ActiveSkill defaultSkill;
    [SerializeField] ActiveSkill basicAttack;
    SkillSlotUI[] skillSlots;
    PassiveSlotUI[] passiveSlots;

    [SerializeField] Text portraitName;
    [SerializeField] Text portraitLevel;
    [SerializeField] Text portraitStrenght;
    [SerializeField] Text portraitIntelligence;
    [SerializeField] Text portraitDefense;
    [SerializeField] Text portraitResistance;
    [SerializeField] Text portraitCritical;
    [SerializeField] Text portraitFaith;

    [SerializeField] Text portraitMaxHP;
    [SerializeField] Text portraitCurrentHP;

    [SerializeField] GameObject skillTooltip;
    [SerializeField] GameObject statusEffectTooltip;
    [SerializeField] GameObject passiveTooltip;

    [SerializeField] GameObject statusEffectSlotPrefab;
    [SerializeField] GameObject statusEffectPanel;


    private void Awake()
    {
        skillSlots = skillBar.GetComponentsInChildren<SkillSlotUI>();
        passiveSlots = passives.GetComponentsInChildren<PassiveSlotUI>();
        EventHandler.enemySelection.AddListener(DisplayUI);
        EventHandler.enemyDeselection.AddListener(HideUI);
        EventHandler.animationStart.AddListener(HideUI);
        this.gameObject.SetActive(false);
    }

    public void HideUI()
    {
        this.gameObject.SetActive(false);
    }
    public void DisplayUI(UnitSelectionData data)
    {
        activeUnit = data.selectedUnit;
        UpdateUI();
        this.gameObject.SetActive(true);

    }

    public void UpdateUI()
    {
        EnemyBattle unitStats = activeUnit.GetComponent<EnemyBattle>();

        foreach (SkillSlotUI skillSlot in skillSlots)
        {
            skillSlot.skill = null;
        }

        for (int i = 0; i < unitStats.activeSkills.Count; i++)
        {
            skillSlots[i + 1].skill = unitStats.activeSkills[i];
        }

        skillSlots[0].skill = new ActiveSkillSlot(basicAttack);

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i].skill == null)
            {
                skillSlots[i].skill = new ActiveSkillSlot(defaultSkill);
            }
        }


        foreach (PassiveSlotUI passiveSlot in passiveSlots)
        {
            passiveSlot.skill = null;
        }

        for (int i = 0; i < unitStats.passiveSkills.Count; i++)
        {
            passiveSlots[i].skill = unitStats.passiveSkills[i].skill;
        }

        portraitName.text = unitStats.displayName;
        portraitLevel.text = "Lvl " + unitStats.level.ToString();
        portraitStrenght.text = unitStats.stats.strenght.GetFinalStat().ToString();
        portraitIntelligence.text = unitStats.stats.intelligence.GetFinalStat().ToString();
        portraitDefense.text = unitStats.stats.defense.GetFinalStat().ToString();
        portraitResistance.text = unitStats.stats.resistance.GetFinalStat().ToString();
        portraitCritical.text = unitStats.stats.critical.GetFinalStat().ToString();
        portraitFaith.text = unitStats.stats.faith.GetFinalStat().ToString();

        portraitMaxHP.text = unitStats.HP.maxHP.ToString();
        portraitCurrentHP.text = unitStats.HP.currentHP.ToString();

        skillTooltip.SetActive(false);
        statusEffectTooltip.SetActive(false);
        passiveTooltip.SetActive(false);

        foreach (Transform child in statusEffectPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (StatusEffectSlot effect in unitStats.statusEffectSlots)
        {
            GameObject slotObject = Instantiate(statusEffectSlotPrefab, Vector3.zero, Quaternion.identity, statusEffectPanel.transform);
            StatusEffectSlotUI slotScript = slotObject.GetComponent<StatusEffectSlotUI>();
            slotScript.statusEffectSlot = effect;
            slotScript.tooltip = statusEffectTooltip;

            
            if (effect.statusEffect.stackingType == StatusEffectStackingType.stackCount)
            {
                slotScript.valueText.text = effect.data.stacks.ToString();
                slotScript.valueText.color = Color.gray;
            }
            else
            {
                if (effect.data.isPermanent)
                {
                    slotScript.valueText.text = "";
                }
                else
                {
                    slotScript.valueText.text = effect.data.duration.ToString();
                }
            }

        }


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateUI();
        }

        if (activeUnit != null && Input.GetMouseButton(0) && !GameState.Instance.mouseIsOnUI && GameState.Instance.MinorState == GameState.minorStates.None)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 0f);
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Grid Tile"))
                {
                    continue;
                }
                hitList.Add(hit);
            }
            if (hitList.Count > 0) return;

            GameState.Instance.TryDeselectDisplayEnemy();
        }




    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameState.Instance.mouseIsOnUI = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameState.Instance.mouseIsOnUI = false;

    }
}
