using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseItem : MonoBehaviour, IInteractEvent
{

    [Header("Base Info")]
    [SerializeField]
    protected Sprite itemSprite;
    [SerializeField]
    protected string name;
    [SerializeField]
    private string desc;

    protected bool pickedUp = false;

    private const string ITEM_PATH = "Prefabs/Items/";

    protected ItemManager managerParent;
    protected Interactable interactable;
    protected SpriteRenderer renderer;
    protected Collider2D collider;

    public virtual void SetupItem()
    {
        //Destroy interactable component *before* as it has dependencies with the renderer and collider
        Destroy(interactable);
        Destroy(renderer);
        Destroy(collider);
        transform.SetParent(managerParent.transform, false);
        transform.localPosition = Vector3.zero;
    }

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        if (renderer == null) renderer = gameObject.AddComponent<SpriteRenderer>();

        renderer.sprite = itemSprite;
    }

    public static int RandomRarity()
    {
        float[] rarityPercentages = new float[] { .4f, .3f, .2f, .1f };
        float percentSum = 0;
        foreach (float f in rarityPercentages) percentSum += f;

        //Generate a random value between 0 and the sum of all percentages
        float itemRoll = RNGManager.GetItemRand(0f, percentSum);

        int rarityNum = 0;
        //Find what rarity it rolled
        for (int i = 0; i < rarityPercentages.Length; i++)
        {
            float minPercent = 0;
            for (int j = 0; j < i; j++) minPercent += rarityPercentages[j];

            float maxPercent = minPercent + rarityPercentages[i];

            //Test if between these percents, set rarityNum to i if so and break
            if (itemRoll >= minPercent && itemRoll <= maxPercent)
            {
                rarityNum = i;
                break;
            }
        }

        return rarityNum;
    }

    //R0 40%, R1 30%, R2 20%, R3 10%
    public static GameObject SelectRandomItem()
    {
        GameObject[] items;

        //See if we're testing items
        if (Floor.Instance.itemTesting)
        {
            items = Resources.LoadAll<GameObject>(ITEM_PATH + "RTest");
            return items[RNGManager.GetItemRand(0, items.Length)];
        }

        //Returns 0-3 based on rarity
        int rarity = BaseItem.RandomRarity();

        //Find path to corresponding rarity folder
        string rarityFolder = ITEM_PATH + "R" + rarity;

        //Get all items within the folder
        items = Resources.LoadAll<GameObject>(rarityFolder);

        //Choose random item
        return items[RNGManager.GetItemRand(0, items.Length)];
    }

    public static GameObject SelectRandomItem(int rarity)
    {
        GameObject[] items;

        //See if we're testing items
        if (Floor.ItemTesting)
        {
            items = Resources.LoadAll<GameObject>(ITEM_PATH + "RTest");
            return items[RNGManager.GetItemRand(0, items.Length)];
        }

        //Find path to corresponding rarity folder
        string rarityFolder = ITEM_PATH + "R" + rarity;

        //Get all items within the folder
        items = Resources.LoadAll<GameObject>(rarityFolder);

        //Choose random item
        return items[RNGManager.GetItemRand(0, items.Length)];
    }

    public void PickUp()
    {
        managerParent = Player.Instance.GetComponentInChildren<ItemManager>();
        managerParent?.AddItem(this);
    }

    public void OnInteract()
    {
        PickUp();
    }

    public void OnInteractEnter()
    {
        MessageManager.Instance.SendPopup(name, desc, float.MaxValue, itemSprite);
    }

    public void OnInteractLeave()
    {
        MessageManager.Instance.ClosePopup();
    }
}
