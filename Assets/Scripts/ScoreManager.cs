using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Data")]
    public int score = 0;

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
