using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { AssualtRife=0, Pistol, HandGrenade}
[System.Serializable]
public struct WeaponSetting
{
    public WeaponName weaponName;
    public int damage;
    public int CurrentMagazine;
    public int maxMagazine;
    public int currentAmmo;
    public int maxAmmo;
    public float fireRate;
    public float fireDistance;
    public bool isAutomaticFire;
}
