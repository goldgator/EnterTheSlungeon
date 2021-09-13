using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
public class BaseEnemy : MonoBehaviour
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

    //Protected fields
    protected Player target;
    protected Health health;
    protected Animator animator;
    protected Collider2D collider;
    protected AudioSource audioSource;
    protected float stopTime = 0;
    protected EnemySpawn homeSpawner;

    protected virtual void Start()
    {
        collider = GetComponent<Collider2D>();
        target = Player.Instance;
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.clip = attackSound;
    }

    public void Death()
    {
        //Create deathFX
        GameObject fxObject = Resources.Load<GameObject>(fxPath + deathFX);
        GameObject newFX = Instantiate(fxObject, transform.position, Quaternion.identity);
        newFX.transform.localScale = transform.localScale;
        //Destroy this object
        homeSpawner?.EnemyDied();
        Destroy(gameObject);
    }

    public void InstantiateEnemy(EnemySpawn newSpawn)
    {
        homeSpawner = newSpawn;
    }
}
