using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] GameObject gridTileParent;
    [SerializeField] GameObject enemyParent;
    [SerializeField] Tilemap Tilemap;
    [SerializeField] bool EditorScene;
    public GameObject[,] Grid;
    

    private static GridManager _instance;

    public static GridManager Instance { get { return _instance; } }

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

    void Start()
    {
        Grid = new GameObject[Tilemap.height, Tilemap.width];
        
        for (int i = 0; i < Tilemap.height; i++)
        {
            for (int j = 0; j < Tilemap.width; j++)
            {
                Grid[i, j] = CreateTile(i, j);
            }
        }

        if (EditorScene)
            return;
        foreach (EnemyInTile enemyInTile in Tilemap.enemyInTile)
        {
            GameObject enemyObject = InstantiateAtTile(enemyInTile.enemyPrefab, Grid[enemyInTile.i, enemyInTile.j], enemyParent);
            enemyObject.GetComponent<EnemyBattle>().SetupBattleStats(enemyInTile.enemy);
            enemyObject.GetComponent<GridObject>().SetupToTile(Grid[enemyInTile.i, enemyInTile.j]);
        }
    }



    public Vector2 CartToIso(Vector2 cartVector)
    {
        Vector2 isoVector = new Vector2();
        isoVector.x = cartVector.y - cartVector.x;
        isoVector.y = -(cartVector.x + cartVector.y) / 2;
        return isoVector;
    }

    public GameObject CreateTile(int i, int j)
    {
        GameObject newTile = Instantiate(Tilemap.tile[i * Tilemap.width + j], CartToIso(new Vector2(i, j)), Quaternion.identity);
        newTile.transform.parent = gridTileParent.transform;
        newTile.name = "Tile[" + i + "," + j + "]";
        GridTile newTileGridPosition = newTile.GetComponent<GridTile>();
        newTileGridPosition.i = i;
        newTileGridPosition.j = j;
        newTileGridPosition.SetLayerOrder();
        newTileGridPosition.SetZ();


        if (EditorScene)
            newTile.AddComponent<EditorTileClick>();
        return newTile;
    }

    public GameObject InstantiateAtTile(GameObject gameObject, GameObject tile, GameObject parent)
    {
        GridObject objectGridPosition = gameObject.GetComponent<GridObject>();
        GridTile tileGridPosition = tile.GetComponent<GridTile>();

        if (objectGridPosition.occupiesTile && tileGridPosition.occupiedBy != null)
            return null;

        int i = tileGridPosition.i;
        int j = tileGridPosition.j;
        float yOffset = tileGridPosition.height + tileGridPosition.yOffset + objectGridPosition.standingOffset;

        GameObject instantiatedObject = Instantiate(gameObject, CartToIso(new Vector2(i, j)) + new Vector2(0, yOffset), Quaternion.identity);
        GridObject instantiatedObjectGridPosition = instantiatedObject.GetComponent<GridObject>();

        instantiatedObjectGridPosition.i = i;
        instantiatedObjectGridPosition.j = j;
        instantiatedObjectGridPosition.yOffset = yOffset;
        instantiatedObject.transform.parent = parent.transform;
        instantiatedObjectGridPosition.baseTile = tile;
        instantiatedObjectGridPosition.SetLayerOrder();
        instantiatedObjectGridPosition.SetZ();


        return instantiatedObject;
    }



    public Vector3 CalculatePositionAtTile(GameObject unit , GameObject tile)
    {
        GridObject unitPos = unit.GetComponent<GridObject>();
        GridTile tilePos = tile.GetComponent<GridTile>();
        Vector2 tilePositionCart = new Vector2(tilePos.i, tilePos.j);
        Vector2 tilePositionIso = CartToIso(tilePositionCart);
        Vector3 position = new Vector3(tilePositionIso.x, tilePositionIso.y + unitPos.standingOffset + tilePos.height + tilePos.yOffset, 0);
        return position;
    }

}