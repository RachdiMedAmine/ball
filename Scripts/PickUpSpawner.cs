using UnityEngine;
using System.Collections;

public class PickUpSpawner : MonoBehaviour
{
    // Reference to the Pick Up prefab
    public GameObject pickUpPrefab;

    // Number of pick ups to spawn
    public int numberOfPickUps = 12;

    // Define the spawn area boundaries (adjusted for scale 7x7 map = ~60 units)
    public float spawnAreaWidth = 60f;
    public float spawnAreaDepth = 60f;
    public float spawnHeight = 0.5f;

    // Minimum distance between pick ups
    public float minDistanceBetweenPickUps = 2f;

    // Time between each spawn
    public float spawnInterval = 5f;

    private int spawned = 0;

    void Start()
    {
        // Start the repeating spawn coroutine
        StartCoroutine(SpawnPickUpsOverTime());
    }

    IEnumerator SpawnPickUpsOverTime()
    {
        while (spawned < numberOfPickUps)
        {
            // Try to spawn one pick up
            SpawnSinglePickUp();

            // Wait for the specified interval before spawning the next one
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSinglePickUp()
    {
        int maxAttempts = 100; // Prevent infinite loops

        while (maxAttempts > 0)
        {
            // Generate random position within the arena
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                spawnHeight,
                Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2)
            );

            // Check if position is valid (not too close to other pick ups)
            if (IsValidSpawnPosition(randomPosition))
            {
                // Instantiate the pick up at the random position
                Instantiate(pickUpPrefab, randomPosition, Quaternion.identity);
                spawned++;
                return; // Successfully spawned, exit the method
            }

            maxAttempts--;
        }

        Debug.LogWarning("Could not find valid spawn position. Try increasing spawn area or reducing minimum distance.");
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // Find all existing pick ups in the scene
        GameObject[] existingPickUps = GameObject.FindGameObjectsWithTag("Pick Up");

        // Check distance from all existing pick ups
        foreach (GameObject pickUp in existingPickUps)
        {
            float distance = Vector3.Distance(position, pickUp.transform.position);
            if (distance < minDistanceBetweenPickUps)
            {
                return false; // Too close to another pick up
            }
        }

        return true; // Position is valid
    }
}