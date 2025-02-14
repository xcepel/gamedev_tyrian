using DefaultNamespace;
using UnityEngine;

public class CheatsManager : MonoBehaviour
{
    public static CheatsManager Instance { get; private set; }
    
    private bool isImmortal = false;
    private GameManager gameManager;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        ImmortalityCheat();
    }

    private void ImmortalityCheat()
    {
        if (gameManager && gameManager.GetCurrentState() == GameState.PLAYING_LEVEL)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                isImmortal = !isImmortal;
                Debug.Log("Immortality " + (isImmortal ? "activated" : "deactivated"));
                // TODO maybe if on some visual change?
            }
        }
    }

    public bool IsImmortal()
    {
        return isImmortal;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}