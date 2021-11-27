using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] Transform tilePrefab;
    [SerializeField] Vector2 mapSize;

    [Range(0, 1)]
    [SerializeField] float outlinePercent;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Generated Map";
        Transform mapHolder = transform.Find(holderName);
        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }

        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        float tileWidth = tilePrefab.GetComponent<Renderer>().bounds.size.x;
        float tileHeight = tilePrefab.GetComponent<Renderer>().bounds.size.z;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = new Vector3((-mapSize.x + tileWidth) / 2f + (float)x * tileWidth, 0, (-mapSize.y + tileHeight) / 2f + (float)y * tileHeight);
                Transform newTile = Instantiate(tilePrefab, tilePosition, tilePrefab.transform.rotation);
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }

}
