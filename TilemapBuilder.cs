using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TilemapBuilder : MonoBehaviour
{
    [SerializeField] public Tilemap Tilemap;
    //[SerializeField] public GameObject[] TilePrefabs;
    [SerializeField] public Tileset[] Tilesets;
    [SerializeField] GameObject DefaultPrefab;
    public GameObject SelectedPrefab;
    public Tileset SelectedTileset;
    public bool selected;
    public int height;
    GameObject tilePreview;
    GameObject heightCount;
    void Awake()
    {
        
        if (!Tilemap.initialized)
        {
            Initialize();
        }
        //PrintTiles();
    }
    private void Start()
    {
        tilePreview = GameObject.Find("TilePreview");
        heightCount = GameObject.Find("HeightCount");
        SelectedTileset = Tilesets[0];
        SelectedPrefab = SelectedTileset.tiles[0];
        selected = true;
        height = 0;
        UpdatePreview();
    }
    void Initialize()
    {
        
        Tilemap.tile = new GameObject[Tilemap.height * Tilemap.width];
        for (int i = 0; i < Tilemap.height; i++)
        {
            for (int j = 0; j < Tilemap.width; j++)
            {
                Tilemap.tile[i * Tilemap.width + j] = DefaultPrefab;
            }
        }
        Tilemap.initialized = true;
    }

    void PrintTiles()
    {
        for (int i = 0; i < Tilemap.height; i++)
        {
            for (int j = 0; j < Tilemap.width; j++)
            {
                Debug.Log("[" + i + ", " + j + "] " + Tilemap.tile[i * Tilemap.width + j]);
            }
        }
    }

    public int GetPrefabLenght()
    {
        //return TilePrefabs.Length;
        return Tilesets.Length;
    }

    public void OnSelection(GameObject clickedObject)
    {
        Debug.Log("Selected");
        int index = clickedObject.transform.GetSiblingIndex();
        Debug.Log(index);
        SelectedTileset = Tilesets[index];
        SelectedPrefab = SelectedTileset.tiles[height];
        UpdatePreview();
        selected = true;
    }

    public void Save()
    {
        Debug.Log("saved");
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(Tilemap);
        AssetDatabase.SaveAssets();
    }

    public void UpdatePreview()
    {
        Sprite sprite = SelectedPrefab.GetComponent<SpriteRenderer>().sprite;
        tilePreview.GetComponent<Image>().sprite = sprite;
        tilePreview.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1 + height/4) * 120;
        heightCount.GetComponent<Text>().text = height.ToString();
    }
    private void Update()
    {
        float y = Input.mouseScrollDelta.y;
        if (Mathf.Abs(y) > 0)
        {
            height += (int) y;
            if (height < 0)
                height = 0;
            if (height > SelectedTileset.tiles.Count - 1)
                height = SelectedTileset.tiles.Count - 1;

            SelectedPrefab = SelectedTileset.tiles[height];
            UpdatePreview();
        }
    }
}
