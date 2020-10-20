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
    TextMeshProUGUI txt; // Remove later
    [SerializeField]
    WeaponObject wo;
    [SerializeField]
    GameObject bulletHolePrefab;
    [SerializeField]
    Transform playerCamera;

    int currentMag;
    int totalAmmoLeft;
    int maxAmmo;

    bool isEquipped = false;
    bool isEquipping = false;
    bool canShoot = false;

    Transform weaponTransform, hipTransform, unequippedTransform;
    
    AudioSource audio;

    float timeSinceLastShot = 0f;
    float currentSpread;

    void Start()
    {
        if (!photonView.IsMine)
            return;

        currentMag = wo.magSize;
        totalAmmoLeft = wo.magSize * wo.magAmount - wo.magSize;
        maxAmmo = wo.magSize * wo.magAmount;

        weaponTransform = gameObject.transform.Find("Weapon");
        hipTransform = gameObject.transform.Find("Hip");
        unequippedTransform = gameObject.transform.Find("Unequipped");

        audio = GetComponent<AudioSource>();
        //playerCamera = GameObject.FindGameObjectWithTag("PlayerEyes").transform; // Refactor?

        currentSpread = wo.minSpread;

        Equip();
    }
    float timez = 0;
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            var b = GameObject.FindGameObjectsWithTag("Respawn");
            foreach (var item in b)
            {
                Destroy(item);
            }
        }
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
                photonView.RPC("Shoot", RpcTarget.All);
                currentMag--;
            }
            else
            {
                if (wo.outOfAmmoAudio != null)
                    audio.PlayOneShot(wo.outOfAmmoAudio);
            }
        }
    }

    [PunRPC]
    void Shoot()
    {
        int clip = UnityEngine.Random.Range(0, wo.shootAudio.Length - 1);
        audio.PlayOneShot(wo.shootAudio[clip]);

        var spread = CalculateWeaponSpread();
        timeSinceLastShot = 0f;
        currentSpread = Mathf.Clamp(currentSpread + wo.spreadAddedPerShot, wo.minSpread, wo.maxSpread);

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, spread, out hit, 100f))
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
            bulletHole.transform.LookAt(hit.point + hit.normal);
            //Destroy(bulletHole, 8f);

            if (photonView.IsMine)
            {
                // if we hit a player...
                // rpc call damage to that player!
            }
        }
    }

    void CalculateSpreadValue()
    {
        currentSpread = Mathf.MoveTowards(currentSpread, wo.minSpread, wo.spreadRecoverySpeed * Time.deltaTime);

        timeSinceLastShot += Time.deltaTime;
    }

    Vector3 CalculateWeaponSpread()
    {
        //var currentSpread = Mathf.Clamp()
        var spread = playerCamera.position + playerCamera.forward * 100f;
        spread += Random.Range(-currentSpread, currentSpread) * Vector3.right;
        spread += Random.Range(-currentSpread, currentSpread) * Vector3.up;
        spread -= playerCamera.position;
        spread.Normalize();

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
}
