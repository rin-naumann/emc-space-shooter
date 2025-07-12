using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public GameObject explosionEffect;

    void Start()
    {
        if (CompareTag("PlayerProjectile"))
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f); // Light blue
        }
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("EnemyProjectile") && other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
            if (playerManager != null && !playerManager.isInvincible)
            {
                playerManager.TakeDamage();
            }

            Destroy(gameObject);
        }
        else if (CompareTag("PlayerProjectile") && other.CompareTag("Enemy"))
        {
            // Let the EnemyController handle its own explosion
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController == null)
                enemyController = other.GetComponentInParent<EnemyController>();

            if (enemyController != null)
            {
                enemyController.Die(); // This handles its own explosion
            }

            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
