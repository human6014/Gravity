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

    [Header("Timing")]
    [SerializeField] private float m_LightOffTime = 0.3f;
    [SerializeField] private float m_InstanceBulletTime = 1f;

    private WaitForSeconds m_LightOffSecond;
    private WaitForSeconds m_InstanceBulletSecond;

    protected override void Awake()
    {
        base.Awake();

        m_LightOffSecond = new WaitForSeconds(m_LightOffTime);
        m_InstanceBulletSecond = new WaitForSeconds(m_InstanceBulletTime - m_LightOffTime);
    }

    public override void DoFire()
    {
        base.DoFire();
        StartCoroutine(EndFire());
    }

    protected override void FireRay()
    {
        for (int i = 0; i < m_RayNum; i++)
        {
            if (Physics.Raycast(m_MuzzlePos.position, GetFireDirection(), out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
                base.ProcessingRay(hit, i);
        }
    }

    private Vector3 GetFireDirection()
    {
        Vector3 targetPos = mainCamera.position + mainCamera.forward * m_RangeWeaponStat.m_MaxRange;

        targetPos.x += Random.Range(-m_SpreadRange.x, m_SpreadRange.x);
        targetPos.y += Random.Range(-m_SpreadRange.y, m_SpreadRange.y);
        targetPos.z += Random.Range(-m_SpreadRange.z, m_SpreadRange.z);

        Vector3 direction = targetPos - mainCamera.position;
        return direction.normalized;
    }

    protected override IEnumerator EndFire()
    {
        yield return m_LightOffSecond;
        m_FireLight.Stop(false);

        yield return m_InstanceBulletSecond;
        InstanceBullet();
    }
}
