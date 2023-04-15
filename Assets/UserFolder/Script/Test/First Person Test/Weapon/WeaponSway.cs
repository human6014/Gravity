using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    [SerializeField] private Vector3 m_OriginalPivotPosition;
    [SerializeField] private Vector3 m_IdleLimitPos;
    [SerializeField] private Vector3 m_AimingLimitPos;
    [SerializeField] private Vector3 m_RunningLimitPos;

    [SerializeField] private Vector3 m_SmoothSway;
    [SerializeField] private Transform m_Pivot;
    private PlayerState m_PlayerState;
    private Vector3 m_TargetPos;
    private Vector3 m_CurrentPos;

    private void Awake()
    {
        m_PlayerState = FindObjectOfType<Contoller.Player.FirstPersonController>().m_PlayerState;
    }
    private void TryMouseSway(float xMovement, float yMovement)
    {
        if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Idle)
        {
            if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Idle) m_TargetPos = m_OriginalPivotPosition;
            //else if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Aiming) m_TargetPos = m_AimingPivotPosition;

            m_CurrentPos.Set(Mathf.Clamp(Mathf.Lerp(m_CurrentPos.x, -xMovement, m_SmoothSway.x), -m_IdleLimitPos.x, m_IdleLimitPos.x),
             Mathf.Clamp(Mathf.Lerp(m_CurrentPos.y, -yMovement, m_SmoothSway.y), -m_IdleLimitPos.y, m_IdleLimitPos.y),
             m_Pivot.localPosition.z);
            m_Pivot.localPosition = m_CurrentPos;
        }
    }
}
