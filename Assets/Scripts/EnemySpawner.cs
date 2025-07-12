using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public int maxEnemies = 20;
    public float spawnInterval = 2f;
    public int waveSize = 10;
    public float waveCooldown = 3f;
    private bool waveActive = false;
    private float waveChance;
    private float waveRamp;

    void Start()
    {
        StartCoroutine(SpawningAlgorithm());
    }

    void LateUpdate()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    IEnumerator SpawningAlgorithm()
    {
        while (true)
        {
            if (!waveActive && spawnedEnemies.Count < maxEnemies)
            {
                waveChance = Mathf.Clamp01(waveRamp / 50f); // scales from 0 to 1

                if (Random.value < waveChance)
                {
                    // Spawn a wave
                    StartCoroutine(SpawnWave());
                }
                else
                {
                    // Spawn a single enemy
                    SpawnEnemy();
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnWave()
    {
        waveActive = true;

        for (int i = 0; i < waveSize; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.3f);
        }

        // Wait until all enemies from the wave are destroyed
        yield return new WaitUntil(() => spawnedEnemies.Count == 0);

        waveActive = false;
        waveRamp = 0f;

        yield return new WaitForSeconds(waveCooldown);
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        Vector3 spawnPosition = new Vector3(10, Random.Range(-4f, 4f), 0);
        GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);

        // Hook into enemy death — requires the enemy to call OnDeath
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnDeath += () =>
            {
                spawnedEnemies.Remove(enemy);
                waveRamp += 1f;
            };
        }

        spawnedEnemies.Add(enemy);
    }
}
