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

    void Start()
    {
        // Assign the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Reduce drag for more responsive movement
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
    }

    void FixedUpdate()
    {
        // Check if game is over
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Don't allow movement if disabled
        if (!canMove)
            return;

        // Check if player fell off the map
        if (transform.position.y < fallThreshold)
        {
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

            // Notify the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectCoin();
            }
        }
    }

    public void GameOver()
    {
        canMove = false;

        // Stop the player's movement
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}