using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails", menuName = "Custom/WeaponDetails")]
public class WeaponDetails : ScriptableObject
{
    [Header("WeaponStats")]
    public float fireRate;
    public float shotDamage;
    public float shotSize;


    [Header("Components")]
    public Sprite gunSprite;
    public GameObject projectile;
}
