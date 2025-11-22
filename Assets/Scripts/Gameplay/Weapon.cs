using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    // Configurable properties
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileSpeed = 50f;
    [SerializeField] float baseShootCooldown = .5f;
    [SerializeField] float burstFireProportion = .5f;
    [SerializeField] float spreadAngleDegrees = 45f;

    // Cached references
    private GameManager gameManager;
    private StatsManager statsManager;

    // Internal fields
    private bool _isShooting;
    private float _shootCooldownAccumulator;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        statsManager = FindFirstObjectByType<StatsManager>();
    }

    void Update()
    {
        if (!gameManager.GameIsActive || gameManager.GameIsResetting) return;
        
        Aim();
        Shoot();
    }

    private void Aim()
    {
        // Raycast the mouse position onto the middle plane
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: LayerMask.GetMask("Middle Plane"));

        // Rotate to face this position
        Vector3 directionToLook = hit.point - transform.position;
        if (directionToLook.magnitude < Mathf.Epsilon) return;
        transform.rotation = Quaternion.LookRotation(directionToLook, Vector3.up);
    }

    private void Shoot()
    {
        float currentShootCooldown = baseShootCooldown * (1f / statsManager.AttackSpeedMultiplier);
        if (_shootCooldownAccumulator < currentShootCooldown)
        {
            _shootCooldownAccumulator += Time.deltaTime;
        }

        if (_isShooting && _shootCooldownAccumulator >= currentShootCooldown)
        {
            float burstCooldown = currentShootCooldown * burstFireProportion / statsManager.BurstFireShotCount;
            StartCoroutine(BurstFireCoroutine(statsManager.BurstFireShotCount, burstCooldown));

            _shootCooldownAccumulator = 0f;
        }
    }

    private IEnumerator BurstFireCoroutine(int count, float delayBetweenShots)
    {
        int shotsFired = 0;
        while (shotsFired < count)
        {
            shotsFired++;

            float minAngle = -spreadAngleDegrees;
            float angleStep = spreadAngleDegrees * 2f / (statsManager.SpreadFireShotCount + 1);

            Projectile projectile;
            for (float angle = minAngle + angleStep; angle < spreadAngleDegrees; angle += angleStep)
            {
                Quaternion rotation = projectileSpawnPoint.rotation * Quaternion.AngleAxis(angle, Vector3.up);
                projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, rotation).GetComponent<Projectile>();
                projectile.Speed = projectileSpeed;
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    public void OnShoot(InputValue value)
    {
        _isShooting = value.Get<float>() > 0;
    }
}
