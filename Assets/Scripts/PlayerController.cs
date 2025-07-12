using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SocialPlatforms.Impl;

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
    public float ammoGunRemaining = 25f;
    public float ammoBombRemaining = 5f;
    public bool isReloading = false;
    private float nextFireTime = 0.0f;
    public float reloadTime = 1f;
    private int scoreForNextBomb = 2000;

    // SFX related variables
    private AudioSource audioSource;
    public AudioClip shootingSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
            audioSource.PlayOneShot(shootingSFX);
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
        reloadTime = 1f; // Reset reload time
        isReloading = false;
    }

    void BombReloader()
    {
        int score = ScoreManager.Instance.score;
        if (score == scoreForNextBomb)
        {
            ammoBombRemaining++;
            scoreForNextBomb += scoreForNextBomb;
        }
    }
}
