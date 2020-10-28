using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/New weapon")]
public class WeaponObject : ScriptableObject
{
    public string WeaponName;
    public bool oneHanded;
    public bool IsUnlocked;
    [Header("Handling")]
    public float fireRate;
    public bool singleShot;
    [Header("Recoil & Kickback")]
    public float recoilStrength;
    public float recoilRecoverySpeed;
    public float kickbackStrength;
    public float kickbackRecoverySpeed;
    [Header("Aiming")]
    public bool canAim;
    public float aimSpeed;
    [Header("Damage")]
    public float minDamage;
    public float maxDamage;
    [Header("Weapon Spread")]
    public float minSpread;
    public float maxSpread;
    public float spreadAddedPerShot;
    [Tooltip("Spread recovered per second")]
    public float spreadRecoverySpeed;
    [Header("Reload & Ammo")]
    public float reloadTime;
    public AnimationClip reloadAnimation;
    [Tooltip("Number of bullets in magazine")]
    public int magSize;
    [Tooltip("Number of magazines (-1 for infinite)")]
    public int magAmount;
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    [Header("Sound")]
    public AudioClip[] shootAudio;
    public AudioClip[] reloadAudio;
    public AudioClip outOfAmmoAudio;
}
