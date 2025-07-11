using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public List<GameObject> enemyPrefabs = new List<GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public int maxEnemies = 10;
    public int currentEnemyCount = 0;
    public float spawnInterval = 2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawningAlgorithm());
    }

    IEnumerator SpawningAlgorithm()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        Vector3 spawnPosition = new Vector3(10, Random.Range(-4f, 4f), 0);
        GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        currentEnemyCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
