using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public int lives = 3;
    public GameObject playerPrefab;
    public GameObject hitExplosion;

    [Header("Invincibility Settings")]
    public float invincibilityTime = 1f;
    private float invincibilityTimer = 0f;
    public bool isInvincible = false;
    private Coroutine flashingCoroutine;

    [Header("Audio Settings")]
    public AudioClip hitSFX;
    public AudioClip deathSFX;

    void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    void OnDestroy()
    {
        // Plays hit explosion at the player prefab's position
        Instantiate(hitExplosion, playerPrefab.transform.position, Quaternion.identity);
    }

    public void TakeDamage()
    {
        // Exit if already invincible
        if (isInvincible || invincibilityTimer > 0f) return;

        // Stop any ongoing flashing effect
        if (flashingCoroutine != null)
            StopCoroutine(flashingCoroutine);

        GameManager.Instance.playSFX(hitSFX); // Global hit SFX
        Instantiate(hitExplosion, playerPrefab.transform.position, Quaternion.identity);
        lives--;
        invincibilityTimer = invincibilityTime;

        // Start flashing to indicate temporary invincibility
        flashingCoroutine = StartCoroutine(FlashInvincibility());

        if (lives <= 0)
        {
            GameManager.Instance.playSFX(deathSFX); // Global death SFX
            GameManager.Instance.TriggerGameOver(); // Call game over
            Destroy(gameObject);
        }
    }

    IEnumerator FlashInvincibility()
    {
        // Flashes player sprite to indicate invincibility
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float flashInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            spriteRenderer.color = new Color(0f, 0f, 0f, 0); // Invisible
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor; // Visible
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval * 2;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
        flashingCoroutine = null;
    }
}

