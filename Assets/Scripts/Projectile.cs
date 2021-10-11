using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Vector3 startVelocity;
    private float damage;
    private float knockBack;

    public AudioClip hitFX;

    public void InstantiateProjectile(Vector3 newStartVelocity, float newDamage, float newKnockBack)
    {
        knockBack = newKnockBack;
        startVelocity = newStartVelocity;
        damage = newDamage;
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = startVelocity;
        rb.mass = knockBack;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health otherHealth = collision.collider.GetComponent<Health>();

        if (otherHealth)
        {
            otherHealth.SubtractHealth(damage);
        }

        AudioSource audioFXOG = Utilities.AudioFXObject.GetComponent<AudioSource>();
        AudioSource audioFX = Instantiate(audioFXOG, transform.position, Quaternion.identity);
        audioFX.clip = hitFX;
        audioFX.Play();
        

        Destroy(gameObject);
    }
}
