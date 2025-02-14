using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        scoreText.text = "Score: " + Currencies.GetScore();
    }
    
    void OnEnable()
    {
        Currencies.OnScoreChanged += UpdateScoreText;
    }

    void OnDisable()
    {
        Currencies.OnScoreChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(int newScore)
    {
        Debug.Log($"Updating score text: {newScore}");
        scoreText.text = "Score: " + newScore.ToString();
    }
}