using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    // Configurable properties
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileSpeed = 50f;
    [SerializeField] float baseShootCooldown = .5f;

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
        if (!gameManager.GameIsActive) return;
        
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
            Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation).GetComponent<Projectile>();
            projectile.Speed = projectileSpeed;
            _shootCooldownAccumulator = 0f;
        }
    }

    public void OnShoot(InputValue value)
    {
        _isShooting = value.Get<float>() > 0;
    }
}
