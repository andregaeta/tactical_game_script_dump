using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCharDisplay : MonoBehaviour
{
    CharacterInventory inventory;
    [SerializeField] GameObject placeholderSprite;
    [SerializeField] Canvas canvas;

    private void Start()
    {
        inventory = GameState.Instance.activeCharacterInventory;
        for (int i = 0; i < inventory.characters.Count; i++)
        {
            GameObject image = Instantiate(placeholderSprite, Vector3.zero, Quaternion.identity);
            image.transform.SetParent(canvas.transform);
            image.transform.position = new Vector3(60 + 100 * i, 60, 0);
            InventoryCharacterDisplay characterDisplay = image.GetComponent<InventoryCharacterDisplay>();
            characterDisplay.inventory = inventory;
            characterDisplay.id = i;

        }

    }

}
