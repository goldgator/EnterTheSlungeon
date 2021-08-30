using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RushEnemy : BaseEnemy
{
    [Header("Enemy Stats")]
    public bool hasAttack;

    

    // Update is called once per frame
    void Update()
    {
        stopTime -= Time.deltaTime;
        if (!health.HasDied && stopTime <= 0)
        {
            OnMove();
            OnAttack();
        }
    }

    private void OnMove()
    {
        //Get distance vector
        Vector3 distance = target.transform.position - transform.position;

        //Orient sprite
        transform.localScale = new Vector3(Mathf.Sign(distance.x), transform.localScale.y, transform.localScale.z);
        
        //if (distance.magnitude > attackRange)
        Vector3 velocity = speed * distance.normalized;

        velocity = velocity * Time.deltaTime;
        transform.position += velocity;
    }

    private void OnAttack()
    {
        if (hasAttack)
        {
            Vector3 distance = target.transform.position - transform.position;

            if (distance.magnitude < (attackRange * 0.7f))
            {
                animator.Play("Attack");
                stopTime = attackCooldown;
            }
        }  
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasAttack)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Health otherHealth = collision.collider.GetComponent<Health>();

                if (otherHealth)
                {
                    otherHealth.SubtractHealth(damage); 
                }
            }
        }
    }

    public void Attack()
    {
        //Play sound
        if (audioSource) audioSource.Play();

        //Attempt to attack
        Vector2 position = new Vector2(transform.position.x, transform.position.y) + collider.offset;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position, attackRange, new Vector2());
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Health health = Player.Instance.GetComponent<Health>();
                if (health)
                {
                    health.SubtractHealth(damage);
                }
                break;
            }
        }
    }
}
