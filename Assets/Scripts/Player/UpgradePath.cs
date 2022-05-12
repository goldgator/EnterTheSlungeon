using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UpgradePath
{
    public GameObject upgradedGun;
    public Wallet price;

    public UpgradePath(GameObject gunPrefab)
    {
        upgradedGun = gunPrefab;
        price = new Wallet(0,0,0);
    }

    public UpgradePath(BaseWeapon weapon)
    {
        upgradedGun = weapon.gameObject;
        price = new Wallet(0,0,0);
    }
}
