using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCharacterDisplay : MonoBehaviour
{
    [SerializeField] GameObject temporaryPrefab;
    public int id;
    public CharacterInventory inventory;
    UnitDeployer deployer;

    TestCharDisplay testCharDisplay;
    private void Start()
    {
        GameObject gm = GameObject.Find("GM");
        deployer = gm.GetComponent<UnitDeployer>();
    }

    public void OnClickButton()
    {
        deployer.SelectDeployUnitByID(temporaryPrefab, id); //make function to return prefab based on vessel/devotion
    }

}
