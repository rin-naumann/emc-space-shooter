using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    public int maxEnemies = 20;
    public float spawnInterval = 2f;

    [Header("Wave Settings")]
    public int waveSize = 10;
    public float waveCooldown = 3f;
    private bool waveActive = false;

    // Internal tracking
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float waveChance;
    private float waveRamp;

    void Start()
    {
        StartCoroutine(SpawningAlgorithm());
    }

    void LateUpdate()
    {
        // Clean up destroyed enemies from the list
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    IEnumerator SpawningAlgorithm()
    {
        while (true)
        {
            if (!waveActive && spawnedEnemies.Count < maxEnemies)
            {
                // Wave chance increases as more enemies are killed (maxes out at 100%)
                waveChance = Mathf.Clamp01(waveRamp / 50f);

                if (Random.value < waveChance)
                {
                    StartCoroutine(SpawnWave());
                }
                else
                {
                    SpawnEnemy();
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnWave()
    {
        waveActive = true;

        // Spawn a group of enemies with short delays between each
        for (int i = 0; i < waveSize; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.1f);
        }

        // Wait until all wave enemies are destroyed
        yield return new WaitUntil(() => spawnedEnemies.Count == 0);

        // Reset after wave ends
        waveActive = false;
        waveRamp = 0f;

        yield return new WaitForSeconds(waveCooldown);
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        // Choose random prefab and spawn off-screen
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        Vector3 spawnPosition = new Vector3(10f, Random.Range(-4f, 4f), 0f);

        GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        // Register cleanup and waveRamp increment when enemy dies
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnDeath += () =>
            {
                spawnedEnemies.Remove(enemy);
                waveRamp += 1f;
            };
        }
    }
}
