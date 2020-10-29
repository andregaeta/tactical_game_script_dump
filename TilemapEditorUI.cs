using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilemapEditorUI : MonoBehaviour
{
    [SerializeField] GameObject itemContainer;
    [SerializeField] GameObject slotPrefab;

    private void Start()
    {
        int lenght = this.gameObject.GetComponent<TilemapBuilder>().GetPrefabLenght();
        Debug.Log(lenght);
        for (int i = 0; i < 12; i++)
        {
            if (i > lenght - 1)
                itemContainer.transform.GetChild(i).GetComponent<Image>().enabled = false;
            else
            {
                //itemContainer.transform.GetChild(i).GetComponent<Image>().sprite = this.gameObject.GetComponent<TilemapBuilder>().TilePrefabs[i].GetComponent<SpriteRenderer>().sprite;
                TilemapBuilder builder = this.gameObject.GetComponent<TilemapBuilder>();
                itemContainer.transform.GetChild(i).GetComponent<Image>().sprite = builder.Tilesets[i].tiles[2].GetComponent<SpriteRenderer>().sprite;
            }

        }
    }
}
