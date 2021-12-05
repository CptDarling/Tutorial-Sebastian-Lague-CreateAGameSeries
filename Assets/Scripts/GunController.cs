using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    [SerializeField] Transform weaponHold;
    [SerializeField] Gun startingGun;

    Gun equippedGun;

    private void Start()
    {
        if (startingGun != null)
            EquipGun(startingGun);
    }

    internal void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null) Destroy(equippedGun.gameObject);
        //equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun = Instantiate(gunToEquip, weaponHold) as Gun;
    }

    internal void Shoot()
    {
        if (equippedGun != null)
            equippedGun.Shoot();
    }
}
