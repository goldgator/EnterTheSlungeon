using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : BaseEnemy
{
    public GameObject projectile;
    public float shotSpeed;


    void Update()
    {
        stopTime -= Time.deltaTime;
        if (!health.HasDied && stopTime <= 0)
        {
            OnMove();
            OnAttack();
        }
    }

    protected override void Start()
    {
        base.Start();
        stopTime = RNGManager.GetEventRand(attackCooldown / 4f, attackCooldown / 2f);
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
        Vector3 distance = target.transform.position - transform.position;

        if (distance.magnitude < (attackRange * 0.7f))
        {
            animator.Play("Attack");
            stopTime = attackCooldown;
        }
    }

    public void Attack()
    {
        //Play sound
        if (audioSource) audioSource.Play();

        //Create projectile
        Vector3 distance = target.transform.position - transform.position;
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        newProjectile.GetComponent<Projectile>().InstantiateProjectile(distance.normalized * shotSpeed, damage, 1);
    }
}
