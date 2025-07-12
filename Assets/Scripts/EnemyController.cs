using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using NUnit.Framework;

public class EnemyController : MonoBehaviour
{
    // Movement related variables
    public float speed = 2f;
    public float chargeSpeed = 10f;
    public bool isCharging = false;

    // Shooting related variables
    public GameObject bulletPrefab; 
    public GameObject ExplosionEffect;
    public Transform firePoint;
    private bool isShooting = false;

    // SFX related variables
    private AudioSource audioSource;
    public AudioClip shootingSFX;
    public AudioClip deathSFX;

    // On Death handler
    public System.Action OnDeath;

    void Start()
    {
        StartCoroutine(EntryPhase());
        audioSource = GetComponent<AudioSource>();
    }

    public void Die()
    {
        OnDeath?.Invoke(); // Invoke the death event if there are any subscribers
        audioSource.PlayOneShot(deathSFX);
        ScoreManager.Instance?.AddScore(100);
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject); // Destroy the enemy object
    }
    IEnumerator EntryPhase()
    {
        yield return new WaitForSeconds(1f); // Wait before starting the movement
        float moveDuration = 1f;
        float t = 0f;
        Vector3 startPosition = transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 90); // Ensure the enemy is facing left
        Vector3 targetPosition = new Vector3(6, Random.Range(-4f, 4f), 0); // Move towards the right side of the screen
        while (t < moveDuration)
        {
            t += Time.deltaTime / moveDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t); // Smooth transition
            transform.position = Vector3.Lerp(startPosition, targetPosition, ease);
            yield return null; // Wait for the next frame
        }
        StartCoroutine(ShootingPhase());
    }

    IEnumerator ShootingPhase()
    {
        isShooting = true;

        float shootingPhaseDuration = Random.Range(3f, 8f);
        float elapsedTime = 0f;

        yield return new WaitForSeconds(0.5f);

        while (elapsedTime < shootingPhaseDuration)
        {
            audioSource.PlayOneShot(shootingSFX);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            float fireRate = Random.Range(0.1f, 2f);
            elapsedTime += fireRate;
            yield return new WaitForSeconds(fireRate);
        }

        isShooting = false;
        StartCoroutine(MovePhase());
    }

    IEnumerator MovePhase()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);
        float moveDuration = 1f; // adjust this for faster/slower total move time
        float rotateSpeed = 360f; // rotation speed (degrees per second)
        float t = 0f;

        yield return new WaitForSeconds(1f); // optional pause before moving

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / moveDuration);
            float eased = Mathf.SmoothStep(0f, 1f, progress); // easing for smooth start/end

            // Smooth movement
            transform.position = Vector3.Lerp(startPosition, targetPosition, eased);

            // Rotation toward target
            Vector3 direction = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        // Decide next action after reaching the target
        if (transform.position.x <= 0)
        {
            StartCoroutine(MoveTowardPlayer());
        }
        else
        {
            StartCoroutine(ShootingPhase());
        }
    }

    IEnumerator MoveTowardPlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        float aimDuration = 1.5f;
        float timer = 0f;

        Vector3 lockedDirection = Vector3.zero;

        // Aiming Phase: Track the player with rotation
        while (timer < aimDuration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                Vector3 currentDirection = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle-90); // +180 for left-facing sprite

                lockedDirection = currentDirection; // Update aim direction every frame
            }

            yield return null;
        }

        // Dash Phase: Lock the last known direction and move straight
        isCharging = true;
        while (isCharging)
        {
            transform.position += lockedDirection * chargeSpeed * Time.deltaTime;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
            playerManager.TakeDamage(); // Call TakeDamage method from 
            Die(); // Call Die method to handle enemy death
        }
    }

    void Update()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (isOffScreen() && isCharging)
        {
            Destroy(gameObject); // Destroy the enemy if it goes off-screen AFTER charging
        }
        
        // Aim at the player while shooting
        if (isShooting && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (isCharging && isOffScreen())
        {
            Destroy(gameObject);
        }
    }
    
    bool isOffScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }
}
