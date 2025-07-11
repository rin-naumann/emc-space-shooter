using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : MonoBehaviour
{
    // Movement related variables
    private float moveSpeed = 5f;
    private float dashMultiplier = 2f;
    private float dashDuration = 0.2f;
    private float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashTimeRemaining;
    public float dashCooldownTimer;

    // Shooting related variables
    public float ammoGun = 20f;
    public float ammoBomb = 5f;
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    public Transform firePoint;
    public float ammoGunRemaining;
    public float ammoBombRemaining = 5f;
    public bool isReloading = false;
    private float nextFireTime = 0.0f;
    public float reloadTime = 2f;

    // Life related variables
    public int lives = 3;

    // Update is called once per frame
    void Update()
    {
        movementHandler();
        dashHandler();
        shootingHandler();
    }

    void movementHandler()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float currentSpeed = isDashing ? moveSpeed * dashMultiplier : moveSpeed;

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * currentSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void dashHandler()
    {
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            dashCooldownTimer = dashCooldown;
        }

        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
            }
        }
    }

    void shootingHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ammoGunRemaining > 0 && !isReloading && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            ammoGunRemaining--;
        }

        if (Input.GetKeyDown(KeyCode.R) && ammoGunRemaining < ammoGun)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.E) && ammoBombRemaining > 0)
        {
            Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
            ammoBombRemaining--;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime); // Simulate reload time
        ammoGunRemaining = ammoGun; // Refill ammo
        reloadTime = 2f; // Reset reload time
        isReloading = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            lives--;
            if (lives <= 0)
            {
                // Handle player death logic here
                Destroy(gameObject); // Destroy the player
            }
        }
    }
}
