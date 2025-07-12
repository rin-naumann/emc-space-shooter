using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Settings")]
    public List<GameObject> asteroidPrefabs; // List of asteroid types to spawn

    [Header("Spawn Timing")]
    public float spawnInterval = 3f; // Time between spawns

    [Header("Spawn Position")]
    public float spawnDistanceFromCenter = 10f; // Distance from center where asteroids appear

    void Start()
    {
        // Repeatedly call SpawnAsteroid at fixed intervals
        InvokeRepeating(nameof(SpawnAsteroid), 0f, spawnInterval);
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefabs.Count == 0) return;

        Vector2 screenCenter = Vector2.zero; // Center of screen (can update if needed)
        Vector2 spawnDir = Random.insideUnitCircle.normalized; // Random direction from center
        Vector2 spawnPos = screenCenter + spawnDir * spawnDistanceFromCenter;

        // Pick a random asteroid from the list
        GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];
        GameObject asteroid = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Send asteroid toward the screen center
        Vector2 moveDirection = (screenCenter - spawnPos).normalized;
        asteroid.GetComponent<AsteroidController>()?.Initialize(moveDirection);
    }
}
