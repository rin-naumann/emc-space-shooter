using UnityEngine;
using System.Collections;
public class PlayerManager : MonoBehaviour
{
    // Player lives related variables
    public int lives = 3; 
    public GameObject playerPrefab; 
    public GameObject hitExplosion; 

    // I-frame related variables
    public float invincibilityTime = 1f; 
    private float invincibilityTimer = 0f; 
    public bool isInvincible = false; 
    private Coroutine flashingCoroutine;

    // SFX related variables
    private AudioSource audioSource;
    public AudioClip hitSFX;
    public AudioClip deathSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        
        Instantiate(hitExplosion, playerPrefab.transform.position, Quaternion.identity); // Instantiate explosion effect
        Destroy(gameObject); // Destroy the player object
    }

    public void TakeDamage()
    {
        if (isInvincible || invincibilityTimer > 0f) return;

        if (flashingCoroutine != null)
        {
            StopCoroutine(flashingCoroutine);
        }

        audioSource.PlayOneShot(hitSFX);
        Instantiate(hitExplosion, playerPrefab.transform.position, Quaternion.identity);
        lives--;
        invincibilityTimer = invincibilityTime;
        StartCoroutine(FlashInvincibility());

        if (lives <= 0)
        {
            audioSource.PlayOneShot(deathSFX);
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
