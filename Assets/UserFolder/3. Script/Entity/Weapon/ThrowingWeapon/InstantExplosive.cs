using Controller.Player.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantExplosive : Explosible
{
    [Header("Shake")]
    [SerializeField] private float m_ShakeRadiusAdd = 20;
    [SerializeField] private DistanceShakeController m_DistanceShakeController;

    public override void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot, AttackType bulletType)
    {
        base.Init(poolingObject, pos, rot, bulletType);
        StartCoroutine(InstantExplosion());
    }

    private IEnumerator InstantExplosion()
    {
        yield return base.Explosion();
        m_DistanceShakeController.CheckPlayerShake(transform.position, m_ShakeRadiusAdd + AttackRadius);
        base.Damage(true);
        yield return m_DestroyObjectSecond;

        base.EndExplosion();
        base.ReturnObject();
    }
}
