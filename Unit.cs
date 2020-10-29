using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private void Awake()
    {
        //EventHandler.characterSelection.AddListener(OnSelection);
    }

    void Start()
    {
        BattleManager.Instance.charList.Add(this.gameObject);
    }

    public void OnSelection(UnitSelectionData data)
    {
        //if (this.gameObject != data.selectedUnit) return;

        //GameState.Instance.TrySelectDisplayCharacter(this.gameObject);

    }
}
