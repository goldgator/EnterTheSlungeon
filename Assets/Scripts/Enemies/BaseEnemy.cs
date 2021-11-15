using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour, IHealthDeath
{
    [Header("Enemy Stats")]
    public float speed = 0;
    public float damage = 0;
    public float attackRange = 0;
    public float attackCooldown = 0;
    public AudioClip attackSound;

    [Header("Details")]
    protected const string fxPath = "Prefabs/Enemies/Deaths/";
    public string deathFX;
    public bool flipSprite = false;
    protected int flipMod { get { return (flipSprite) ? -1 : 1; } }

    //Protected fields
    protected Player target;
    protected Health health;
    protected Animator animator;
    protected Collider2D collider;
    protected SpriteRenderer renderer;
    protected Rigidbody2D rb;
    protected AudioSource audioSource;
    protected float stopTime = 0;
    protected EnemySpawn homeSpawner;

    //Events
    public delegate void EnemyEvent();

    public static event EnemyEvent enemyDeathEvent;
    public static event EnemyEvent enemySpawnEvent;


    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected virtual void Start()
    {
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        target = Player.Instance;
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<SpriteRenderer>();
        if (audioSource) audioSource.clip = attackSound;

        enemySpawnEvent?.Invoke();

        //Delay action if enemy starts with a Spawn animation
        DelayAction();
    }

    public void ShareSpawner(BaseEnemy friendEnemy)
    {
        friendEnemy.homeSpawner = homeSpawner;
        homeSpawner.AddTrackedEnemy(friendEnemy);
    }

    private void DelayAction()
    {
        //Check if it has a spawn anim
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
        {
            stopTime = animator.GetCurrentAnimatorStateInfo(0).length * 1.2f;
        }
    }

    public virtual void Death()
    {
        //Create deathFX
        if (deathFX != "")
        {
            GameObject fxObject = Resources.Load<GameObject>(fxPath + deathFX);
            GameObject newFX = Instantiate(fxObject, transform.position, Quaternion.identity);
            newFX.transform.localScale = transform.localScale;
        }
        //Destroy this object
        homeSpawner?.EnemyDied(this);
        enemyDeathEvent?.Invoke();
        Destroy(gameObject);
    }

    public void InstantiateEnemy(EnemySpawn newSpawn)
    {
        homeSpawner = newSpawn;
        newSpawn.AddTrackedEnemy(this);
    }

    public void OnDeath()
    {
        Death();
    }
}
