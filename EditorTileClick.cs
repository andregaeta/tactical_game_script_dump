using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTileClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject tilemapBuilder = GameObject.Find("TilemapBuilder");
        GameObject gridManager = GameObject.Find("GridManager");
        GameObject selectedPrefab = tilemapBuilder.GetComponent<TilemapBuilder>().SelectedPrefab;
        Tilemap tilemap = tilemapBuilder.GetComponent<TilemapBuilder>().Tilemap;

        if (tilemapBuilder.GetComponent<TilemapBuilder>().selected && selectedPrefab != null)
        {
            //Debug.Log(selectedPrefab);
            //GameObject newTile = Instantiate(selectedPrefab, gridManager.GetComponent<GridManager2>().CartToIso(new Vector2(this.gameObject.GetComponent<Tile>().getTileX(), this.gameObject.GetComponent<Tile>().getTileY())), Quaternion.identity);
            //newTile.AddComponent<EditorTileClick>();
            //Debug.Log(newTile);
            int tileIndex = this.gameObject.GetComponent<GridPosition>().i * tilemap.width + this.gameObject.GetComponent<GridPosition>().j;
            tilemap.tile[tileIndex] = selectedPrefab;

            GridManager.Instance.CreateTile(this.gameObject.GetComponent<GridTile>().i, this.gameObject.GetComponent<GridTile>().j);


            if (this.gameObject.GetComponent<TileHighlight>().isHighlighted)
                Destroy(this.gameObject.GetComponent<TileHighlight>().instancedHighlightTile);
            Destroy(this.gameObject);
        }
    }
}
