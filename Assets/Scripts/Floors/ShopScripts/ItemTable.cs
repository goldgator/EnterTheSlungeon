using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTable : MonoBehaviour
{
    private bool itemPickedUp = false;

    [Header("Components")]
    [SerializeField]
    private Transform itemSpawnTransform;
    [SerializeField]
    private GameObject priceParent;
    [SerializeField]
    private Image quartzImage;
    [SerializeField]
    private TMP_Text quartzCostText;

    [Header("Details")]
    [SerializeField]
    private ResourceType quartzType;
    [SerializeField]
    private bool randomizeQuartzType;
    private int quartzCost;

    private BaseItem spawnedItem;
    private Interactable itemInteractable;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateItem();
        InstantiateUI();
    }

    private void InstantiateItem()
    {
        if (randomizeQuartzType) quartzType = (ResourceType)RNGManager.GetEventRand(0, 3);

        //Determine spawned Item rarity
        int itemRarity = BaseItem.RandomRarity();

        //Determine cost of this rarity
        quartzCost = (itemRarity * 10) + 20;

        //Get a randomized item from rarity
        spawnedItem = Instantiate(BaseItem.SelectRandomItem(itemRarity)).GetComponent<BaseItem>();
        spawnedItem.transform.SetParent(itemSpawnTransform, false);
        itemInteractable = spawnedItem.GetComponent<Interactable>();
    }

    private void InstantiateUI()
    {
        quartzImage.sprite = Quartz.GetQuartzSprite(quartzType);
        quartzCostText.text = quartzCost + "";
    }

    // Update is called once per frame
    void Update()
    {
        if (!itemPickedUp)
        {
            if (itemInteractable == null)
            {
                itemPickedUp = true;
                itemSpawnTransform.gameObject.SetActive(false);
                Player.Instance.GetWallet().RemoveQuartz(quartzType, quartzCost);
                return;
            }

            bool locked = (Player.Instance.GetWallet().QuartzAmount(quartzType) < quartzCost);
            itemInteractable.SetLocked(locked);
        }
    }
}
