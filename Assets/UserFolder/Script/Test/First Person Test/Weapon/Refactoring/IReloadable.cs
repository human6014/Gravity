using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReloadable
{
    public bool GetIsNonEmptyReloading();
    public void Setup(RangeWeaponSoundScriptable m_RangeWeaponSound,Animator m_ArmAnimator);
    public void SetupMagazinePooling(ObjectPoolManager.PoolingObject m_MagazinePoolingObject);
    public void DoReload(bool m_IsEmpty);
}
