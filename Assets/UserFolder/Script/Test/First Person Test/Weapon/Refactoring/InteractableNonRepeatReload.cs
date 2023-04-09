using Entity.Object;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Test.RangeWeapon))]
public class InteractableNonRepeatReload : Reloadable
{
    protected override void Awake()
    {
        base.Awake();
        m_HowInteratable = Interactabe.Semi;
    }

    public override void DoReload(bool m_IsEmpty)
    {
        if (m_IsEmpty) StartCoroutine(NonRepeatableEmptyReload());
        else StartCoroutine(NonRepeatableNonEmptyReload());
    }

    private IEnumerator NonRepeatableNonEmptyReload()
    {
        m_IsReloading = true;
        m_IsNonEmptyReloading = true;

        m_ArmAnimator.SetTrigger("Empty Reload");
        m_EquipmentAnimator.SetTrigger("Empty Reload");

        yield return base.DelaySound(m_RangeWeaponSound.reloadStartSoundClips, 1);

        yield return base.DelaySoundWithAnimation(m_RangeWeaponSound.reloadSoundClips, 5, 0.4f);

        yield return base.DelaySound(m_RangeWeaponSound.reloadEndSoundClips, 1);

        m_IsNonEmptyReloading = false;
        m_IsReloading = false;
    }

    private IEnumerator NonRepeatableEmptyReload()
    {
        m_IsReloading = true;
        m_IsEmptyReloading = true;

        m_ArmAnimator.SetTrigger("Empty Reload");
        m_EquipmentAnimator.SetTrigger("Empty Reload");

        yield return base.DelaySound(m_RangeWeaponSound.emptyReloadSoundClips, 1);

        base.InstanceMagazine();
        m_IsEmptyReloading = false;
        m_IsReloading = false;
    }

    public override void StopReload()
    {
        if (m_IsNonEmptyReloading)
        {
            m_IsReloading = false;
            m_IsNonEmptyReloading = false;
            StopAllCoroutines();
        }
    }

    public override bool CanFire() => !m_IsEmptyReloading;
    
}
