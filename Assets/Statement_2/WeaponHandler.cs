using System;
using UnityEngine;

public class WeaponHandler
{
    [SerializeField] private Weapon CurrentWeapon;
    private Weapon PrimaryWeapon1;
    private Weapon PrimaryWeapon2;
    private Weapon SecondaryWeapon;

    public Action<Weapon, Weapon, Weapon> OnWeaponEquipped; // Update all three weapons....
    public Action<Weapon> OnWeaponSwitched; // Update current weapon....

    public void OnInitialize()
    {
        PrimaryWeapon1 = null;
        PrimaryWeapon2 = null;
        SecondaryWeapon = null;
        CurrentWeapon = null;
        OnWeaponEquipped?.Invoke(null, null, null);
    }

    public void EquipWeapon(WeaponSlot slot, Weapon weapon)
    {
        if (weapon.GetWeaponType() == WeaponType.PrimaryGuns)
        {
            if (PrimaryWeapon1 == null)
            {
                PrimaryWeapon1 = weapon;
            }
            else if (PrimaryWeapon2 == null)
            {
                PrimaryWeapon2 = weapon;
            }
            else
            {
                Debug.Log("Both primary weapon slots are full.");
                switch (slot)
                {
                    case WeaponSlot.PrimarySlot1:
                        PrimaryWeapon1 = weapon;
                        break;
                    case WeaponSlot.PrimarySlot2:
                        PrimaryWeapon2 = weapon;
                        break;
                    default:
                        Debug.Log("Invalid slot for primary weapon.");
                        break;
                }
            }
        }
        else
        {
            if (SecondaryWeapon == null)
            {
                SecondaryWeapon = weapon;
            }
            else
            {
                Debug.Log("Secondary weapon slot is full. Replacing existing weapon.");
                SecondaryWeapon = weapon;
            }
        }
        OnWeaponEquipped?.Invoke(PrimaryWeapon1, PrimaryWeapon2, SecondaryWeapon);
    }

    public void SwitchWeapon(WeaponSlot slot)
    {
        switch (slot)
        {
            case WeaponSlot.PrimarySlot1:
                if (PrimaryWeapon1 != null)
                {
                    CurrentWeapon = PrimaryWeapon1;
                }
                else
                {
                    Debug.Log("No weapon equipped in Primary Slot 1.");
                }
                break;
            case WeaponSlot.PrimarySlot2:
                if (PrimaryWeapon2 != null)
                {
                    CurrentWeapon = PrimaryWeapon2;
                }
                else
                {
                    Debug.Log("No weapon equipped in Primary Slot 2.");
                }
                break;
            case WeaponSlot.SecondarySlot:
                if (SecondaryWeapon != null)
                {
                    CurrentWeapon = SecondaryWeapon;
                }
                else
                {
                    Debug.Log("No weapon equipped in Secondary Slot.");
                }
                break;
            default:
                Debug.Log("Invalid weapon slot.");
                break;
        }
        Debug.Log($"Current WeaponData => {JsonUtility.ToJson(CurrentWeapon.GetWeaponData())} ==> {CurrentWeapon.GetWeaponType()}");
        OnWeaponSwitched?.Invoke(CurrentWeapon);
    }
}