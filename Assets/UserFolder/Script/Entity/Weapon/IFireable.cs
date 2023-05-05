using Contoller.Player;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Object.Weapon
{
    public interface IFireable
    {
        public void Setup(Scriptable.Equipment.RangeWeaponStatScriptable m_RangeWeaponStat, ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects,
                        SurfaceManager m_SurfaceManager, FirstPersonController m_FirstPersonController);

        public void SetupCasingPooling(ObjectPoolManager.PoolingObject m_CasingPoolingObject);

        public void DoFire();
    }
}
