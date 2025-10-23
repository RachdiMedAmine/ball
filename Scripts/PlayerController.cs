using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // Create public variables for player speed, and for the Text UI game objects
    public float speed = 10f;
    public float maxSpeed = 15f;
    public Text countText;
    public Text winText;
    public Text loseText;
    public GameObject restartButton;

    // Set the target score needed to win
    public int scoreToWin = 12;

    // Fall detection
    public float fallThreshold = -5f; // Y position below which player loses

    // Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
    private Rigidbody rb;
    private int count;
    private bool gameOver = false;

    // At the start of the game..
    void Start()
    {
        // Assign the Rigidbody component to our private rb variable
        rb = GetComponent<Rigidbody>();

        // Reduce drag for more responsive movement
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;

        // Set the count to zero 
        count = 0;

        // Run the SetCountText function to update the UI (see below)
        SetCountText();

        // Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
        winText.text = "";

        // Set the lose text to empty and hide restart button
        if (loseText != null)
            loseText.text = "";

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    // Each physics step..
    void FixedUpdate()
    {
        // Don't allow movement if game is over
        if (gameOver)
            return;

        // Check if player fell off the map
        if (transform.position.y < fallThreshold)
        {
            GameOver();
            return;
        }

        // Set some local float variables equal to the value of our Horizontal and Vertical Inputs
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
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

    // When this game object intersects a collider with 'is trigger' checked, 
    // store a reference to that collider in a variable named 'other'..
    void OnTriggerEnter(Collider other)
    {
        // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // Make the other game object (the pick up) inactive, to make it disappear
            other.gameObject.SetActive(false);

            // Add one to the score variable 'count'
            count = count + 1;

            // Run the 'SetCountText()' function (see below)
            SetCountText();
        }
    }

    // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
    void SetCountText()
    {
        // Update the text field of our 'countText' variable to show current score and target
        countText.text = "Count: " + count.ToString() + " / " + scoreToWin.ToString();

        // Check if our 'count' has reached or exceeded the score needed to win
        if (count >= scoreToWin)
        {
            // Set the text value of our 'winText'
            winText.text = "You Win!";
            gameOver = true;

            // Show restart button
            if (restartButton != null)
                restartButton.SetActive(true);
        }
    }

    // Public function that can be called when the enemy catches the player
    public void GameOver()
    {
        gameOver = true;

        // Stop the player's movement
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Display lose message
        if (loseText != null)
            loseText.text = "You Lose!";

        // Show restart button
        if (restartButton != null)
            restartButton.SetActive(true);
    }

    // Function to restart the game (called by the restart button)
    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}