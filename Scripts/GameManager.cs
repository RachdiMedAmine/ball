using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;

    // UI References
    public Text levelText;
    public Text objectiveText;
    public Text timerText;
    public Text winText;
    public Text loseText;
    public GameObject restartButton;

    // Enemy prefab and spawn settings
    public GameObject enemyPrefab;
    public float spawnAreaWidth = 60f;
    public float spawnAreaDepth = 60f;
    public float spawnHeight = 0.5f;
    public float minSpawnDistanceFromPlayer = 10f;

    // Player reference
    public GameObject player;

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
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Initialize UI
        UpdateLevelUI();
        winText.text = "";
        loseText.text = "";
        timerText.text = "";
        restartButton.SetActive(false);

        // Start Level 1
        StartLevel1();
    }

    void Update()
    {
        // Handle Level 3 survival timer
        if (isSurvivalMode && !isGameOver)
        {
            survivalTimer -= Time.deltaTime;
            timerText.text = "Survive: " + Mathf.Ceil(survivalTimer).ToString() + "s";

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

    void StartLevel1()
    {
        currentLevel = 1;
        coinsCollected = 0;
        UpdateLevelUI();
        Debug.Log("Level 1: Peaceful collection phase started");
    }

    void StartLevel2()
    {
        currentLevel = 2;
        UpdateLevelUI();

        // Spawn first enemy
        enemy1 = SpawnEnemy();

        Debug.Log("Level 2: First enemy spawned!");
        StartCoroutine(ShowLevelTransition("Level 2: Enemy Incoming!"));
    }

    void StartLevel3()
    {
        currentLevel = 3;
        isSurvivalMode = true;
        survivalTimer = level3SurvivalTime;
        UpdateLevelUI();

        // Spawn second enemy
        enemy2 = SpawnEnemy();

        Debug.Log("Level 3: Survival mode activated!");
        StartCoroutine(ShowLevelTransition("Level 3: Survive 60 Seconds!"));
    }

    GameObject SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned to GameManager!");
            return null;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Set player reference on enemy
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null && player != null)
        {
            enemyAI.player = player;
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
            else
            {
                return randomPosition;
            }

            maxAttempts--;
        }

        return new Vector3(0, spawnHeight, 0);
    }

    void UpdateLevelUI()
    {
        levelText.text = "Level: " + currentLevel;

        if (currentLevel == 1)
        {
            objectiveText.text = "Collect Coins: " + coinsCollected + " / " + level1CoinsRequired;
        }
        else if (currentLevel == 2)
        {
            objectiveText.text = "Collect Coins: " + coinsCollected + " / " + level2CoinsRequired + " (Enemy Active!)";
        }
        else if (currentLevel == 3)
        {
            objectiveText.text = "SURVIVE! (Coins: " + coinsCollected + ")";
        }
    }

    IEnumerator ShowLevelTransition(string message)
    {
        // Store original text
        string originalObjective = objectiveText.text;

        // Show transition message
        objectiveText.text = message;
        objectiveText.fontSize = 24;
        objectiveText.color = Color.red;

        yield return new WaitForSeconds(3f);

        // Restore normal text
        objectiveText.fontSize = 18;
        objectiveText.color = Color.white;
        UpdateLevelUI();
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        loseText.text = "You Lose!";
        restartButton.SetActive(true);

        Debug.Log("Game Over!");
    }

    void WinGame()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isSurvivalMode = false;
        winText.text = "You Win! All Levels Complete!";
        restartButton.SetActive(true);

        // Stop all enemies
        if (enemy1 != null)
        {
            EnemyAI ai1 = enemy1.GetComponent<EnemyAI>();
            if (ai1 != null) ai1.enabled = false;
        }
        if (enemy2 != null)
        {
            EnemyAI ai2 = enemy2.GetComponent<EnemyAI>();
            if (ai2 != null) ai2.enabled = false;
        }

        Debug.Log("Victory! Player survived all 3 levels!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}