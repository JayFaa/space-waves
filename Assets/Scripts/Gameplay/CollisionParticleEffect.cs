using UnityEngine;

public class CollisionParticleEffect : MonoBehaviour
{
    void Awake()
    {
        Destroy(gameObject, GetComponentInChildren<ParticleSystem>().main.startLifetime.constantMax);
    }
}
