using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // UI References - Just drag the GameObjects here!
    [Header("Drag the GameObject (not the component) for each UI element")]
    public GameObject levelTextObject;
    public GameObject objectiveTextObject;
    public GameObject timerTextObject;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject restartButton;

    // Enemy prefab and spawn settings
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float spawnAreaWidth = 60f;
    public float spawnAreaDepth = 60f;
    public float spawnHeight = 0.5f;
    public float minSpawnDistanceFromPlayer = 10f;

    // Internal text component references (auto-detected)
    private Text levelText;
    private Text objectiveText;
    private Text timerText;
    private Text winText;
    private Text loseText;

    private TextMeshProUGUI levelTextTMP;
    private TextMeshProUGUI objectiveTextTMP;
    private TextMeshProUGUI timerTextTMP;
    private TextMeshProUGUI winTextTMP;
    private TextMeshProUGUI loseTextTMP;

    // Player reference
    private GameObject player;

    // Level tracking
    private int currentLevel = 1;
    private int coinsCollected = 0;
    private bool isGameOver = false;

    // Level 3 timer
    private float survivalTimer = 60f;
    private bool isSurvivalMode = false;

    // Spawned enemies
    private GameObject enemy1;
    private GameObject enemy2;

    // Level requirements
    private int level1CoinsRequired = 4;
    private int level2CoinsRequired = 10;
    private float level3SurvivalTime = 60f;

    void Awake()
    {
        // Singleton pattern - destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Find player
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        // Auto-detect text components from GameObjects
        DetectTextComponents();

        // Validate references
        ValidateReferences();

        // Initialize game state
        InitializeGame();
    }

    void DetectTextComponents()
    {
        // Try to get Text components (legacy)
        if (levelTextObject != null)
        {
            levelText = levelTextObject.GetComponent<Text>();
            levelTextTMP = levelTextObject.GetComponent<TextMeshProUGUI>();
        }

        if (objectiveTextObject != null)
        {
            objectiveText = objectiveTextObject.GetComponent<Text>();
            objectiveTextTMP = objectiveTextObject.GetComponent<TextMeshProUGUI>();
        }

        if (timerTextObject != null)
        {
            timerText = timerTextObject.GetComponent<Text>();
            timerTextTMP = timerTextObject.GetComponent<TextMeshProUGUI>();
        }

        if (winTextObject != null)
        {
            winText = winTextObject.GetComponent<Text>();
            winTextTMP = winTextObject.GetComponent<TextMeshProUGUI>();
        }

        if (loseTextObject != null)
        {
            loseText = loseTextObject.GetComponent<Text>();
            loseTextTMP = loseTextObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void ValidateReferences()
    {
        if (levelTextObject == null) Debug.LogError("Level Text Object not assigned!");
        if (objectiveTextObject == null) Debug.LogError("Objective Text Object not assigned!");
        if (timerTextObject == null) Debug.LogError("Timer Text Object not assigned!");
        if (winTextObject == null) Debug.LogError("Win Text Object not assigned!");
        if (loseTextObject == null) Debug.LogError("Lose Text Object not assigned!");
        if (restartButton == null) Debug.LogError("Restart Button not assigned!");
        if (enemyPrefab == null) Debug.LogError("Enemy Prefab not assigned!");
    }

    void InitializeGame()
    {
        // Reset all game state
        currentLevel = 1;
        coinsCollected = 0;
        isGameOver = false;
        isSurvivalMode = false;
        survivalTimer = level3SurvivalTime;

        // Clear enemy references
        enemy1 = null;
        enemy2 = null;

        // Initialize UI
        SetText(levelTextObject, levelText, levelTextTMP, "Level: 1");
        SetText(objectiveTextObject, objectiveText, objectiveTextTMP, "Collect Coins: 0 / " + level1CoinsRequired);
        SetText(timerTextObject, timerText, timerTextTMP, "");
        SetText(winTextObject, winText, winTextTMP, "");
        SetText(loseTextObject, loseText, loseTextTMP, "");

        if (restartButton != null) restartButton.SetActive(false);

        Debug.Log("Game Initialized - Level 1 Started");
    }

    // Helper method to set text (works with both Text and TextMeshPro)
    void SetText(GameObject textObj, Text legacyText, TextMeshProUGUI tmpText, string value)
    {
        if (legacyText != null)
        {
            legacyText.text = value;
        }
        else if (tmpText != null)
        {
            tmpText.text = value;
        }
        else if (textObj != null)
        {
            Debug.LogWarning("No Text or TextMeshPro component found on " + textObj.name);
        }
    }

    void Update()
    {
        // Handle Level 3 survival timer
        if (isSurvivalMode && !isGameOver)
        {
            survivalTimer -= Time.deltaTime;

            SetText(timerTextObject, timerText, timerTextTMP, "Survive: " + Mathf.CeilToInt(survivalTimer).ToString() + "s");

            if (survivalTimer <= 0)
            {
                WinGame();
            }
        }
    }

    // Called by player when collecting a coin
    public void CollectCoin()
    {
        if (isGameOver)
            return;

        coinsCollected++;
        Debug.Log("Coin collected! Total: " + coinsCollected);

        UpdateLevelUI();

        // Check level progression
        if (currentLevel == 1 && coinsCollected >= level1CoinsRequired)
        {
            StartLevel2();
        }
        else if (currentLevel == 2 && coinsCollected >= level2CoinsRequired)
        {
            StartLevel3();
        }
    }

    void StartLevel2()
    {
        currentLevel = 2;
        Debug.Log("=== LEVEL 2 STARTING ===");

        UpdateLevelUI();

        // Spawn first enemy
        StartCoroutine(SpawnEnemyWithDelay(1f, true));

        // Show transition message
        StartCoroutine(ShowLevelTransition("Level 2: Enemy Incoming!"));
    }

    void StartLevel3()
    {
        currentLevel = 3;
        isSurvivalMode = true;
        survivalTimer = level3SurvivalTime;

        Debug.Log("=== LEVEL 3 STARTING ===");

        UpdateLevelUI();

        // Spawn second enemy
        StartCoroutine(SpawnEnemyWithDelay(1f, false));

        // Show transition message
        StartCoroutine(ShowLevelTransition("Level 3: Survive 60 Seconds!"));
    }

    IEnumerator SpawnEnemyWithDelay(float delay, bool isFirstEnemy)
    {
        yield return new WaitForSeconds(delay);

        GameObject enemy = SpawnEnemy();

        if (isFirstEnemy)
        {
            enemy1 = enemy;
            Debug.Log("First enemy spawned!");
        }
        else
        {
            enemy2 = enemy;
            Debug.Log("Second enemy spawned!");
        }
    }

    GameObject SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned to GameManager!");
            return null;
        }

        if (player == null)
        {
            Debug.LogError("Player reference is null! Cannot spawn enemy.");
            return null;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Enemy spawned at position: " + spawnPosition);

        // Set player reference on enemy
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.player = player;
            Debug.Log("Enemy AI configured with player reference");
        }
        else
        {
            Debug.LogError("EnemyAI component not found on spawned enemy!");
        }

        return enemy;
    }

    Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 100;

        while (maxAttempts > 0)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                spawnHeight,
                Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2)
            );

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(randomPosition, player.transform.position);
                if (distanceToPlayer >= minSpawnDistanceFromPlayer)
                {
                    return randomPosition;
                }
            }

            maxAttempts--;
        }

        // Fallback position
        Debug.LogWarning("Could not find ideal spawn position. Using fallback.");
        return new Vector3(spawnAreaWidth / 3, spawnHeight, spawnAreaDepth / 3);
    }

    void UpdateLevelUI()
    {
        SetText(levelTextObject, levelText, levelTextTMP, "Level: " + currentLevel);

        if (currentLevel == 1)
        {
            SetText(objectiveTextObject, objectiveText, objectiveTextTMP, "Collect Coins: " + coinsCollected + " / " + level1CoinsRequired);
        }
        else if (currentLevel == 2)
        {
            SetText(objectiveTextObject, objectiveText, objectiveTextTMP, "Collect Coins: " + coinsCollected + " / " + level2CoinsRequired + " (Enemy Active!)");
        }
        else if (currentLevel == 3)
        {
            SetText(objectiveTextObject, objectiveText, objectiveTextTMP, "SURVIVE! (Coins: " + coinsCollected + ")");
        }
    }

    IEnumerator ShowLevelTransition(string message)
    {
        if (objectiveTextObject == null)
            yield break;

        // Show transition message
        SetText(objectiveTextObject, objectiveText, objectiveTextTMP, message);

        // Change color to red if possible
        if (objectiveText != null) objectiveText.color = Color.red;
        if (objectiveTextTMP != null) objectiveTextTMP.color = Color.red;

        yield return new WaitForSeconds(3f);

        // Restore color
        if (objectiveText != null) objectiveText.color = Color.white;
        if (objectiveTextTMP != null) objectiveTextTMP.color = Color.white;

        UpdateLevelUI();
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        Debug.Log("=== GAME OVER ===");

        SetText(loseTextObject, loseText, loseTextTMP, "You Lose!");

        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        // Stop all enemies
        StopAllEnemies();
    }

    void WinGame()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isSurvivalMode = false;

        Debug.Log("=== VICTORY ===");

        SetText(winTextObject, winText, winTextTMP, "You Win! All Levels Complete!");

        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        // Stop all enemies
        StopAllEnemies();
    }

    void StopAllEnemies()
    {
        if (enemy1 != null)
        {
            EnemyAI ai1 = enemy1.GetComponent<EnemyAI>();
            if (ai1 != null)
            {
                ai1.enabled = false;
                Rigidbody rb1 = enemy1.GetComponent<Rigidbody>();
                if (rb1 != null)
                {
                    rb1.velocity = Vector3.zero;
                    rb1.angularVelocity = Vector3.zero;
                }
            }
        }

        if (enemy2 != null)
        {
            EnemyAI ai2 = enemy2.GetComponent<EnemyAI>();
            if (ai2 != null)
            {
                ai2.enabled = false;
                Rigidbody rb2 = enemy2.GetComponent<Rigidbody>();
                if (rb2 != null)
                {
                    rb2.velocity = Vector3.zero;
                    rb2.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // Destroy singleton instance before reloading
        Instance = null;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    void OnDestroy()
    {
        // Clear singleton reference when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
}