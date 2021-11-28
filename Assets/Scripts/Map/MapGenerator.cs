using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] Transform tilePrefab;
    [SerializeField] Transform obstaclePrefab;
    [SerializeField] Vector2 mapSize;

    [Range(0, 1)]
    [SerializeField] float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    [SerializeField] int seed = 1;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {

        allTileCoords = new List<Coord>();

        Vector3 localScale = Vector3.one * (1 - outlinePercent);

        string holderName = "Generated Map";
        Transform mapHolder = transform.Find(holderName);
        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }

        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));

                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, tilePrefab.transform.rotation);
                newTile.localScale = localScale;
                newTile.parent = mapHolder;
            }

        // Shuffle the tile coords
        shuffledTileCoords = new Queue<Coord>(Utility.Array.ShuffleArray(allTileCoords.ToArray(), seed));

        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord coord = GetRandomCoord();
            Vector3 position = CoordToPosition(coord.x, coord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, position + Vector3.up * 0.5f, obstaclePrefab.transform.rotation);
            newObstacle.SetParent(mapHolder);
        }

    }

    Vector3 CoordToPosition(int x, int y)
    {
        float tileWidth = tilePrefab.GetComponent<Renderer>().bounds.size.x;
        float tileHeight = tilePrefab.GetComponent<Renderer>().bounds.size.z;
        return new Vector3((-mapSize.x + tileWidth) / 2f + (float)x * tileWidth, 0, (-mapSize.y + tileHeight) / 2f + (float)y * tileHeight);
    }

    Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

}
