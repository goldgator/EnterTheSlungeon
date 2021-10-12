using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{

    public WeaponManager weaponManager;

    public TMP_Text ammoText;
    public Image gunImage;
    public Image overheatImage;
    public Slider reloadSlider;

    public float overheatMaxAlpha = 140f;
    public float overheatPulseStrength = 15.0f;
    public float overheatPulseSpeed = 1.2f;
    private float currentPulseTime = 0f;
    public AnimationCurve pulseCurve;

    // Start is called before the first frame update
    void Start()
    {
        UpdateBasics();
    }

    public void UpdateBasics()
    {
        UpdateSprite();
        UpdateAmmo();
    }

    public void UpdateSprite()
    {
        gunImage.sprite = weaponManager.CurrentWeapon.gunSprite;
    }

    public void UpdateReload(float progress)
    {
        reloadSlider.value = 1 - progress;
    }

    public void UpdateAmmo()
    {
        ammoText.text = weaponManager.CurrentWeapon.GetRemainingAmmo() + "/" + weaponManager.CurrentWeapon.GetMaxAmmo();
    }

    public void UpdateOverheat(bool overheated, float overheatProgress)
    {
        float alpha = Mathf.Lerp(0, 140, overheatProgress);

        if (overheated)
        {
            currentPulseTime = (currentPulseTime + Time.deltaTime) % overheatPulseSpeed;

            alpha += pulseCurve.Evaluate(currentPulseTime / overheatPulseSpeed) * overheatPulseStrength;
        }

        Color newColor = overheatImage.color;
        newColor.a = alpha / 255f;
        overheatImage.color = newColor;
    }
}
