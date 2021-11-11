using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class UpgradeUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject uiParent;
    [SerializeField]
    private GunDescription descriptionUI;
    [SerializeField]
    private GameObject optionsParent;
    [SerializeField]
    private Image currentGunImage;
    
    
    private BaseWeapon currentGun;
    private GameObject upgradeObjectPrefab;


    private void Awake()
    {
        //Get the Object prefab
        upgradeObjectPrefab = Resources.Load<GameObject>("Prefabs/UI/UpgradeMenu/UpgradeOption");

        //Change music
        MusicManager.Instance.PlaySong("Shop");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Grab currentGun
            WeaponManager manager = collision.GetComponentInChildren<WeaponManager>();
            if (manager != null)
            {
                //if you grabbed a gun, open the UI
                currentGun = manager.CurrentWeapon;

                InstantiateUI();
            }
        }
    }

    public void CloseUI()
    {
        //Disable UI
        uiParent.SetActive(false);

        //Enable player movement
        Player.Instance.SetPlayerEnabled(true);
    }

    private void InstantiateUI()
    {
        //Disable player movement
        Player.Instance.SetPlayerEnabled(false);

        //Set parent enabled to true
        uiParent.SetActive(true);

        //Set current gun sprite
        currentGunImage.sprite = currentGun.GetComponent<BaseWeapon>().gunSprite;

        //Set the current gun for the descriptionUI
        descriptionUI.SetCurrentGun(currentGun);

        //Display current gun in description
        descriptionUI.UpdateDescription(currentGun);

        //Remove all previous upgrade objects
        RemoveUpgradeObjects();

        //Create new upgrade objects
        GenerateUpgradeObjects();
    }

    private void RemoveUpgradeObjects()
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < optionsParent.transform.childCount; i++) { children.Add(optionsParent.transform.GetChild(i).gameObject); }
        children.ForEach(ctx => Destroy(ctx));
    }

    private void GenerateUpgradeObjects()
    {
        foreach(UpgradePath path in currentGun.upgradePaths)
        {
            AddUpgradeObject(path);
        }
    }

    private void AddUpgradeObject(UpgradePath path)
    {
        GunUpgradeOption newUpgrade = Instantiate(upgradeObjectPrefab, optionsParent.transform).GetComponent<GunUpgradeOption>();
        newUpgrade.SetGunDescription(descriptionUI);
        newUpgrade.SetUpgradePath(path);
    }


}
