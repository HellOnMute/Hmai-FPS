using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviourPun
{
    [SerializeField] List<GameObject> weapons;

    List<GameObject> unlockedWeapons;
    GameObject currentWeapon;
    Weapon currentWeaponScript;
    int currentWeaponIndex = 0;

    private void Start()
    {
        unlockedWeapons = GetUnlockedWeapons();
        photonView.RPC("SetCurrentWeapon", RpcTarget.All, currentWeaponIndex);
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        SwitchWeapon();

        // If can shoot state
        currentWeaponScript.Shoot();
    }

    List<GameObject> GetUnlockedWeapons()
    {
        List<GameObject> unlocked = new List<GameObject>();
        foreach (var item in weapons)
	    {
            if (item.GetComponent<Weapon>().GetWeaponObject.IsUnlocked)
                unlocked.Add(item);
	    }

        if (unlocked.Count == 0)
            throw new UnityException("No weapons marked as unlocked");

        return unlocked;
    }

    void UnlockWeapon(string weaponName, bool equip)
    {

    }

    [PunRPC]
    void SetCurrentWeapon(int index)
    {
        foreach (var w in unlockedWeapons)
        {
            w.SetActive(false);
        }
        currentWeapon = unlockedWeapons[index];
        currentWeapon.SetActive(true);
        currentWeaponScript = currentWeapon.GetComponent<Weapon>();
        currentWeaponScript.Equip();
    }

    void NextWeapon()
    {
        if (currentWeaponIndex + 1 >= unlockedWeapons.Count)
            currentWeaponIndex = 0;
        else
            currentWeaponIndex++;

        photonView.RPC("SetCurrentWeapon", RpcTarget.All, currentWeaponIndex);
    }

    void PreviousWeapon()
    {
        if (currentWeaponIndex - 1 < 0)
            currentWeaponIndex = unlockedWeapons.Count - 1;
        else
            currentWeaponIndex--;

        photonView.RPC("SetCurrentWeapon", RpcTarget.All, currentWeaponIndex);
    }

    void SwitchWeapon()
    {
        if (unlockedWeapons.Count <= 1)
            return;

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            NextWeapon();
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            PreviousWeapon();
    }
}
