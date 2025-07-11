using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : MonoBehaviour
{
    // Movement related variables
    private float moveSpeed = 5f;
    private float dashMultiplier = 3f;
    private float dashDuration = 0.2f;
    private float dashCooldown = 1f;
    public bool isDashing = false;
    private float dashTimeRemaining;
    public float dashCooldownTimer;

    // Shooting related variables
    public float ammoGun = 25f;
    public float ammoBomb = 5f;
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    public Transform firePoint;
    public float ammoGunRemaining;
    public float ammoBombRemaining = 5f;
    public bool isReloading = false;
    private float nextFireTime = 0.0f;
    public float reloadTime = 1f;

    // Update is called once per frame
    void Update()
    {
        MovementHandler();
        DashHandler();
        ShootingHandler();
        BombReloader();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        // Get orthographic camera bounds using screen corners
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float padding = 0.5f; // adjust based on your player size

        pos.x = Mathf.Clamp(pos.x, bottomLeft.x + padding, topRight.x - padding);
        pos.y = Mathf.Clamp(pos.y, bottomLeft.y + padding, topRight.y - padding);

        transform.position = pos;
    }

    void MovementHandler()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float currentSpeed = isDashing ? moveSpeed * dashMultiplier : moveSpeed;

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * currentSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void DashHandler()
    {
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            PlayerManager pm = GetComponent<PlayerManager>();
            pm.isInvincible = true; // Set player invincible during dash
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
                PlayerManager pm = GetComponent<PlayerManager>();
                pm.isInvincible = false; // Reset invincibility after dash
            }
        }
    }

    void ShootingHandler()
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

    void BombReloader()
    {
        if ((ScoreManager.Instance?.score % 2500 == 0) && ScoreManager.Instance?.score != 0)
        {
            ammoBombRemaining++;
        }
    }
}
