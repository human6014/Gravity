using Entity.Object;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object.Weapon
{
    [RequireComponent(typeof(RangeWeapon))]
    public class SemiInteractableReload : Reloadable
    {
        protected override void Awake()
        {
            base.Awake();
            HowInteratable = Interactabe.Semi;
        }

        public override void DoReload(bool m_IsEmpty, int difference)
        {
            if (m_IsEmpty) StartCoroutine(NonInteractableEmptyReload());
            else StartCoroutine(InteractableNonEmptyReload(difference));
        }

        private IEnumerator InteractableNonEmptyReload(int difference)
        {
            IsReloading = true;
            IsNonEmptyReloading = true;

            m_ArmAnimator.SetTrigger("Empty Reload");
            m_EquipmentAnimator.SetTrigger("Empty Reload");

            yield return base.DelaySound(m_RangeWeaponSound.reloadStartSoundClips, 1);

            yield return base.DelaySoundWithAnimation(m_RangeWeaponSound.reloadSoundClips, difference, 0.4f);

            yield return base.DelaySound(m_RangeWeaponSound.reloadEndSoundClips, 1);

            IsNonEmptyReloading = false;
            IsReloading = false;
        }

        private IEnumerator NonInteractableEmptyReload()
        {
            IsReloading = true;
            IsEmptyReloading = true;

            m_ArmAnimator.SetTrigger("Empty Reload");
            m_EquipmentAnimator.SetTrigger("Empty Reload");

            yield return base.DelaySound(m_RangeWeaponSound.emptyReloadSoundClips, 1);

            m_PlayerData.RangeWeaponReload();
            base.InstanceMagazine();

            IsEmptyReloading = false;
            IsReloading = false;
        }

        public override void StopReload()
        {
            if (IsNonEmptyReloading)
            {
                IsReloading = false;
                IsNonEmptyReloading = false;
                StopAllCoroutines();
            }
        }

        public override bool CanFire() => !IsEmptyReloading;

    }
}
