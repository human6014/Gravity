using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Contoller.Player;
using Manager;

[RequireComponent(typeof(Test.RangeWeapon))]
public class NormalFire : Fireable
{
    protected override bool FireRay()
    {
        if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward + base.GetCurrentAccuracy(), out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            return base.ProcessingRay(hit, 0);
        return false;
    }
}
