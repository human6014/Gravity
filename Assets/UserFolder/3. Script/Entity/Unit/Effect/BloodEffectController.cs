using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectController : PoolableScript
{
    public void Init(Manager.ObjectPoolManager.PoolingObject poolingObject)
    {
        m_PoolingObject = poolingObject;
        Invoke(nameof(ReturnObject),15);
    }

    public override void ReturnObject()
    {
        m_PoolingObject.ReturnObject(this);
    }
}
