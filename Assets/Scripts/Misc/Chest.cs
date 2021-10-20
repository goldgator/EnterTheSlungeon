using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Chest : MonoBehaviour
{
    private const string ITEM_PATH = "Prefabs/Items/";
    [SerializeField]
    private Transform spawnTransform;
    private GameObject spawnedItem;
    private Collider2D collider;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        SelectItem();
    }

    //R0 40%, R1 30%, R2 20%, R3 10%
    private void SelectItem()
    {
        //Returns 0-3 based on rarity
        int rarity = SelectRarity();

        //Find path to corresponding rarity folder
        string rarityFolder = ITEM_PATH + "R" + rarity;

        //Get all items within the folder
        GameObject[] items = Resources.LoadAll<GameObject>(rarityFolder);

        //Choose random item
        spawnedItem = items[RNGManager.GetItemRand(0, items.Length)];
    }

    private int SelectRarity()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.Play("Open");
            collider.enabled = false;
        }
    }

    public void SpawnItem()
    {
        Instantiate(spawnedItem, spawnTransform.position, Quaternion.identity);
    }
}
