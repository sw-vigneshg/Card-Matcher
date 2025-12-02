using System;
using UnityEngine;

public class WeaponData
{
    public string Name;
    public int AmmoInMag;
    public int TotalAmmo;
    public int MagSize;

    public void ResetWeaponData()
    {
        AmmoInMag = MagSize;
        TotalAmmo = MagSize * 3;
    }

    public void AddAmmo(int amount)
    {
        TotalAmmo += amount;
    }
}

public class Weapon
{
    [SerializeField] private WeaponData WeaponData;
    [SerializeField] private WeaponType WeaponType;
    [SerializeField] private float FireRate;
    [SerializeField] private float ReloadTime;
    [SerializeField] private float Damage = 10f;
    private float LastFireTime = 0f;

    public WeaponData GetWeaponData()
    {
        return WeaponData;
    }

    public WeaponType GetWeaponType()
    {
        return WeaponType;
    }

    public bool CanFire() => (WeaponData.AmmoInMag > 0 && Time.time >= LastFireTime + FireRate);

    public void Fire()
    {
        if (WeaponData.AmmoInMag <= 0 || WeaponData.TotalAmmo <= 0)
        {
            Debug.Log("No Ammo in Magazine. Please Reload....");
            return;
        }

        WeaponData.AmmoInMag -= 1;
        LastFireTime = Time.time;
    }

    public void Reload()
    {
        if (WeaponData.AmmoInMag >= WeaponData.MagSize)
        {
            Debug.Log("Magazine is already full.");
            return;
        }

        if (WeaponData.TotalAmmo <= 0)
        {
            Debug.Log("No Ammo found......Please find Ammo");
            return;
        }

        // One way of reload...
        int ammoNeeded = WeaponData.MagSize - WeaponData.AmmoInMag;
        if (WeaponData.TotalAmmo >= ammoNeeded)
        {
            WeaponData.AmmoInMag += ammoNeeded;
            WeaponData.TotalAmmo -= ammoNeeded;
        }
        else
        {
            WeaponData.AmmoInMag += WeaponData.TotalAmmo;
            WeaponData.TotalAmmo = 0;
        }

        // Another way of reload...
        int requiredAmmo = WeaponData.MagSize - WeaponData.AmmoInMag;
        int takenAmmo = Math.Min(requiredAmmo, WeaponData.TotalAmmo);
        WeaponData.AmmoInMag += takenAmmo;
        WeaponData.TotalAmmo -= takenAmmo;
    }
}