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
    private WaitForSeconds m_EndFireSecond;
    protected override void Awake()
    {
        base.Awake();

        m_EndFireSecond = new WaitForSeconds(0.1f);
    }

    public override void DoFire()
    {
        base.DoFire();
        base.InstanceBullet();
        StartCoroutine(EndFire());
    }

    protected override void FireRay()
    {
        if (Physics.Raycast(m_MuzzlePos.position, mainCamera.forward, out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            base.ProcessingRay(hit, 0);
    }

    protected override IEnumerator EndFire()
    {
        yield return m_EndFireSecond;
        m_FireLight.Stop(false);
    }
}
