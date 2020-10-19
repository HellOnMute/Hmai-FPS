using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    WeaponObject wo;

    int currentMag;
    int totalAmmo;

    bool isEquipped = false;
    bool isEquipping = false;
    bool canShoot = false;

    Transform weaponTransform, hipTransform, unequippedTransform;

    AudioSource audio;

    void Start()
    {
        currentMag = wo.magSize;
        totalAmmo = wo.magSize * wo.magAmount;

        weaponTransform = gameObject.transform.Find("Weapon");
        hipTransform = gameObject.transform.Find("Hip");
        unequippedTransform = gameObject.transform.Find("Unequipped");

        audio = GetComponent<AudioSource>();

        Equip();
    }
    
    void Update()
    {
        if (isEquipping)
            EquipWeapon();
        if (!isEquipped)
            return;

        if (canShoot)
            Shooting();

        Aim();
    }

    #region Equip
    public void Equip()
    {
        isEquipping = true;
        canShoot = false;
        weaponTransform.localRotation = unequippedTransform.localRotation;
    }

    public void Unequip()
    {
        isEquipped = false;
        canShoot = false;
    }

    private void EquipWeapon()
    {
        weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, hipTransform.localRotation, Time.deltaTime / 0.05f);
        if (weaponTransform.localRotation == hipTransform.localRotation)
        {
            canShoot = true;
            isEquipped = true;
        }
    }
    #endregion

    #region Shooting
    void Shooting()
    {
        if ((Input.GetMouseButton(0) && !wo.singleShot) || Input.GetMouseButtonDown(0))
        {
            if (currentMag > 0)
            {
                Shoot();
                currentMag--;
            }
            else
            {
                if (wo.outOfAmmoAudio != null)
                    audio.PlayOneShot(wo.outOfAmmoAudio);
            }
        }
    }

    void Shoot()
    {
        int clip = UnityEngine.Random.Range(0, wo.shootAudio.Length);
        audio.PlayOneShot(wo.shootAudio[clip]);
    }
    #endregion

    void Aim()
    {
        if (!wo.canAim)
            return;
        
        Transform aim = gameObject.transform.Find("Aim");

        if (Input.GetMouseButton(1))
        {
            weaponTransform.position = Vector3.Lerp(weaponTransform.position, aim.position, Time.deltaTime / wo.aimSpeed);
            // Add camera for sights?
        }
        else
        {
            weaponTransform.position = Vector3.Lerp(weaponTransform.position, hipTransform.position, Time.deltaTime / wo.aimSpeed);
        }
    }
}
