using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro; // remove later
using Photon.Pun;

public class Weapon : MonoBehaviourPun
{
    [SerializeField]
    WeaponObject wo;
    [SerializeField]
    GameObject bulletHolePrefab, playerHitFXPrefab;
    [SerializeField]
    Transform cameraHolder, guntip, weaponVisuals, hipPosition, aimPosition, equipPosition;
    [SerializeField]
    AudioSource guntipAudio, gunAudio;

    int currentMag;
    int totalAmmoLeft;
    int maxAmmo;
    
    //AudioSource guntipAudio, gunAudio;
    ParticleSystem muzzleFlash;
    float timeSinceLastShot = 0f;
    float currentSpread;
    Animation anim;

    public bool IsAiming { get; set; } = false;
    public bool IsReloading { get; set; } = false;

    public WeaponObject GetWeaponObject => wo;

    PlayerState state;

    void Start()
    {
        //gunAudio = gunAudioPosition.GetComponent<AudioSource>();
        //guntipAudio = guntip.GetComponent<AudioSource>();
        cameraHolder = transform.root.Find("CameraHolder");
        muzzleFlash = guntip.GetComponent<ParticleSystem>();
        anim = GetComponent<Animation>();
        if (!photonView.IsMine)
            return;

        state = transform.root.gameObject.GetComponent<PlayerState>();

        currentMag = wo.magSize;
        totalAmmoLeft = wo.magSize * wo.magAmount - wo.magSize;
        maxAmmo = wo.magSize * wo.magAmount;

        currentSpread = wo.minSpread;

        Cursor.lockState = CursorLockMode.Locked; // Fix
    }
    
    void Update()
    {
        ResetWeaponTransform();

        if (!photonView.IsMine)
            return;

        Aim();
        SetPlayerStates();

        if (Input.GetKeyDown(KeyCode.R)) // Refactor
            Reload();

        CalculateSpreadValue();

        if (Input.GetKeyDown(KeyCode.Escape)) // FIX
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetPlayerStates()
    {
        state.IsAiming = IsAiming;
        state.IsReloading = IsReloading;
        state.CurrentAmmo = currentMag;
        state.CurrentAmmoReserve = totalAmmoLeft;
    }

    void ResetWeaponTransform()
    {
        weaponVisuals.localRotation = Quaternion.Lerp(weaponVisuals.localRotation, Quaternion.identity, Time.deltaTime * wo.recoilRecoverySpeed);
        weaponVisuals.localPosition = Vector3.Lerp(weaponVisuals.localPosition, new Vector3(weaponVisuals.localPosition.x, weaponVisuals.localPosition.y, 0f), Time.deltaTime * wo.kickbackRecoverySpeed);
    }

    public void Equip()
    {
        StartCoroutine(EquipRoutine());
    }

    IEnumerator EquipRoutine()
    {
        state.CanChangeWeapon = false;
        weaponVisuals.localPosition = equipPosition.localPosition;

        yield return new WaitForSeconds(.4f);

        state.CanChangeWeapon = true;
    }

    #region Shooting
    public void Shoot()
    {
        if (IsReloading)
            return;

        if ((Input.GetMouseButton(0) && !wo.singleShot && wo.fireRate <= timeSinceLastShot) || Input.GetMouseButtonDown(0))
        {
            if (currentMag > 0)
            {
                DoShoot();
                currentMag--;
                timeSinceLastShot = 0;
            }
            else
            {
                if (wo.outOfAmmoAudio != null && ((!wo.singleShot && wo.fireRate * 2 <= timeSinceLastShot) || wo.singleShot))
                {
                    photonView.RPC("OutOfAmmoSound", RpcTarget.All);
                    timeSinceLastShot = 0;                    
                }                    
            }
        }
    }

    void DoShoot()
    {
        var spread = CalculateWeaponSpread();

        photonView.RPC("NetworkShootEffects", RpcTarget.All);

        RaycastHit hit;
        if (Physics.Raycast(cameraHolder.position, spread, out hit, 100f, 9))
        {

            photonView.RPC("NetworkHitEffects", RpcTarget.All, hit.transform.tag == "Player", hit.point, hit.normal);
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
        
        bool hasAnim = wo.reloadAnimation != null;
        float wait = hasAnim ? wo.reloadAnimation.length : wo.reloadTime;

        StartCoroutine(ReloadAnim(wait, hasAnim));
    }

    IEnumerator ReloadAnim(float wait, bool hasAnimation)
    {
        if (IsAiming)
        {
            IsReloading = true;
            yield return new WaitForSeconds(.2f);
        }

        IsReloading = true;

        if (anim != null && hasAnimation)
        {
            anim.clip = wo.reloadAnimation;
            anim.Play();
        }
        totalAmmoLeft += currentMag;

        yield return new WaitForSeconds(wait);

        currentMag = 0;
        currentMag += totalAmmoLeft - wo.magSize >= 0 ? wo.magSize : totalAmmoLeft;
        totalAmmoLeft -= currentMag;
        IsReloading = false;
    }

    void Aim()
    {
        if (!wo.canAim)
            return;

        if (Input.GetMouseButton(1) && !IsReloading)
        {
            IsAiming = true; // REFACTOR MAYBE
            weaponVisuals.position = Vector3.Lerp(weaponVisuals.position, aimPosition.position, Time.deltaTime * wo.aimSpeed);
        }
        else
        {
            IsAiming = false;
            weaponVisuals.position = Vector3.Lerp(weaponVisuals.position, hipPosition.position, Time.deltaTime * wo.aimSpeed);
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
        muzzleFlash.Play();

        weaponVisuals.Rotate(Mathf.Clamp(-wo.recoilStrength, -wo.recoilStrength * 2, 0), 0, 0);
        weaponVisuals.localPosition -= Vector3.forward * wo.kickbackStrength;
    }

    [PunRPC]
    void NetworkHitEffects(bool hitPlayer, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!hitPlayer)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hitPoint + hitNormal * 0.001f, Quaternion.identity);
            bulletHole.transform.LookAt(hitPoint + hitNormal);
        }
        else
        {
            GameObject bulletHole = Instantiate(playerHitFXPrefab, hitPoint + hitNormal * 0.001f, Quaternion.identity);
            bulletHole.transform.LookAt(hitPoint + hitNormal);
        }
    }

    #endregion
}
