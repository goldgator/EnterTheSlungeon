using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeWeapon : BaseWeapon
{
    public bool canReleaseEarly = true;
    private float chargeTime = 0f;
    [SerializeField]
    private float shakeStrength = 0.05f;
    [SerializeField]
    private GameObject chargingAnim;

    private Vector3 lastOffset = new Vector3();
    private Vector3 originalOffset = new Vector3();

    private void Start()
    {
        originalOffset = transform.localPosition;
    }

    protected override void StartControls()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.fireStartEvent += TryReload;
            InputManager.Instance.fireUpdateEvent += OnFire;
            InputManager.Instance.reloadStartEvent += OnReload;
            InputManager.Instance.fireStopEvent += FireRelease;
        }
    }

    protected override void DisableControls()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.fireStartEvent -= TryReload;
            InputManager.Instance.fireUpdateEvent -= OnFire;
            InputManager.Instance.reloadStartEvent -= OnReload;
            InputManager.Instance.fireStopEvent -= FireRelease;
        }
    }

    

    protected override void Fire()
    {
        chargeTime += Time.deltaTime;

        //Add sparkle effect if finished charging
        float maxChargeTime = stats.GetStatValue("ChargeTime");
        chargingAnim.SetActive(chargeTime >= maxChargeTime);

        //Shake anim
        //Set local pos back to original position
        transform.localPosition = originalOffset;

        //Get random Vector2
        float randAngle = RNGManager.GetEventRand(0, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));

        //Get currentStrength
        float currentStrength = Mathf.Lerp(0, shakeStrength, chargeTime / maxChargeTime);

        lastOffset = new Vector3(direction.x * currentStrength, direction.y * currentStrength);
        transform.localPosition += lastOffset;
    }

    private void FireRelease()
    {
        if (chargeTime == 0) return;
        float maxChargeTime = stats.GetStatValue("ChargeTime");

        //Redo last offset to put back to normal pos
        transform.localPosition -= lastOffset;
        chargingAnim.SetActive(false);

        if (!canReleaseEarly)
        {
            if (chargeTime < maxChargeTime) return;
        }


        SpawnProjectile();

        fireTimer = 0f;
        chargeTime = 0f;
    }

    protected override void SpawnProjectile()
    {
        float chargeRatio = chargeTime / stats.GetStatValue("ChargeTime");

        float damageMult = Mathf.Lerp(1, stats.GetStatValue("DamageMultiplier"), chargeRatio);
        float sizeMult = Mathf.Lerp(1, stats.GetStatValue("SizeMultiplier"), chargeRatio);
        float knockbackMult = Mathf.Lerp(1, stats.GetStatValue("KnockbackMultiplier"), chargeRatio);

        //Call ProcessAim once more to ensure scales and directions have been updated correctly before shot
        ProcessAim();

        //Make Sound
        audioSource.Play();

        //reduce ammo
        remainingAmmo--;
        gunUI.UpdateAmmo();
        AddOverheat();

        //Create projectile
        GameObject newProjectile = Instantiate(projectile, shotTransform.position, Quaternion.identity);
        float size = stats.GetStatValue("ShotSize") * sizeMult;
        newProjectile.transform.localScale = new Vector3(size, size, size);

        //Add velocity
        Vector3 velocity = stats.GetStatValue("ShotSpeed") * transform.right;
        newProjectile.GetComponent<Projectile>().InstantiateProjectile(velocity, stats.GetStatValue("ShotDamage") * damageMult, stats.GetStatValue("KnockBack") * knockbackMult);
    }

}
