using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public AudioClip clickSFX;
    void Start()
    {
        scoreText.text = $"Game Over! You scored\n" +
                        $"{ScoreManager.Instance.score}";
    }

    public void PlayAgain()
    {
        playSFX(clickSFX);
        SceneManager.LoadScene("Gameplay");
    }

    public void ReturnToMenu()
    {
        playSFX(clickSFX);
        SceneManager.LoadScene("MainMenu");
    }

    public void playSFX(AudioClip clip)
    {
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = 1f;
        aSource.spatialBlend = 0f;
        aSource.Play();
        Destroy(tempGO, clip.length);
    }
}
