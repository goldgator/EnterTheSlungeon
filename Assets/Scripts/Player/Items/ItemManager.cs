using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    public delegate void ItemUpdate();
    public event ItemUpdate itemUpdates;

    
    private List<BaseItem> items = new List<BaseItem>();
    [SerializeField]
    private List<StatBlock> targetStatBlocks = new List<StatBlock>();

    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void AddTargetStatBlock(StatBlock newBlock)
    {
        targetStatBlocks.Add(newBlock);
        ApplyPassiveMods();
    }

    public void ApplyPassiveMods()
    {
        foreach (BaseItem item in items)
        {
            if (item is PassiveItem)
            {
                PassiveItem passiveItem = (PassiveItem)item;
                passiveItem.ApplyMods();
            }
        }
    }

    public void RemoveTargetStatBlock(StatBlock oldBlock)
    {
        targetStatBlocks.Remove(oldBlock);
    }

    public void AttachMod(StatModifier modifier)
    {
        foreach (StatBlock block in targetStatBlocks)
        {
            block.AddModifier(modifier);
        }
    }

    public void RemoveMod(StatModifier modifier)
    {
        foreach (StatBlock block in targetStatBlocks)
        {
            block.RemoveModifier(modifier);
        }
    }

    public void AddItem(BaseItem item)
    {
        item.SetupItem();
        items.Add(item);
    }

    // Update is called once per frame
    void Update()
    {
        itemUpdates?.Invoke();
    }
}
