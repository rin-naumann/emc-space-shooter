using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI dashStatusText;
    public TextMeshProUGUI scoreText;

    [Header("Player References")]
    public PlayerController playerController;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;

    void Update()
    {
        UpdatePlayerInfo();
        UpdateDashStatus();
        UpdateScore();
    }

    void UpdatePlayerInfo()
    {
        // Display player lives, ammo, bombs, and reload status
        string ammoText;

        if (playerController.isReloading)
        {
            // Format reload time without modifying the variable
            float timeRemaining = Mathf.Max(0f, playerController.reloadTime);
            ammoText = $"Ammo: Reloading... {playerController.reloadTimer.ToString("F2")}s";
        }
        else
        {
            ammoText = $"Ammo: {playerController.ammoGunRemaining} / {playerController.ammoGun}";
        }

        playerInfoText.text =
            $"Lives: {playerManager.lives}\n" +
            $"{ammoText}\n" +
            $"Bombs: {playerController.ammoBombRemaining} / {playerController.ammoBomb}\n" +
            $"Dash: ";
    }

    void UpdateDashStatus()
    {
        // Update dash availability text and color
        if (playerController.dashCooldownTimer > 0f)
        {
            dashStatusText.text = "Inactive";
            dashStatusText.color = Color.red;
        }
        else
        {
            dashStatusText.text = "Active";
            dashStatusText.color = Color.green;
        }
    }

    void UpdateScore()
    {
        scoreText.text = $"Score: {scoreManager.score}";
    }
}
