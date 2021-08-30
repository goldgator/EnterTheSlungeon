using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponDetails weaponDetails;
    public Transform shotTransform;
    private AudioSource audioSource;

    private float fireRate;
    private float shotDamage;
    private float shotSize;
    private float shotSpeed;
    

    private float fireTimer = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = weaponDetails.gunSound;
    }

    // Start is called before the first frame update
    void Start()
    {
        fireRate = weaponDetails.fireRate;
        shotDamage = weaponDetails.shotDamage;
        shotSize = weaponDetails.shotSize;
        shotSpeed = weaponDetails.shotSpeed;
    }

    private void OnEnable()
    {
        if (InputManager.Instance) InputManager.Instance.fireUpdateEvent += OnFire;
    }

    private void OnDisable()
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

    private void ProcessAim()
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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Determine player and weapon scale
        float aimX = Mathf.Sign(direction.x);
        Player.Instance.transform.localScale = new Vector3(aimX, 1, 1);
        transform.localScale = new Vector3(aimX, aimX, 1); ;
    }

    void OnFire(bool pressed)
    {
        if (fireTimer >= fireRate && pressed)
        {
            fireTimer = 0;

            //Make Sound
            audioSource.Play();

            //Create projectile
            GameObject newProjectile = Instantiate(weaponDetails.projectile, shotTransform.position, Quaternion.identity);
            newProjectile.transform.localScale = new Vector3(shotSize, shotSize, shotSize);

            //Add velocity
            Vector3 velocity = shotSpeed * transform.right;
            newProjectile.GetComponent<Projectile>().InstantiateProjectile(velocity, shotDamage);
        }
    }
}
