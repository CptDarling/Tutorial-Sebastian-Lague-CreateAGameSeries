using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapGenerator : MonoBehaviour
{

    [SerializeField] Map[] maps;
    [SerializeField] int mapIndex;

    [SerializeField] Transform tilePrefab;
    [SerializeField] Transform obstaclePrefab;
    [SerializeField] Transform navmeshFloor;
    [SerializeField] Transform navmeshMaskPrefab;

    [SerializeField] Vector2 maxMapSize;

    [Range(0, 1)]
    [SerializeField] float outlinePercent;

    [SerializeField] float tileSize = 1f;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    Map currentMap;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);

        // Represents a unit tile size
        float tileSizeFactor = (1 - outlinePercent) * tileSize;

        // Generate tile coords.
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
            for (int y = 0; y < currentMap.mapSize.y; y++)
                allTileCoords.Add(new Coord(x, y));
        // Shuffle the tile coords
        shuffledTileCoords = new Queue<Coord>(Utility.Array.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Create map holder object
        string holderName = "Generated Map";
        Transform mapHolder = transform.Find(holderName);
        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }

        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {

                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, tilePrefab.transform.rotation);
                newTile.localScale = Vector3.one * tileSizeFactor;
                newTile.parent = mapHolder;
            }

        // Spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord coord = GetRandomCoord();
            obstacleMap[coord.x, coord.y] = true;
            currentObstacleCount++;
            if (coord != currentMap.mapCentre && MapIsFullyAccessable(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 position = CoordToPosition(coord.x, coord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, position + Vector3.up * obstacleHeight / 2f, obstaclePrefab.transform.rotation);
                newObstacle.localScale = new Vector3(tileSizeFactor, obstacleHeight, tileSizeFactor);
                newObstacle.SetParent(mapHolder);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                float colourPercent = coord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

            }
            else
            {
                obstacleMap[coord.x, coord.y] = false;
                currentObstacleCount--;
            }
        }

        // Creating the navmesh mask
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessable(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapflags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCentre);
        mapflags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

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

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y) - currentObstacleCount;
        return accessibleTileCount == targetAccessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        float tileWidth = tilePrefab.GetComponent<Renderer>().bounds.size.x;
        float tileHeight = tilePrefab.GetComponent<Renderer>().bounds.size.z;
        return new Vector3((-currentMap.mapSize.x + tileWidth) / 2f + (float)x * tileWidth, 0, (-currentMap.mapSize.y + tileHeight) / 2f + (float)y * tileHeight) * tileSize;
        //return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Map
    {

        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCentre
        {
            get { return new Coord(mapSize.x / 2, mapSize.y / 2); }
        }

    }


}
