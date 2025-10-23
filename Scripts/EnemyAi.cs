using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    // Reference to the player
    public GameObject player;

    // Movement speed of the enemy
    public float moveSpeed = 8f;
    public float maxSpeed = 12f;

    // How close the enemy needs to be to catch the player
    public float catchDistance = 1.5f;

    // Obstacle detection settings
    public float detectionDistance = 5f;
    public float avoidanceForce = 15f;
    public LayerMask obstacleLayer;

    // Spawn area settings
    public float spawnAreaWidth = 60f;
    public float spawnAreaDepth = 60f;
    public float spawnHeight = 0.5f;

    // Fall detection
    public float fallThreshold = -5f;

    // Minimum distance from player when spawning
    public float minSpawnDistanceFromPlayer = 8f;

    private Rigidbody rb;
    private bool hasWon = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Set up physics for smooth movement
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;

        // If player reference is not set, try to find it
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Spawn at random position immediately
        SpawnAtRandomPosition();
    }

    void FixedUpdate()
    {
        // Check if enemy fell off the map
        if (transform.position.y < fallThreshold)
        {
            RespawnEnemy();
            return;
        }

        // Don't move if game is over
        if (hasWon || player == null)
            return;

        // Calculate direction to player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;

        // Detect obstacles and calculate avoidance
        Vector3 avoidanceDirection = DetectAndAvoidObstacles();

        // Combine player direction with obstacle avoidance
        Vector3 finalDirection = (directionToPlayer + avoidanceDirection).normalized;

        // Apply force towards the final direction
        rb.AddForce(finalDirection * moveSpeed * 10f);

        // Limit maximum speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }

        // Check if enemy has caught the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= catchDistance)
        {
            CatchPlayer();
        }
    }

    void SpawnAtRandomPosition()
    {
        int maxAttempts = 100;

        while (maxAttempts > 0)
        {
            // Generate random position within the arena
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                spawnHeight,
                Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2)
            );

            // Check if spawn position is far enough from player
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(randomPosition, player.transform.position);
                if (distanceToPlayer >= minSpawnDistanceFromPlayer)
                {
                    // Valid spawn position found
                    transform.position = randomPosition;

                    // Reset velocity
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    return;
                }
            }
            else
            {
                // If no player reference, spawn anyway
                transform.position = randomPosition;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
            }

            maxAttempts--;
        }

        // If we couldn't find a valid position, spawn anyway at last random position
        Debug.LogWarning("Could not find spawn position far from player. Spawning at random position anyway.");
        Vector3 fallbackPosition = new Vector3(
            Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
            spawnHeight,
            Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2)
        );
        transform.position = fallbackPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void RespawnEnemy()
    {
        Debug.Log("Enemy fell off map. Respawning...");
        SpawnAtRandomPosition();
    }

    Vector3 DetectAndAvoidObstacles()
    {
        Vector3 avoidance = Vector3.zero;

        // Cast rays in multiple directions to detect obstacles
        Vector3[] rayDirections = new Vector3[]
        {
            transform.forward,                          // Front
            Quaternion.Euler(0, 45, 0) * transform.forward,   // Front-right
            Quaternion.Euler(0, -45, 0) * transform.forward,  // Front-left
            Quaternion.Euler(0, 90, 0) * transform.forward,   // Right
            Quaternion.Euler(0, -90, 0) * transform.forward,  // Left
        };

        // Direction to player for reference
        Vector3 toPlayer = (player.transform.position - transform.position).normalized;
        toPlayer.y = 0;

        foreach (Vector3 direction in rayDirections)
        {
            Vector3 rayDir = direction;
            rayDir.y = 0;
            rayDir.Normalize();

            RaycastHit hit;

            // Cast ray to detect obstacles
            if (Physics.Raycast(transform.position, rayDir, out hit, detectionDistance, obstacleLayer))
            {
                // Calculate avoidance direction (perpendicular to obstacle)
                Vector3 avoidDir = Vector3.Cross(hit.normal, Vector3.up);

                // Choose the perpendicular direction that's closer to the player
                Vector3 avoidDir1 = avoidDir;
                Vector3 avoidDir2 = -avoidDir;

                float angle1 = Vector3.Angle(avoidDir1, toPlayer);
                float angle2 = Vector3.Angle(avoidDir2, toPlayer);

                Vector3 bestAvoidDir = (angle1 < angle2) ? avoidDir1 : avoidDir2;

                // Stronger avoidance for closer obstacles
                float avoidStrength = 1f - (hit.distance / detectionDistance);
                avoidance += bestAvoidDir * avoidStrength * avoidanceForce;

                // Debug visualization (comment out if not needed)
                Debug.DrawRay(transform.position, rayDir * hit.distance, Color.red);
            }
            else
            {
                // Debug visualization (comment out if not needed)
                Debug.DrawRay(transform.position, rayDir * detectionDistance, Color.green);
            }
        }

        return avoidance.normalized;
    }

    void CatchPlayer()
    {
        hasWon = true;

        // Find the player controller and trigger game over
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.GameOver();
        }
    }
}