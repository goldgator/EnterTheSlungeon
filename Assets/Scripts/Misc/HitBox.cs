using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    Collider2D hitbox;
    public float damage;
    public bool boxEnabled = false;

    [SerializeField]
    private float tickSpeed = .2f;

    [SerializeField]
    private float lifetime = float.MaxValue;

    private float tickTimer;
    private bool didDamage;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Collider2D>();
    }

    private void Update()
    {
        tickTimer -= Time.deltaTime;
        lifetime -= Time.deltaTime;

        if (lifetime <= 0) boxEnabled = false;

        if (didDamage)
        {
            tickTimer = tickSpeed;
            didDamage = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DoDamage(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DoDamage(collision);
    }

    private void DoDamage(Collider2D collision)
    {
        if (boxEnabled && tickTimer < 0)
        {
            Health health = collision.GetComponent<Health>();

            if (health)
            {
                health.SubtractHealth(damage);
                didDamage = true;
            }
        }
    }
}
