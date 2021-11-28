using System;
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
    [Range(0, 1)]
    [SerializeField] float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    [SerializeField] int seed = 1;
    Coord mapCentre;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {

        // Generate tile coords.
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                allTileCoords.Add(new Coord(x, y));
        // Shuffle the tile coords
        shuffledTileCoords = new Queue<Coord>(Utility.Array.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord(mapSize.x / 2, mapSize.y / 2);

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

                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, tilePrefab.transform.rotation);
                newTile.localScale = localScale;
                newTile.parent = mapHolder;
            }

        // Cells marked as true have been checked.
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord coord = GetRandomCoord();
            obstacleMap[coord.x, coord.y] = true;
            currentObstacleCount++;
            if (coord != mapCentre && MapIsFullyAccessable(obstacleMap, currentObstacleCount))
            {
                Vector3 position = CoordToPosition(coord.x, coord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, position + Vector3.up * 0.5f, obstaclePrefab.transform.rotation);
                newObstacle.SetParent(mapHolder);
            }
            else
            {
                obstacleMap[coord.x, coord.y] = false;
                currentObstacleCount--;
            }
        }

    }

    bool MapIsFullyAccessable(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapflags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCentre);
        mapflags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int ix = -1; ix <= 1; ix++)
                for (int iy = -1; iy <= 1; iy++)
                {
                    int x = tile.x + ix;
                    int y = tile.y + iy;
                    if (ix == 0 || iy == 0)
                    {
                        if (x >= 0 && x < obstacleMap.GetLength(0) && y >= 0 && y < obstacleMap.GetLength(1))
                        {
                            if (!mapflags[x, y] && !obstacleMap[x, y])
                            {
                                mapflags[x, y] = true;
                                queue.Enqueue(new Coord(x, y));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y) - currentObstacleCount;
        return accessibleTileCount == targetAccessibleTileCount;
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
        public Coord(float _x, float _y)
        {
            x = (int)_x;
            y = (int)_y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(Coord a, Coord b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }
    }

}
