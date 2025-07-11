using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Public variables
    public float speed = 20f;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(0f, 255f, 255f); // Set bullet color to light blue
        transform.rotation = Quaternion.Euler(0, 0, 270); // Ensure the bullet is facing right
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Handle bullet hit logic here
            Destroy(other.gameObject); // Destroy the enemy
            Destroy(gameObject); // Destroy the bullet
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
