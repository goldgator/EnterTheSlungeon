using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider))]
public class RushEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float speed = 0;
    public float damage = 0;
    public float attackRange = 0;
    public bool contactFocused;

    [Header("Details")]
    private const string fxPath = "Prefabs/Enemies/Deaths/";
    public string deathFX;


    //Private fields
    private Player target;
    private Health health;
    private Animator animator;

    private void Start()
    {
        target = Player.Instance;
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.HasDied)
        {
            OnMove();
        }
    }

    private void OnMove()
    {
        //Get distance vector
        Vector3 distance = target.transform.position - transform.position;
        
        if (distance.magnitude > attackRange)
        {
            float travel = Mathf.Min(distance.magnitude, speed);
            Vector3 velocity = travel * distance.normalized;

            transform.position += velocity * Time.deltaTime;
        }
    }

    public void Death()
    {
        //Create deathFX
        GameObject fxObject = Resources.Load<GameObject>(fxPath + deathFX);
        Instantiate(fxObject, transform.position, Quaternion.identity);
        //Destroy this object
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health otherHealth = collision.collider.GetComponent<Health>();
            otherHealth.SubtractHealth(damage);
        }
    }
}
