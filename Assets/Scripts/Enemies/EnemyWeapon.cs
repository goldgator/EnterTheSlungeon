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
    public bool swapX = false;

    public GameObject projectile;
    public float shotSpeed;
    public float damage;
    protected float shotTimer = 0;
    public float shotCooldown;
    public float shotCooldownStart;

    protected void Start()
    {
        target = Player.Instance;
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<SpriteRenderer>();

        shotTimer = -shotCooldownStart;
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

    public virtual void Shoot()
    {
        SpawnProjectile();
    }

    protected void SpawnProjectile()
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
        Vector3 newScale = (swapX) ? new Vector3(aimX, aimX, 1) : new Vector3(1, aimX, 1);
        transform.localScale = newScale;
    }

}
