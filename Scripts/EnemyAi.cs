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

    // Fall detection
    public float fallThreshold = -5f;

    // Minimum distance from player when spawning
    public float minSpawnDistanceFromPlayer = 8f;

    private Rigidbody rb;
    private bool hasWon = false;
    private bool isInitialized = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing on Enemy!");
            return;
        }

        // Set up physics for smooth movement
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
    }

    void Start()
    {
        // If player reference is not set, try to find it
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("Player not found! Enemy cannot function.");
            }
        }

        isInitialized = true;
        Debug.Log("Enemy AI initialized at position: " + transform.position);
    }

    void FixedUpdate()
    {
        if (!isInitialized || rb == null)
            return;

        // Check if enemy fell off the map
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Enemy fell off map at position: " + transform.position);
            Destroy(gameObject); // Destroy instead of respawn since GameManager spawns them
            return;
        }

        // Don't move if game is over or player is missing
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

    Vector3 DetectAndAvoidObstacles()
    {
        Vector3 avoidance = Vector3.zero;

        if (player == null)
            return avoidance;

        // Cast rays in multiple directions to detect obstacles
        Vector3[] rayDirections = new Vector3[]
        {
            transform.forward,
            Quaternion.Euler(0, 45, 0) * transform.forward,
            Quaternion.Euler(0, -45, 0) * transform.forward,
            Quaternion.Euler(0, 90, 0) * transform.forward,
            Quaternion.Euler(0, -90, 0) * transform.forward,
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
        if (hasWon)
            return;

        hasWon = true;

        Debug.Log("Enemy caught the player!");

        // Find the player controller and trigger game over
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.GameOver();
        }
        else
        {
            Debug.LogError("PlayerController not found on player object!");
        }
    }
}