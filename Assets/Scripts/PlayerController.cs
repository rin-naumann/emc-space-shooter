using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float dashMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool isDashing = false;
    private float dashTimeRemaining;
    public float dashCooldownTimer;

    [Header("Gun Settings")]
    public float ammoGun = 25f;
    public float ammoGunRemaining = 25f;
    public float reloadTime = 1f;
    public float reloadTimer = 0f;
    public bool isReloading = false;

    [Header("Bomb Settings")]
    public float ammoBomb = 5f;
    public float ammoBombRemaining = 5f;
    public GameObject bombPrefab;
    private int nextBombThreshold = 2000;

    [Header("Life Reward")]
    private int nextLifeThreshold = 5000;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float nextFireTime = 0f;
    public float fireRate = 0.15f;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip shootingSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleMovement();
        HandleDash();
        HandleShooting();
        CheckRewards();
    }

    void LateUpdate()
    {
        ClampPositionToScreen();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float currentSpeed = isDashing ? moveSpeed * dashMultiplier : moveSpeed;

        Vector2 direction = new Vector2(h, v).normalized;
        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }

    void HandleDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            dashCooldownTimer = dashCooldown;
            GetComponent<PlayerManager>().isInvincible = true;
        }

        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
                GetComponent<PlayerManager>().isInvincible = false;
            }
        }
    }

    void HandleShooting()
    {
        // Hold to shoot
        if (Input.GetKey(KeyCode.Space) && ammoGunRemaining > 0 && !isReloading && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            audioSource.PlayOneShot(shootingSFX);
            ammoGunRemaining--;
            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetKeyDown(KeyCode.R) && ammoGunRemaining < ammoGun && !isReloading)
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
    reloadTimer = reloadTime;

    while (reloadTimer > 0f)
    {
        reloadTimer -= Time.deltaTime;
        yield return null;
    }

    ammoGunRemaining = ammoGun;
    isReloading = false;
}

    void CheckRewards()
    {
        int currentScore = ScoreManager.Instance.score;
        PlayerManager pm = GetComponent<PlayerManager>();

        // Bomb every 2000 points
        while (currentScore >= nextBombThreshold)
        {
            ammoBombRemaining++;
            nextBombThreshold += 2000;
        }

        // Life every 5000 points
        while (currentScore >= nextLifeThreshold)
        {
            pm.lives++;
            nextLifeThreshold += 5000;
        }
    }

    void ClampPositionToScreen()
    {
        Vector3 pos = transform.position;
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float padding = 0.5f;

        pos.x = Mathf.Clamp(pos.x, bottomLeft.x + padding, topRight.x - padding);
        pos.y = Mathf.Clamp(pos.y, bottomLeft.y + padding, topRight.y - padding);
        transform.position = pos;
    }
}
