using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantExplosive : Explosible
{
    [Header("Shake")]
    [SerializeField] private float m_ShakeRadiusAdd = 20;
    [SerializeField] private DistanceShakeController m_DistanceShakeController;

    protected override void Awake()
    {
        base.Awake();
        m_DistanceShakeController.Init(FindObjectOfType<Controller.Player.Utility.PlayerShakeController>());
    }

    public override void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot, AttackType bulletType)
    {
        base.Init(poolingObject, pos, rot, bulletType);
        StartCoroutine(InstantExplosion());
    }

    private IEnumerator InstantExplosion()
    {
        yield return base.Explosion();
        m_DistanceShakeController.CheckPlayerShake(ShakeType.Explosion, transform.position, m_ShakeRadiusAdd + AttackRadius, 20);
        base.Damage(true);
        yield return m_DestroyObjectSecond;

        base.EndExplosion();
        base.ReturnObject();
    }
}
