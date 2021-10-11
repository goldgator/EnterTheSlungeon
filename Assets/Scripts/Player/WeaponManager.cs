using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public List<Transform> weaponPivots = new List<Transform>();
    private List<BaseWeapon> weapons = new List<BaseWeapon>();

    int currWeapon = 0;

    public Transform CurrentPivot
    {
        get { return weaponPivots[currWeapon]; }
    }

    public BaseWeapon CurrentWeapon
    {
        get { return weapons[currWeapon]; }
    }

    private void OnEnable()
    {
        InputManager.Instance.scrollStartEvent += SwapWeapon;
    }

    private void OnDisable()
    {
        InputManager.Instance.scrollStartEvent -= SwapWeapon;
    }

    private void Start()
    {
        //UpdateInfo();
    }

    public void AddGun(GameObject newGun)
    {
        GameObject newPivot = new GameObject();
        newPivot.name = "Weapon" + weaponPivots.Count;
        newPivot.transform.SetParent(transform, false);
        weaponPivots.Add(newPivot.transform);

        BaseWeapon newWeapon = Instantiate(newGun).GetComponent<BaseWeapon>();
        newWeapon.transform.SetParent(newPivot.transform, false);
        weapons.Add(newWeapon);

        SwapToWeapon(weapons.Count - 1);
    }

    private void UpdateInfo()
    {
        //Currently breaks if any weapons are disabled
        for (int i = 0; i < weaponPivots.Count; i++)
        {
            weapons[i] = weaponPivots[i].GetComponentInChildren<BaseWeapon>();
        }
    }

    private BaseWeapon GetGun(int index)
    {
        if (weapons[index] != null)
        {
            return weapons[index];
        } else
        {
            BaseWeapon weapon = weaponPivots[index].GetComponentInChildren<BaseWeapon>();
            return weapon;
        }
    }

    private void SwapWeapon(bool pressed)
    {
        SwapToWeapon((currWeapon + 1) % weapons.Count);
    }

    private void SwapToWeapon(int index)
    {
        SetGunState(currWeapon, false);
        currWeapon = index;
        SetGunState(currWeapon, true);
    }

    private void SetGunState(int index, bool state)
    {
        //Debug.Log(index);
        weapons[index].gameObject.SetActive(state);
    }


}