using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    #region General States
    public bool IsAlive { get; set; }
    #endregion

    #region Camera States
    // Future states like disorientation etc
    #endregion

    #region Movement States
    public bool IsMoving { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsSneaking { get; set; }
    public bool IsGrounded { get; set; }
    #endregion

    #region Weapon States
    public bool IsAiming { get; set; }
    public bool IsReloading { get; set; }
    public int CurrentAmmo { get; set; }
    public int CurrentAmmoReserve { get; set; }
    #endregion

    #region Restrictions
    private bool canControl = true;
    public bool CanControl
    {
        get
        {
            return canControl;
        }
        set
        {
            canControl = value;
        }
    }
    private bool canMove = true;
    public bool CanMove
    {
        get
        {
            return canMove;
        }
        set
        {
            canMove = value;
        }
    }
    private bool canSprint = true;
    public bool CanSprint 
    { 
        get 
        {
            return canSprint && !IsAiming;
        } 
        set
        {
            canSprint = value;
        } 
    }
    private bool canRespawn = true;
    public bool CanRespawn
    {
        get
        {
            return canRespawn;
        }
        set
        {
            canRespawn = value;
        }
    }
    private bool canChangeWeapon = true;
    public bool CanChangeWeapon 
    {
        get
        {
            return canChangeWeapon && !IsReloading;
        }
        set
        {
            canChangeWeapon = value;
        }
    }
    #endregion
}
