using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float rotationSpeed = 20f;

    [Header("Health Settings")]
    public int health = 3;
    private bool hasEnteredScreen;
    private Vector3 moveDirection;

    [Header("Effects")]
    public GameObject explosionEffect;

    void Start()
    {
        // Pick a random direction to drift in
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        moveDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
    }

    void Update()
    {
        // Move and rotate the asteroid
        transform.position += moveDirection * speed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Track if the asteroid has ever entered the screen
        if (IsVisibleFrom(Camera.main))
        {
            hasEnteredScreen = true;
        }
    }

    public void Initialize(Vector2 direction)
    {
        // Optional initializer if spawning manually
        moveDirection = direction.normalized;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Create explosion and grant points
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        ScoreManager.Instance?.AddScore(250);
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        // Destroy only if it was visible before
        if (hasEnteredScreen)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collisions with bullets, bombs, and players
        switch (other.tag)
        {
            case "PlayerProjectile":
            case "EnemyProjectile":
                BulletController bullet = other.GetComponent<BulletController>();
                if (bullet != null)
                {
                    TakeDamage(1);
                    Destroy(other.gameObject);
                }
                break;

            case "Bomb":
                BombController bomb = other.GetComponent<BombController>();
                if (bomb != null)
                {
                    TakeDamage(3);
                    Destroy(other.gameObject);
                }
                break;

            case "Player":
                PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
                playerManager?.TakeDamage();
                Die();
                break;

            case "Enemy":
                EnemyController enemy = other.GetComponentInParent<EnemyController>();
                enemy?.Die();
                break;
        }
    }

    // Check if asteroid is within the camera view
    bool IsVisibleFrom(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider2D>().bounds);
    }
}
