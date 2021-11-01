using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicNecklace : BaseItem
{
    [Header("StatMods")]
    public List<StatModifier> statMods = new List<StatModifier>();

    private float healthRatioThreshold = .25f;
    private float cooldown = 20f;
    private float abilityLength = 10f;
    private bool available = true;

    public override void SetupItem()
    {
        base.SetupItem();

        //Subscribe to player health event
        Player.playerHealthEvent += OnPlayerDamage;
    }

    private void OnPlayerDamage(float healthRatio)
    {
        if (healthRatio <= healthRatioThreshold && available)
        {
            available = false;
            //Start Coroutine
            StartCoroutine(ActivateMods());
        }
    }

    private IEnumerator ActivateMods()
    {
        foreach (StatModifier mod in statMods) managerParent.AttachMod(mod);
        //Tint player sprite to red
        Player.Instance.TintPlayer(new Color(.8f,.2f,.2f));

        yield return new WaitForSeconds(abilityLength);

        foreach (StatModifier mod in statMods) managerParent.RemoveMod(mod);
        //Remove tint
        Player.Instance.TintPlayer(Color.white);

        //Start Cooldown Coroutine
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(cooldown);

        available = true;
    }
}
