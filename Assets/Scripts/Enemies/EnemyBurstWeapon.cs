using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBurstWeapon : EnemyWeapon
{
    [SerializeField]
    private int burstNum = 3;
    [SerializeField]
    private float burstGap = .1f;

    public override void Shoot()
    {
        shotTimer = -((burstNum - 1) * burstGap);
        StartCoroutine(ShootBurst());
    }

    private IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstNum; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(burstGap);
        }
    }
}
