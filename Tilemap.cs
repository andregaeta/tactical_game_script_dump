using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tilemap", menuName = "Tilemap")]
public class Tilemap : ScriptableObject
{
    public int height;
    public int width;
    public bool initialized;
    [SerializeField] public GameObject[] tile;
    [SerializeField] public EnemyInTile[] enemyInTile;
}

[System.Serializable]
public class EnemyInTile
{
    public Enemy enemy;
    public GameObject enemyPrefab;
    public int i;
    public int j;
}
