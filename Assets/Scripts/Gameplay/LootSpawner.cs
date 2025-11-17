using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] ParticleSystem lootSpawnEffect;
    [SerializeField] GameObject lootItemPrefab;

    private List<ParticleSystem.Particle> _particles = new();

    public void SpawnLoot(int amount)
    {
        lootSpawnEffect.Emit(amount);
    }

    public void OnParticleTrigger()
    {
        // Fill the list with collision events from the particle system
        lootSpawnEffect.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, _particles);

        // Spawn loot items at each particle collision point
        foreach (ParticleSystem.Particle p in _particles)
        {
            Rigidbody lootRB = Instantiate(lootItemPrefab, p.position, Quaternion.identity).GetComponent<Rigidbody>();
            lootRB.linearVelocity = p.velocity;
        }
    }
}