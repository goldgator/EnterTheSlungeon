using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatBlock))]
public class BaseWeapon : MonoBehaviour
{

    [Header("Components")]
    public new SpriteRenderer renderer;
    public Transform shotTransform;
    protected AudioSource audioSource;

    [Header("Assets")]
    public Sprite gunSprite;
    public GameObject projectile;
    public AudioClip gunSound;

    //[Header("Stats")]
    //[SerializeField]
    protected StatBlock stats;
    

    //Misc Fields
    protected float fireTimer = 0;

    private void Awake()
    {
        stats = GetComponent<StatBlock>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gunSound;
        renderer.sprite = gunSprite;
    }


    protected virtual void OnEnable()
    {
        if (InputManager.Instance) InputManager.Instance.fireUpdateEvent += OnFire;
    }

    protected virtual void OnDisable()
    {
        if (InputManager.Instance) InputManager.Instance.fireUpdateEvent -= OnFire;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;  
    }

    private void FixedUpdate()
    {
        if (Player.Instance.GetPlayerEnabled())
        {
            ProcessAim();
        }
    }

    protected void ProcessAim()
    {
        if (InputManager.isGamepad)
        {
            ProcessAimGamePad();
        }
        else
        {
            ProcessAimKeyboard();
        }
    }

    private void ProcessAimGamePad()
    {

    }

    private void ProcessAimKeyboard()
    {
        Vector3 worldMousePos = InputManager.Instance.MouseWorldPosition;

        //Weapon
        Vector3 direction = (worldMousePos - transform.position).normalized;
        CorrectDirAndScale(direction);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }

    private void CorrectDirAndScale(Vector3 direction)
    {
        //Determine player and weapon scale
        float aimX = Mathf.Sign(direction.x);
        Player.Instance.transform.localScale = new Vector3(aimX, 1, 1);
        transform.localScale = new Vector3(aimX, aimX, 1);
    }

    private void CorrectDirAndScale()
    {
        Vector3 worldMousePos = InputManager.Instance.MouseWorldPosition;
        Vector3 direction = (worldMousePos - transform.position).normalized;

        //Determine player and weapon scale
        float aimX = Mathf.Sign(direction.x);
        Player.Instance.transform.localScale = new Vector3(aimX, 1, 1);
        transform.localScale = new Vector3(aimX, aimX, 1);
    }

    protected virtual void OnFire(bool pressed)
    {
        if (fireTimer >= stats.GetStatValue("FireRate") && pressed)
        {
            Fire();
        }
    }

    protected virtual void Fire()
    {
        fireTimer = 0;
        SpawnProjectile();
    }

    protected virtual void SpawnProjectile()
    {
        //Call ProcessAim once more to ensure scales and directions have been updated correctly before shot
        ProcessAim();

        //Make Sound
        audioSource.Play();

        //Create projectile
        GameObject newProjectile = Instantiate(projectile, shotTransform.position, Quaternion.identity);
        float size = stats.GetStatValue("ShotSize");
        newProjectile.transform.localScale = new Vector3(size, size, size);

        //Add velocity
        Vector3 velocity = stats.GetStatValue("ShotSpeed") * transform.right;
        newProjectile.GetComponent<Projectile>().InstantiateProjectile(velocity, stats.GetStatValue("ShotDamage"), stats.GetStatValue("KnockBack"));
    }
}
