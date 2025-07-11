using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Public variables
    public List<GameObject> asteroidPrefabs; // List of asteroid prefabs to spawn
    public float spawnInterval = 10f; // Time interval between spawns
    public float spawnRadius = 20f; // Radius around the player to spawn asteroids
    public float spawnDistanceFromCenter = 10f;
    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroid), 0f, spawnInterval);
    }

    void SpawnAsteroid()
    {
        Vector2 screenCenter = Vector2.zero;
        Vector2 spawnDir = Random.insideUnitCircle.normalized; // random direction
        Vector2 spawnPos = screenCenter + spawnDir * spawnDistanceFromCenter;

        // Choose a random asteroid prefab from the list
        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        // Make the asteroid move toward the screen center (or through it)
        Vector2 targetDir = (screenCenter - spawnPos).normalized;
        asteroid.GetComponent<AsteroidController>().Initialize(targetDir);
    }
}
