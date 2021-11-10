using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunDescription : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Image gunImage;
    [SerializeField]
    private TMP_Text gunName;

    [SerializeField]
    private TMP_Text statChanges;

    [SerializeField]
    private TMP_Text voidQuartzText;
    [SerializeField]
    private TMP_Text timeQuartzText;
    [SerializeField]
    private TMP_Text spaceQuartzText;

    private BaseWeapon currentWeapon;
    

    public void UpdateDescription(UpgradePath path)
    {
        BaseWeapon upgradedGun = path.upgradedGun.GetComponent<BaseWeapon>();

        //Update Image and Name
        UpdateBasicInfo(upgradedGun);

        //Update price
        UpdatePrice(path.price);

        //Update Stat differences
        statChanges.text = GetStatDifferences(upgradedGun);
    }

    public void SetCurrentGun(BaseWeapon newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void UpdateDescription(BaseWeapon gunPrefab)
    {
        UpdateDescription(new UpgradePath(gunPrefab));
    }

    private void UpdateBasicInfo(BaseWeapon weapon)
    {
        gunImage.sprite = weapon.gunSprite;
        gunName.text = weapon.gunName;
    }

    private void UpdatePrice(Wallet price)
    {
        voidQuartzText.text = price.VoidQuartz + "";
        timeQuartzText.text = price.TimeQuartz + "";
        spaceQuartzText.text = price.SpaceQuartz + "";

        //Update colors
        Wallet playerWallet = Player.Instance.GetWallet();
        voidQuartzText.color = (playerWallet.VoidQuartz < price.VoidQuartz) ? Color.red : Color.white;
        timeQuartzText.color = (playerWallet.TimeQuartz < price.TimeQuartz) ? Color.red : Color.white;
        spaceQuartzText.color = (playerWallet.SpaceQuartz < price.SpaceQuartz) ? Color.red : Color.white;
    }

    private string GetStatDifferences(BaseWeapon upgradedGun)
    {
        StatBlock currentGunStats = currentWeapon.GetStatBlock();
        StatBlock upgradedGunStats = upgradedGun.GetStatBlock();

        List<string> statDiffs = currentGunStats.FindStatDiffs(upgradedGunStats);

        string statConcat = "";

        foreach (string statName in statDiffs)
        {
            statConcat += $"{statName}: ({currentGunStats.GetStatValue(statName)} -> {upgradedGunStats.GetStatValue(statName)})\n";
        }

        return statConcat;
    }
}
