using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    #region General States
    public bool IsAlive { get; set; }
    public bool HasControl { get; set; }
    #endregion

    #region Camera States
    // Future states like disorientation etc
    #endregion

    #region Movement States
    public bool IsMoving { get; set; }
    public bool CanMove { get; set; } = true;
    public bool IsSprinting { get; set; }
    public bool CanSprint { get; set; } = true;
    public bool IsSneaking { get; set; }
    public bool IsGrounded { get; set; }
    #endregion

    #region Weapon States
    public bool IsAiming { get; set; }
    public bool IsReloading { get; set; }
    public int CurrentAmmo { get; set; }
    public int CurrentAmmoReserve { get; set; }
    public bool CanChangeWeapon { get; set; }
    #endregion
}
