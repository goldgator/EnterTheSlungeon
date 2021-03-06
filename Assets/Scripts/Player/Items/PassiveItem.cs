using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : BaseItem
{
    [Header("StatMods")]
    public List<StatModifier> statMods = new List<StatModifier>();

    public override void SetupItem()
    {
        base.SetupItem();

        ApplyMods();
    }

    public void ApplyMods()
    {
        foreach (StatModifier mod in statMods)
        {
            managerParent.AttachMod(mod);
        }
    }
}
