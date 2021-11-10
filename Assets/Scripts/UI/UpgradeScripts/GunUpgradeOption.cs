using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GunUpgradeOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GunDescription descriptionUI;
    private Image bgImage;
    [SerializeField]
    private Image gunImage;
    private UpgradePath upgradePath;

    private UpgradeUIManager upgradeUIManager;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        upgradeUIManager = GetComponentInParent<UpgradeUIManager>();
    }

    public void UpdateImage()
    {
        gunImage.sprite = upgradePath.upgradedGun.GetComponent<BaseWeapon>().gunSprite;
    }

    public void SetGunDescription(GunDescription newDescription)
    {
        descriptionUI = newDescription;
    }

    public void SetUpgradePath(UpgradePath path)
    {
        upgradePath = path;
        UpdateImage();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionUI.UpdateDescription(upgradePath);
        bgImage.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bgImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Check if player can afford it
        Wallet playerWallet = Player.Instance.GetWallet();
        if (!playerWallet.CanAfford(upgradePath.price)) return;

        //Take cost out
        playerWallet.RemoveQuartz(upgradePath.price);

        //Replace the weapon
        Player.Instance.GetComponentInChildren<WeaponManager>().ReplaceCurrentWeapon(upgradePath.upgradedGun);

        //Turn off UI
        upgradeUIManager.CloseUI();
    }
}
