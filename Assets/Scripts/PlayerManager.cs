using UnityEngine;
using System.Collections;
public class PlayerManager : MonoBehaviour
{
    public int lives = 3; // Number of lives the player has
    public GameObject playerPrefab; // Prefab for the player character
    public GameObject deathExplosion; // Explosion effect when the player dies
    public float invincibilityTime = 1f; // Time the player is invincible after getting hit
    private float invincibilityTimer = 0f; // Timer to track invincibility duration
    public bool isInvincible = false; // Flag to check if the player is invincible
    private PlayerController playerController; // Reference to the PlayerController script
    private Coroutine flashingCoroutine;
    // Update is called once per frame
    void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        
        Instantiate(deathExplosion, playerPrefab.transform.position, Quaternion.identity); // Instantiate explosion effect
        Destroy(gameObject); // Destroy the player object
    }

    public void TakeDamage()
    {
        if (isInvincible || invincibilityTimer > 0f) return;

        if (flashingCoroutine != null)
        {
            StopCoroutine(flashingCoroutine);
        }

        Instantiate(deathExplosion, playerPrefab.transform.position, Quaternion.identity);
        lives--;
        invincibilityTimer = invincibilityTime;
        StartCoroutine(FlashInvincibility());

        if (lives <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashInvincibility()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float flashInterval = 0.1f;
        float elapsed = 0f;

        while(elapsed < invincibilityTime)
        {
            spriteRenderer.color = new Color(0f, 0f, 0f, 0);
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval * 2;
        }
        spriteRenderer.color = originalColor;
        isInvincible = false;
        flashingCoroutine = null;
    }
}
