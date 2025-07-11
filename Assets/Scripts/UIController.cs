using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Public variables
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI dashStatusText;
    public PlayerController playerController;

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
            playerInfoText.text = $"Lives: {playerController.lives}\n" +
                              $"Ammo: Reloading... {(playerController.reloadTime -= Time.deltaTime).ToString("F2")}s\n" +
                              $"Bombs: {playerController.ammoBombRemaining} / {playerController.ammoBomb} \n" +
                              $"Dash: ";
        }
        else
        {
            playerInfoText.text = $"Lives: {playerController.lives}\n" +
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
    }
}
