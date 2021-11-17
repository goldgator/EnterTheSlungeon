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
    protected GunUI gunUI;

    [Header("Assets")]
    public Sprite gunSprite;
    public GameObject projectile;
    public AudioClip gunSound;

    [Header("Details")]
    public string gunName;

    [Header("Upgrades")]
    public List<UpgradePath> upgradePaths = new List<UpgradePath>();
    //[Header("Stats")]
    //[SerializeField]
    protected StatBlock stats;


    //Misc Fields
    protected bool equipped = false;
    protected float fireTimer = 0;
    protected int remainingAmmo = 0;
    protected bool isReloading = false;
    protected bool overheated = false;
    protected float maxOverheat = 100;
    protected float currentOverheat = 0;

    private void Awake()
    {
        stats = GetComponent<StatBlock>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gunSound;
        renderer.sprite = gunSprite;
        remainingAmmo = (int)stats.GetStatValue("ClipSize");
    }

    

    public void SetEquipped(bool newState)
    {
        equipped = newState;
        renderer.enabled = newState;
        
    }

    

    public StatBlock GetStatBlock()
    {
        if (stats == null) stats = GetComponent<StatBlock>();

        return stats;
    }

    public int GetRemainingAmmo()
    {
        return remainingAmmo;
    }

    public int GetMaxAmmo()
    {
        return (int)stats.GetStatValue("ClipSize");
    }

    public void AddUI(GunUI newUI)
    {
        gunUI = newUI;
    }

    protected virtual void OnEnable()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.fireUpdateEvent += OnFire;
            InputManager.Instance.reloadStartEvent += OnReload;
        }
    }

    protected virtual void OnDisable()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.fireUpdateEvent -= OnFire;
            InputManager.Instance.reloadStartEvent -= OnReload;
        }
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        UpdateOverheat();
    }

    private void FixedUpdate()
    {
        if (Player.Instance.GetPlayerEnabled() && equipped)
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

    private void UpdateOverheat()
    {
        //Change recovery speed if overheated or not
        float heatModifer = (overheated) ? 1 : 2;
        currentOverheat = Mathf.Max(0, currentOverheat - (Time.deltaTime * stats.GetStatValue("OverheatRecovery") * heatModifer));
        float progress = currentOverheat / maxOverheat;

        if (currentOverheat == 0) overheated = false;
        if (equipped) gunUI.UpdateOverheat(overheated, progress);
    }

    protected void AddOverheat()
    {
        float addend = stats.GetStatValue("OverheatAddend");
        currentOverheat += addend;

        if (currentOverheat > maxOverheat)
        {
            overheated = true;
        }
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
        Player.Instance.FlipSprite((aimX < 0));
        //Player.Instance.transform.localScale = new Vector3(aimX, 1, 1);
        //transform.localScale = new Vector3(1, aimX, 1);
        renderer.flipY = (aimX < 0);
    }

    private void CorrectDirAndScale()
    {
        Vector3 worldMousePos = InputManager.Instance.MouseWorldPosition;
        Vector3 direction = (worldMousePos - transform.position).normalized;

        //Determine player and weapon scale
        float aimX = Mathf.Sign(direction.x);
        //Player.Instance.transform.localScale = new Vector3(aimX, 1, 1);
        Player.Instance.FlipSprite((aimX < 0));
        //transform.localScale = new Vector3(1, aimX, 1);
        renderer.flipY = (aimX < 0);
    }

    protected virtual void OnFire(bool pressed)
    {
        if (fireTimer >= stats.GetStatValue("FireRate") && pressed && remainingAmmo > 0 && !isReloading && !overheated && equipped)
        {
            Fire();
        }
    }

    protected virtual void OnReload()
    {
        int maxAmmo = (int)stats.GetStatValue("ClipSize");
        if (remainingAmmo < maxAmmo && !isReloading && !overheated)
        {
            StartCoroutine(Reload(maxAmmo));
        }
    }

    protected virtual IEnumerator Reload(int maxAmmo)
    {
        isReloading = true;
        float reloadTime = stats.GetStatValue("ReloadSpeed");
        float currentTime = 0;

        do
        {
            currentTime += Time.deltaTime;
            gunUI.UpdateReload(currentTime / reloadTime);
            yield return null;
        } while (currentTime < reloadTime);

        isReloading = false;
        remainingAmmo = maxAmmo;
        gunUI.UpdateAmmo();
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

        //Reduce ammo
        remainingAmmo--;
        gunUI.UpdateAmmo();
        AddOverheat();

        //Create projectile
        GameObject newProjectile = Instantiate(projectile, shotTransform.position, Quaternion.identity);
        float size = stats.GetStatValue("ShotSize");
        newProjectile.transform.localScale = new Vector3(size, size, size);

        //Add velocity
        Vector3 velocity = stats.GetStatValue("ShotSpeed") * transform.right;
        newProjectile.GetComponent<Projectile>().InstantiateProjectile(velocity, stats.GetStatValue("ShotDamage"), stats.GetStatValue("KnockBack"));
    }
}
