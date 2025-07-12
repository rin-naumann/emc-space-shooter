using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Public variables
    public float speed = 20f;
    public GameObject explosionEffect; // Assign an explosion effect prefab in the inspector

    void Start()
    {
        
        
    }

    void Update()
{
    if (CompareTag("PlayerProjectile"))
    {
        GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f); // light blue
    }

    // ✅ Move forward in the direction the bullet is facing
    transform.position += transform.up * speed * Time.deltaTime;
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CompareTag("EnemyProjectile"))
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity); // Instantiate explosion effect
            PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
            playerManager.TakeDamage(); // Call TakeDamage method from PlayerManager
            Destroy(gameObject); // Destroy the bullet

        }
        else if (other.CompareTag("Enemy") && CompareTag("PlayerProjectile"))
        {
            EnemyController enemyController = other.GetComponentInParent<EnemyController>();
            enemyController.Die(); // Call Die method to handle enemy death
            Destroy(other.gameObject); // Destroy the enemy
            Destroy(gameObject); // Destroy the bullet
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
