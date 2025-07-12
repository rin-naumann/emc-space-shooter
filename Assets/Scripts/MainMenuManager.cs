using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject menuPanel;
    public AudioClip clickSFX;

    public void Start()
    {
        tutorialPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void StartGame()
    {
        playSFX(clickSFX);
        SceneManager.LoadScene("Gameplay");
    }

    public void ShowTutorial()
    {
        playSFX(clickSFX);
        tutorialPanel.SetActive(true);
        menuPanel.SetActive(false);

    }

    public void HideTutorial()
    {
        playSFX(clickSFX);
        tutorialPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        playSFX(clickSFX);
        Debug.Log("Quitting game...");
        Application.Quit(); // Works in builds only
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

