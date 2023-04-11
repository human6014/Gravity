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
    protected override void FireRay()
    {
        if (Physics.Raycast(m_MuzzlePos.position, m_MainCamera.forward, out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            base.ProcessingRay(hit, 0);
    }
}
