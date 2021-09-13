using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Vector3 startVelocity;
    private float damage;

    public AudioClip hitFX;

    public void InstantiateProjectile(Vector3 newStartVelocity, float newDamage)
    {
        startVelocity = newStartVelocity;
        damage = newDamage;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = startVelocity;
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
