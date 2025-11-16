using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    // Configurable properties
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float projectileSpeed = 50f;
    [SerializeField] float shootCooldown = 0.25f;

    // Cached references
    private GameManager gameManager;

    // Internal fields
    private bool _isShooting;
    private float _shootCooldownAccumulator;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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
        if (_shootCooldownAccumulator < shootCooldown)
        {
            _shootCooldownAccumulator += Time.deltaTime;
        }

        if (_isShooting && _shootCooldownAccumulator >= shootCooldown)
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
