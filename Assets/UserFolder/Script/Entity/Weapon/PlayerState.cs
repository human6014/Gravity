using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBehaviorState
{
    Idle,
    Walking,
    Running,
    Crouching,
    Jumping
}

public enum PlayerWeaponState
{
    Idle,
    Aiming,
    Changing,
    Firing
}

public class PlayerState
{
    public PlayerBehaviorState PlayerBehaviorState { get; private set; }
    public PlayerWeaponState PlayerWeaponState { get; private set; }
    public PlayerWeaponState BeforePlayerWeaponState { get; private set; } 
    public PlayerState()
    {
        PlayerBehaviorState = PlayerBehaviorState.Idle;
        PlayerWeaponState = PlayerWeaponState.Idle;
        BeforePlayerWeaponState = PlayerWeaponState.Idle;
    }

    #region SetPlayerBehaviorState
    public void SetBehaviorIdle()
    {
        if (PlayerBehaviorState != PlayerBehaviorState.Jumping &&
            PlayerBehaviorState != PlayerBehaviorState.Crouching)
        {
            PlayerBehaviorState = PlayerBehaviorState.Idle;
        }
    }
    public void SetBehaviorWalking()
    {
        if (PlayerBehaviorState != PlayerBehaviorState.Running &&
           PlayerBehaviorState != PlayerBehaviorState.Jumping &&
           PlayerBehaviorState != PlayerBehaviorState.Crouching)
        {
            PlayerBehaviorState = PlayerBehaviorState.Walking;
        }
    }
    public void SetBehaviorRunning(bool value)
    {
        if (PlayerBehaviorState != PlayerBehaviorState.Jumping &&
            PlayerBehaviorState != PlayerBehaviorState.Crouching)
        {
            if(value) PlayerBehaviorState = PlayerBehaviorState.Running;
            else PlayerBehaviorState = PlayerBehaviorState.Walking;
        }
    }
    public void SetBehaviorCrouching(bool value)
    {
        if (value) PlayerBehaviorState = PlayerBehaviorState.Crouching;
        else PlayerBehaviorState = PlayerBehaviorState.Idle;
    }
    public void SetBehaviorJumping(bool value)
    {
        if (value) PlayerBehaviorState = PlayerBehaviorState.Jumping;
        else PlayerBehaviorState = PlayerBehaviorState.Idle;
    }
    #endregion

    #region SetPlayerWeaponState
    public void SetWeaponIdle()
    {
        if (PlayerWeaponState != PlayerWeaponState.Changing &&
            PlayerWeaponState != PlayerWeaponState.Firing)
        {
            PlayerWeaponState = PlayerWeaponState.Idle;
        }
    }
    public void SetWeaponAiming()
    {
        if (PlayerWeaponState != PlayerWeaponState.Changing &&
            PlayerWeaponState != PlayerWeaponState.Firing)
        {
            PlayerWeaponState = PlayerWeaponState.Aiming;
        }
    }
    public void SetWeaponChanging(bool value)
    {
        if (value) PlayerWeaponState = PlayerWeaponState.Changing;
        else PlayerWeaponState = PlayerWeaponState.Idle;
    }
    public void SetWeaponFiring()
    {
        if (PlayerWeaponState == PlayerWeaponState.Changing ||
            PlayerWeaponState == PlayerWeaponState.Aiming) return;

        BeforePlayerWeaponState = PlayerWeaponState;
        PlayerWeaponState = PlayerWeaponState.Firing;
    }
    public void SetBack() => PlayerWeaponState = BeforePlayerWeaponState;
    #endregion
}
