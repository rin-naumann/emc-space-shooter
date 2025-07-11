using UnityEngine;

public class BombController : MonoBehaviour
{
    // Public variables
    public float speed = 5f;
    public float explosionRadius = 10f;
    public float fuseTime = 1f;
    public GameObject explosionEffect; // Assign an explosion effect prefab in the inspector

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 270); // Ensure the bullet is facing right
        Invoke("Explode", fuseTime); // Schedule explosion after fuse time
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Handle explosion logic here
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity); // Instantiate explosion effect

        // Logic for explosion effect, e.g., damage enemies within radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyController ec = enemy.GetComponent<EnemyController>();
                if (ec != null) ec.Die();
            }
        }
        Destroy(gameObject); // Destroy the bomb after explosion
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
