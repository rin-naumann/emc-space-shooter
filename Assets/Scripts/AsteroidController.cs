using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AsteroidController : MonoBehaviour
{
    public float speed = 2f;
    public float rotationSpeed = 20f;
    public int health = 3;
    public GameObject explosionEffect;
    private bool hasEnteredScreen;
    private Vector3 moveDirection;

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        // Choose a random movement direction (normalized)
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        moveDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (IsVisibleFrom(Camera.main))
        {
            hasEnteredScreen = true;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void OnBecameInvisible()
    {
        if (hasEnteredScreen)
        {
            Destroy(gameObject);
        }
    }

    void Die()
    {
        
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        ScoreManager.Instance?.AddScore(100); // Add score for destroying the asteroid
        Destroy(gameObject);
    }

    bool IsVisibleFrom(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider2D>().bounds);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "EnemyProjectile":
                BulletController enemyBullet = other.GetComponent<BulletController>();
                if (enemyBullet != null)
                {
                    TakeDamage(1);
                    Destroy(other.gameObject);
                }
                break;
                
            case "PlayerProjectile":
                BulletController bullet = other.GetComponent<BulletController>();
                if (bullet != null)
                {
                    TakeDamage(1);
                    Destroy(other.gameObject); // Destroy the bullet
                }
                break;

            case "Bomb":
                BombController bomb = other.GetComponent<BombController>();
                if (bomb != null)
                {
                    TakeDamage(3);
                    Destroy(other.gameObject); // Destroy the bomb
                }
                break;

            case "Player":
                PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
                playerManager.TakeDamage(); // Call TakeDamage method from PlayerManager
                Die(); // Destroy the asteroid
                break;

            case "Enemy":
                EnemyController enemyController = other.GetComponentInParent<EnemyController>();
                enemyController.Die(); // Call Die method to handle enemy death
                break;
        }
    }
}
