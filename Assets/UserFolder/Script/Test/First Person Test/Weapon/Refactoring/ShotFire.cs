using Contoller.Player;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotFire : MonoBehaviour, IFireable
{
    public void DoFire()
    {
        throw new System.NotImplementedException();
    }

    public void Setup(RangeWeaponStatScriptable m_RangeWeaponStat, ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects, SurfaceManager m_SurfaceManager, FirstPersonController m_FirstPersonController)
    {
        throw new System.NotImplementedException();
    }

    public void SetupCasingPooling(ObjectPoolManager.PoolingObject m_CasingPoolingObject)
    {
        throw new System.NotImplementedException();
    }
}
