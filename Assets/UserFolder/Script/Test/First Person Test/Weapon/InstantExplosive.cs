using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantExplosive : Explosible
{
    public override void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot, BulletType bulletType)
    {
        base.Init(poolingObject, pos, rot, bulletType);
        StartCoroutine(InstantExplosion());
    }

    private IEnumerator InstantExplosion()
    {
        yield return base.Explosion();
        yield return m_DestroyObjectSecond;

        base.EndExplosion();
        base.ReturnObject();
    }
}
