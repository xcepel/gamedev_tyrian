using System;
using UnityEngine;

public class Currencies : MonoBehaviour
{
    public static Currencies Instance { get; private set; }
    
    private static int score = 0;
    private static int credits = 0;
    
    private float crashCoefScore = 1;
    private float hitCoefScore = 5;
    private float killCoefScore = 20;

    private float crashCoefCredit = 1;
    private float hitCoefCredit = 1;
    private float killCoefCredit = 2;
    
    public static event Action<int> OnScoreChanged;
    
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
    
    private void ChangeScore(int amount)
    {
        score += amount;
        print("Score: " + score);
        OnScoreChanged?.Invoke(score); // Notify listeners of the score change
    }

    public static int GetScore()
    {
        return score;
    }

    public static void ChangeCredits(int amount)
    {
        credits += amount;
        print("Credits: " + credits);
    }
    
    public static int GetCredits()
    {
        return credits;
    }
    
    public void OnCollisionCurrenciesEnemy(float healthTaken, bool crash, bool kill)
    {
        float scoreChange = 0;
        float creditChange = 0;
    
        if (crash) // player collides with enemy
        {
            scoreChange = healthTaken * crashCoefScore;
            creditChange = healthTaken * crashCoefCredit;
        }
        else if (kill) // player kills enemy
        {
            scoreChange = killCoefScore;
            creditChange = killCoefCredit;
        }
        else // hit with HP > 0
        {
            scoreChange = healthTaken * hitCoefScore;
            creditChange = healthTaken * hitCoefCredit;
        }

        ChangeScore((int)scoreChange);
        ChangeCredits((int)creditChange);
    }
    
    public void OnCollisionCurrenciesMeteor()
    {
        ChangeScore(5);
        ChangeCredits(5);
    }
    
    public void OnCollisionCurrenciesBoss(bool kill, float healthTaken = 0)
    {
        float scoreChange = 0;
        float creditChange = 0;
    
        if (kill) // player kills boss
        {
            scoreChange = killCoefScore * 100;
            creditChange = killCoefCredit * 10;
        }
        else // hit with HP > 0
        {
            scoreChange = healthTaken * hitCoefScore * 10;
            creditChange = healthTaken * hitCoefCredit;
        }

        ChangeScore((int)scoreChange);
        ChangeCredits((int)creditChange);
    }
    
    public void ResetCurrencies()
    {
        score = 0;
        credits = 0;
    }
}
