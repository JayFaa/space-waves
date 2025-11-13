using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed { get; set; }
 
    void Update()
    {
        transform.position += Speed * Time.deltaTime * transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
