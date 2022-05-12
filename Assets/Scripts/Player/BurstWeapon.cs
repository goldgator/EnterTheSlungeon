using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstWeapon : BaseWeapon
{

    protected override void Fire()
    {
        int burstNum = (int) stats.GetStatValue("BurstNum");
        float burstGap = stats.GetStatValue("BurstGap");

        fireTimer = -((burstNum - 1) * burstGap);
        StartCoroutine(BurstFire(burstNum, burstGap));
    }

    private IEnumerator BurstFire(int fireNum, float burstGap)
    {
        for (int i = 0; i < fireNum; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(burstGap);
        }
    }

    public override void CancelReload()
    {
        if (isReloading) StopAllCoroutines();

        base.CancelReload();
    }
}
