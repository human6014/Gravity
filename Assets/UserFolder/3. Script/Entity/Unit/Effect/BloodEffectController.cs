using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BloodEffectController : PoolableScript
{
    public Action ReturnObjectAction { get; set; }

    public void Init(Manager.ObjectPoolManager.PoolingObject poolingObject)
    {
        m_PoolingObject = poolingObject;
        Invoke(nameof(ReturnObject),15);
    }

    public override void ReturnObject()
    {
        ReturnObjectAction?.Invoke();
        m_PoolingObject.ReturnObject(this);
    }
}
