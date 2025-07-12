using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio")]
    public AudioClip gameOverSFX;
    public GameObject BGMPlayer; // Reference to object playing background music

    void Awake()
    {
        // Ensure only one instance exists (singleton pattern)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TriggerGameOver()
    {
        StartCoroutine(DelayedGameOver());
    }

    private IEnumerator DelayedGameOver()
    {
        yield return null;

        if (BGMPlayer != null)
            BGMPlayer.SetActive(false); // Stop background music

        playSFX(gameOverSFX); // Play game over sound

        yield return new WaitForSeconds(5f); // Delay before loading GameOver scene
        SceneManager.LoadScene("GameOver");
    }

    // Plays a one-shot 2D sound effect
    public void playSFX(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = 1f;
        aSource.spatialBlend = 0f; // 2D sound
        aSource.Play();
        Destroy(tempGO, clip.length);
    }
}
