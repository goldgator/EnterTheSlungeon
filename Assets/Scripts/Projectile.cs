using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Vector3 startVelocity;
    private float damage;
    private float knockBack;
    private int pierce;

    public AudioClip hitFX;


    //Events
    public delegate void ProjectileEvent(Projectile projectile);

    public static event ProjectileEvent projectileSpawnEvent;
    public static event ProjectileEvent projectileUpdateEvent;
    public static event ProjectileEvent projectileHitEvent;
    public static event ProjectileEvent projectileDeathEvent;

    public void InstantiateProjectile(Vector3 newStartVelocity, float newDamage, float newKnockBack)
    {
        knockBack = newKnockBack;
        startVelocity = newStartVelocity;
        damage = newDamage;
        if (gameObject.layer == 7) projectileSpawnEvent?.Invoke(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = startVelocity;
        rb.mass = knockBack;
    }

    private void Update()
    {
        if (gameObject.layer == 7) projectileUpdateEvent?.Invoke(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health otherHealth = collision.collider.GetComponent<Health>();

        if (otherHealth)
        {
            otherHealth.SubtractHealth(damage);
            if (gameObject.layer == 7) projectileHitEvent?.Invoke(this);
        }

        AudioSource audioFXOG = Utilities.AudioFXObject.GetComponent<AudioSource>();
        AudioSource audioFX = Instantiate(audioFXOG, transform.position, Quaternion.identity);
        audioFX.clip = hitFX;
        audioFX.Play();
        
        if (pierce <= 0)
        {
            if (gameObject.layer == 7) projectileDeathEvent?.Invoke(this);
            Destroy(gameObject);
        } else
        {
            pierce--;
        }

    }
}
