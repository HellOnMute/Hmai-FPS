using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro; // remove later
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField]
    WeaponObject wo;
    [SerializeField]
    GameObject bulletHolePrefab;
    [SerializeField]
    Transform cameraHolder, guntip, gunAudioPosition;

    int currentMag;
    int totalAmmoLeft;
    int maxAmmo;

    bool isEquipped = false;
    bool isEquipping = false;
    bool canShoot = false;

    Transform weaponTransform, hipTransform, unequippedTransform;
    
    AudioSource guntipAudio, gunAudio;
    float timeSinceLastShot = 0f;
    float currentSpread;

    void Start()
    {
        gunAudio = gunAudioPosition.GetComponent<AudioSource>();
        guntipAudio = guntip.GetComponent<AudioSource>();
        cameraHolder = transform.root.Find("CameraHolder");

        if (!photonView.IsMine)
            return;

        currentMag = wo.magSize;
        totalAmmoLeft = wo.magSize * wo.magAmount - wo.magSize;
        maxAmmo = wo.magSize * wo.magAmount;

        weaponTransform = gameObject.transform.Find("Weapon");
        hipTransform = gameObject.transform.Find("Hip");
        unequippedTransform = gameObject.transform.Find("Unequipped");

        currentSpread = wo.minSpread;

        Equip();

        Cursor.lockState = CursorLockMode.Locked; // Fix
    }
    
    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (isEquipping)
            EquipWeapon();
        if (!isEquipped)
            return;

        if (canShoot)
            Shooting();

        Aim();

        if (Input.GetKeyDown(KeyCode.R)) // Refactor
            Reload();

        CalculateSpreadValue();

        if (Input.GetKeyDown(KeyCode.Y)) // Remove later
        {
            var b = GameObject.FindGameObjectsWithTag("Respawn");
            foreach (var item in b)
            {
                Destroy(item);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // FIX
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
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
                    photonView.RPC("OutOfAmmoSound", RpcTarget.All);
            }
        }
    }

    void Shoot()
    {
        timeSinceLastShot = 0f;
        var spread = CalculateWeaponSpread();

        photonView.RPC("NetworkShootEffects", RpcTarget.All);

        RaycastHit hit;
        if (Physics.Raycast(cameraHolder.position, spread, out hit, 100f))
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
            bulletHole.transform.LookAt(hit.point + hit.normal);
        }
    }

    void CalculateSpreadValue()
    {
        currentSpread = Mathf.MoveTowards(currentSpread, wo.minSpread, wo.spreadRecoverySpeed * Time.deltaTime);

        timeSinceLastShot += Time.deltaTime;
    }

    Vector3 CalculateWeaponSpread()
    {
        var spread = cameraHolder.position + cameraHolder.forward * 100f;
        spread += Random.Range(-currentSpread, currentSpread) * Vector3.right;
        spread += Random.Range(-currentSpread, currentSpread) * Vector3.up;
        spread -= cameraHolder.position;
        spread.Normalize();

        currentSpread = Mathf.Clamp(currentSpread + wo.spreadAddedPerShot, wo.minSpread, wo.maxSpread);

        return spread;
    }
    #endregion

    void Reload()
    {
        if (totalAmmoLeft <= 0)
            return;
        totalAmmoLeft += currentMag;

        currentMag = 0;
        currentMag += totalAmmoLeft - wo.magSize >= 0 ? wo.magSize : totalAmmoLeft;
        totalAmmoLeft -= currentMag;
    }

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

    #region PunRPC
    [PunRPC]
    void OutOfAmmoSound()
    {
        gunAudio.clip = wo.outOfAmmoAudio;
        gunAudio.Play();
    }

    [PunRPC]
    void NetworkShootEffects()
    {
        int clip = UnityEngine.Random.Range(0, wo.shootAudio.Length - 1);
        guntipAudio.clip = wo.shootAudio[clip];
        guntipAudio.Play();

        // Particle effect, muzzleflash
    }

    #endregion
}
