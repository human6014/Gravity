using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Contoller.Player;
using Manager;

[RequireComponent(typeof(Test.RangeWeapon))]
public class ShotFire : Fireable
{
    [Header("Fire")]
    [Tooltip("ÃÑ¾Ë °³¼ö")]
    [SerializeField] private int m_RayNum;
    [SerializeField] private Vector3 m_SpreadRange;

    protected override void FireRay()
    {
        for (int i = 0; i < m_RayNum; i++)
        {
            if (Physics.Raycast(m_MuzzlePos.position, GetFireDirection() + base.GetCurrentAccuracy(), out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
                base.ProcessingRay(hit, i);
        }
    }

    private Vector3 GetFireDirection()
    {
        Vector3 targetPos = m_CameraTransform.position + m_CameraTransform.forward * m_RangeWeaponStat.m_MaxRange;

        targetPos.x += Random.Range(-m_SpreadRange.x, m_SpreadRange.x);
        targetPos.y += Random.Range(-m_SpreadRange.y, m_SpreadRange.y);
        targetPos.z += Random.Range(-m_SpreadRange.z, m_SpreadRange.z);

        Vector3 direction = targetPos - m_CameraTransform.position;
        return direction.normalized;
    }
}
