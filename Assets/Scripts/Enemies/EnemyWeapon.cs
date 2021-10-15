using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class EnemyWeapon : MonoBehaviour
{
    protected SpriteRenderer renderer;
    protected Player target;
    protected AudioSource audioSource;
    public Transform shootingTransform;

    public GameObject projectile;
    public float shotSpeed;
    public float damage;
    private float shotTimer = 0;
    public float shotCooldown;

    protected void Start()
    {
        target = Player.Instance;
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        shotTimer += Time.deltaTime;
        Aim();
        if (shotTimer > shotCooldown)
        {
            shotTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        //Play sound
        if (audioSource) audioSource.Play();

        //Create projectile
        Vector3 distance = target.transform.position - shootingTransform.position;
        GameObject newProjectile = Instantiate(projectile, shootingTransform.position, Quaternion.identity);

        newProjectile.GetComponent<Projectile>().InstantiateProjectile(distance.normalized * shotSpeed, damage, 1);
    }

    private void Aim()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        CorrectDirAndScale(direction);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void CorrectDirAndScale(Vector3 direction)
    {
        //Determine player and weapon scale
        float aimX = Mathf.Sign(direction.x);
        transform.localScale = new Vector3(aimX, aimX, 1);
    }

}
