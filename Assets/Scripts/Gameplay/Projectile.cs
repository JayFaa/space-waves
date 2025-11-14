using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 25;

    public float Speed { get; set; }
 
    void Update()
    {
        transform.position += Speed * Time.deltaTime * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
