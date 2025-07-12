using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float speed = 5f;
    public float explosionRadius = 10f;
    public float fuseTime = 1f;

    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip explosionSFX;

    private bool hasExploded = false;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 270); // Face forward (right in 2D)
        Invoke(nameof(Explode), fuseTime); // Auto-explode after fuse
    }

    void Update()
    {
        // Moves the bomb forward each frame
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasExploded && other.CompareTag("Enemy"))
        {
            Explode(); // Manual detonation on enemy hit
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Spawn explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Detect all colliders in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController ec = hit.GetComponent<EnemyController>();
                if (ec != null)
                {
                    ec.Die();
                }
            }
        }

        // Play explosion SFX from global manager
        GameManager.Instance.playSFX(explosionSFX);

        Destroy(gameObject); // Destroy bomb object
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject); // Auto-destroy if off-screen
    }
}
