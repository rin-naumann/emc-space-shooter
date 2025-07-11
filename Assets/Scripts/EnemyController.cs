using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // Public variables
    public float speed = 2f;
    public float chargeSpeed = 10f;
    public bool isCharging = false;
    public GameObject bulletPrefab; 
    public Transform firePoint;
    public float fireRate = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EntryPhase());
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
        float shootingPhaseDuration = Random.Range(3f, 8f);
        float elapsedTime = 0f;
        while (elapsedTime < shootingPhaseDuration)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            elapsedTime += fireRate;
            yield return new WaitForSeconds(fireRate);
        }
        StartCoroutine(MovePhase());
    }

    IEnumerator MovePhase()
    {
        Vector3 targetPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);
        yield return new WaitForSeconds(1f); // Wait before moving
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        if (transform.position.x <= 0)
        {
            StartCoroutine(MoveTowardPlayer());
        }
        else
        {
            StartCoroutine(ShootingPhase()); // Restart shooting phase
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

    void Update()
    {
        if (isOffScreen() && isCharging)
        {
            Destroy(gameObject); // Destroy the enemy if it goes off-screen AFTER charging
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject); // Destroy the enemy when hit by a player's projectile
        }
    }
    
    bool isOffScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }
}
