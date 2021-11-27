using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Transform spawnPoint;
    [SerializeField] LivingEntity entity;
    [SerializeField] Wave[] waves;

    Wave currentWave;
    int currentWaveNumber;

    int entitiesRemainingToSpawn;
    int entitiesRemainingAlive;
    float nextSpawnTime;

    void Start()
    {
        StartCoroutine(NextWave());
    }

    void Update()
    {
        if (entitiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            entitiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            LivingEntity newEntity = Instantiate(entity, spawnPoint);
            newEntity.OnDeath += OnEntityDeath;
            newEntity.OnBirth += OnEntityBirth;
        }
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
