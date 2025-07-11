using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIController : MonoBehaviour
{
    // Public variables
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI dashStatusText;
    public TextMeshProUGUI scoreText;
    public PlayerController playerController;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Check if player is reloading
        if (playerController.isReloading)
        {
            playerInfoText.text = $"Lives: {playerManager.lives}\n" +
                                  $"Ammo: Reloading... {(playerController.reloadTime -= Time.deltaTime).ToString("F2")}s\n" +
                                  $"Bombs: {playerController.ammoBombRemaining} / {playerController.ammoBomb} \n" +
                                  $"Dash: ";
        }
        else
        {
            playerInfoText.text = $"Lives: {playerManager.lives}\n" +
                                  $"Ammo: {playerController.ammoGunRemaining} / {playerController.ammoGun} \n" +
                                  $"Bombs: {playerController.ammoBombRemaining} / {playerController.ammoBomb} \n" +
                                  $"Dash: ";
        }

        // Update dash status text
        if (playerController.dashCooldownTimer > 0f)
        {
            dashStatusText.text = $"Inactive";
            dashStatusText.color = Color.red;
        }
        else
        {
            dashStatusText.text = $"Active";
            dashStatusText.color = Color.green;
        }
        
        // Keep updating the score text
        scoreText.text = $"Score: {scoreManager.score}";
    }
}
