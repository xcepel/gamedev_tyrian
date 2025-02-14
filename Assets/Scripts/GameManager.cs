using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private String[] listOfScenesNames;
    
    public GameState currentState = GameState.GAME_MANAGER;

    private static int level;
    private Player player;
    private LevelManager levelManager;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Debug.Log("Started " + SceneManager.GetActiveScene().name);
        DontDestroyOnLoad(this);
        ChangeScene("UI");
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerDeath -= PlayerDied;
        }
        if (levelManager != null)
        {
            levelManager.OnLevelFinished -= LevelCompleted;
            levelManager.OnGameFinished -= GameCompleted;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "UI":
                currentState = GameState.MAIN_MENU;
                break;
            case "Level_1":
            case "Level_2":
            case "Level_3":
                currentState = GameState.PLAYING_LEVEL;
                LoadPlayer();
                LoadLevelManager();
                break;
        }
    }

    public void LoadLevelManager()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager)
        {
            levelManager.OnLevelFinished += LevelCompleted;
            levelManager.OnGameFinished += GameCompleted;
            level = levelManager.GetLevel();
        }
    }

    private void PlayerDied()
    {
        Debug.Log("Player has died.");
        
        ChangeScene("UI");
        SceneManager.sceneLoaded += OnYouDiedSceneLoaded;
        UIManager.Show<YouDiedView>();
    }

    private void OnYouDiedSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UI")
        {
            UIManager.Show<YouDiedView>();
            SceneManager.sceneLoaded -= OnYouDiedSceneLoaded;
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("Level completed!");
    
        UIManager.Show<LevelCompleteView>(); 
        ChangeScene("UI");
    }

    private void GameCompleted()
    {
        Debug.Log("Game completed!");
    
        UIManager.Show<YouWinView>(); 
        ChangeScene("UI");
    }
    
    private void LoadPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player != null)
        {
            player.OnPlayerDeath += PlayerDied;
            Debug.Log("Playing level, player found");
        }
        else
        {
            Debug.Log("Player not found in this scene.");
        }
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public static int GetCurrentLevel()
    {
        return level;
    }
}
