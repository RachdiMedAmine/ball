using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float speed = 10f;
    public float maxSpeed = 15f;

    // Fall detection
    public float fallThreshold = -5f;

    // Private references
    private Rigidbody rb;
    private bool canMove = true;
    private bool hasFallen = false;

    void Start()
    {
        // Assign the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing on Player!");
            return;
        }

        // Reduce drag for more responsive movement
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;

        Debug.Log("Player initialized at position: " + transform.position);
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        // Check if game is over via GameManager
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Don't allow movement if disabled
        if (!canMove)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Check if player fell off the map (only trigger once)
        if (!hasFallen && transform.position.y < fallThreshold)
        {
            Debug.Log("Player fell off the map at Y position: " + transform.position.y);
            hasFallen = true;
            GameOver();
            return;
        }

        // Get input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Apply force for responsive movement
        rb.AddForce(movement * speed * 10f);

        // Limit maximum speed for control
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if we hit a pick up
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // Deactivate the pick up
            other.gameObject.SetActive(false);

            Debug.Log("Player collected a pickup!");

            // Notify the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectCoin();
            }
            else
            {
                Debug.LogError("GameManager.Instance is null! Cannot collect coin.");
            }
        }
    }

    public void GameOver()
    {
        if (!canMove) // Already game over
            return;

        canMove = false;

        Debug.Log("Player GameOver called");

        // Stop the player's movement
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null! Cannot trigger game over.");
        }
    }
}