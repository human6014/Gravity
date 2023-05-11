using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Manager;
using Scriptable;

namespace Entity.Object.Weapon
{
    [RequireComponent(typeof(RangeWeapon))]
    public class FullInteractableReload : Reloadable
    {
        protected override void Awake()
        {
            base.Awake();
            m_HowInteratable = Interactabe.Full;
        }

        public override void DoReload(bool m_IsEmpty, int difference)
        {
            if (m_IsEmpty) StartCoroutine(InteractableEmptyReload(difference));
            else StartCoroutine(InteractableNonEmptyReload(difference));
        }

        private IEnumerator InteractableNonEmptyReload(int difference)
        {
            m_IsReloading = true;
            m_IsNonEmptyReloading = true;

            yield return base.DelaySoundWithAnimation(m_RangeWeaponSound.reloadSoundClips, difference);

            m_IsNonEmptyReloading = false;
            m_IsReloading = false;
        }

        private IEnumerator InteractableEmptyReload(int difference)
        {
            m_IsReloading = true;
            m_IsEmptyReloading = true;

            m_ArmAnimator.SetTrigger("Empty Reload");
            m_EquipmentAnimator.SetTrigger("Empty Reload");

            yield return base.DelaySound(m_RangeWeaponSound.emptyReloadSoundClips, 1, 0.4f);
            m_PlayerData.RangeWeaponCountingReload();
            if (difference != 1)
            {
                m_ArmAnimator.SetTrigger("Start Reload");
                m_EquipmentAnimator.SetTrigger("Start Reload");

                yield return base.DelaySoundWithAnimation(m_RangeWeaponSound.reloadSoundClips, difference - 1, 0.3f);
            }
            m_IsEmptyReloading = false;
            m_IsReloading = false;
        }


        public override void StopReload()
        {
            m_IsReloading = false;
            m_IsEmptyReloading = false;
            m_IsNonEmptyReloading = false;
            StopAllCoroutines();
        }

        public override bool CanFire() => true;
    }
}
