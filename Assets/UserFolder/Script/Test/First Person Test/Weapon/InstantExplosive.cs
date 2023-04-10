using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantExplosive : Explosible
{
    public override void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot)
    {
        base.Init(poolingObject, pos, rot);
        m_TriggerStay += Damage;
        StartCoroutine(InstantExplosion());
    }

    private IEnumerator InstantExplosion()
    {
        yield return base.Explosion();

        base.EndExplosion();
        base.ReturnObject();
    }

    protected override void Damage(bool isInside, Collider other)
    {
        base.Damage(isInside, other);
        if (isInside)
        {
            Debug.Log("Damage");
        }
    }
}
