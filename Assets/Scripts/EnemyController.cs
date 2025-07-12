using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float chargeSpeed = 10f;
    public bool isCharging = false;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public GameObject explosionEffect;
    public Transform firePoint;
    private bool isShooting = false;

    [Header("Audio Settings")]
    public AudioClip shootingSFX;
    public AudioClip deathSFX;

    public System.Action OnDeath;

    void Start()
    {
        StartCoroutine(EntryPhase());
    }

    public void Die()
    {
        OnDeath?.Invoke(); // Notify subscribers
        GameManager.Instance.playSFX(deathSFX);
        ScoreManager.Instance?.AddScore(100);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator EntryPhase()
    {
        yield return new WaitForSeconds(1f);

        float t = 0f;
        float moveDuration = 1f;
        Vector3 start = transform.position;
        Vector3 end = new Vector3(6, Random.Range(-4f, 4f), 0);

        transform.rotation = Quaternion.Euler(0, 0, 90); // Face left

        while (t < moveDuration)
        {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        StartCoroutine(ShootingPhase());
    }

    IEnumerator ShootingPhase()
    {
        isShooting = true;
        float duration = Random.Range(3f, 8f);
        float elapsed = 0f;

        yield return new WaitForSeconds(0.5f); // Delay before first shot

        while (elapsed < duration)
        {
            GameManager.Instance.playSFX(shootingSFX);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            float fireRate = Random.Range(0.1f, 2f);
            elapsed += fireRate;
            yield return new WaitForSeconds(fireRate);
        }

        isShooting = false;
        StartCoroutine(MovePhase());
    }

    IEnumerator MovePhase()
    {
        yield return new WaitForSeconds(1f);

        float t = 0f;
        float duration = 1f;
        float rotateSpeed = 360f;
        Vector3 start = transform.position;
        Vector3 end = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));

            transform.position = Vector3.Lerp(start, end, progress);

            // Rotate toward target position
            Vector3 dir = (end - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle - 90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        // Choose next state
        if (transform.position.x <= 0)
            StartCoroutine(MoveTowardPlayer());
        else
            StartCoroutine(ShootingPhase());
    }

    IEnumerator MoveTowardPlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        float aimTime = 1.5f;
        float timer = 0f;
        Vector3 lockedDir = Vector3.zero;

        // Track the player before charging
        while (timer < aimTime)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                Vector3 currentDir = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                lockedDir = currentDir;
            }

            yield return null;
        }

        // Dash in locked direction
        isCharging = true;
        while (isCharging)
        {
            transform.position += lockedDir * chargeSpeed * Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (isShooting && player != null)
        {
            // Continuously aim at player while shooting
            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        // Destroy enemy when charging off-screen
        if (isCharging && isOffScreen())
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
            playerManager.TakeDamage();
            Die();
        }
    }

    bool isOffScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }
}
