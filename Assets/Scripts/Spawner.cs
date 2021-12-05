using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] LivingEntity entity;
    [SerializeField] Wave[] waves;

    Wave currentWave;
    int currentWaveNumber;

    int entitiesRemainingToSpawn;
    int entitiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    LivingEntity playerEntity;
    Transform playerTransform;
    float timeBetweenCampingChecks = 2f;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerTransform.position;

        map = FindObjectOfType<MapGenerator>();
        StartCoroutine(NextWave());
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.SqrMagnitude(playerTransform.position - campPositionOld) < Mathf.Pow(campThresholdDistance, 1));
                campPositionOld = playerTransform.position;
            }

            if (entitiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                entitiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEntity());
            }
        }
    }

    IEnumerator SpawnEntity()
    {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4;

        Transform spawnTitle = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTitle = map.GetTileFromPosition(playerTransform.position);
        }
        Material tileMaterial = spawnTitle.GetComponent<Renderer>().material;
        Color originalColour = tileMaterial.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {

            tileMaterial.color = Color.Lerp(originalColour, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        LivingEntity newEntity = Instantiate(entity, spawnTitle.position + Vector3.up, Quaternion.identity) as LivingEntity;
        newEntity.OnDeath += OnEntityDeath;
        newEntity.OnBirth += OnEntityBirth;
    }

    private void OnEntityBirth()
    {
        print("An entity has been born, hurray!");
        entitiesRemainingAlive++;
    }

    void OnEntityDeath()
    {
        print("Entity has passed away, how sad :(");
        entitiesRemainingAlive--;
        if (entitiesRemainingAlive == 0)
        {
            StartCoroutine(NextWave());
        }
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
        playerEntity.OnDeath -= OnPlayerDeath;

    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(5);
        currentWaveNumber++;
        print($"Wave: {currentWaveNumber}");
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            entitiesRemainingToSpawn = currentWave.entityCount;
        }
    }

    [System.Serializable]
    class Wave
    {
        public int entityCount;
        public float timeBetweenSpawns;
    }
}
