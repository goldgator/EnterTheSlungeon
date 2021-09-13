using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    Collider2D hitbox;
    public float damage;
    public bool boxEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (boxEnabled)
        {
            Health health = collision.GetComponent<Health>();

            if (health)
            {
                health.SubtractHealth(damage);
            }
        }
    }
}
