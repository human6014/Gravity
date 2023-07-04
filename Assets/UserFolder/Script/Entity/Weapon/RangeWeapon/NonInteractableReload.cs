using Entity.Object;
using Manager;
using Scriptable;
using Scriptable.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object.Weapon
{
    [RequireComponent(typeof(RangeWeapon))]
    public class NonInteractableReload : Reloadable
    {
        protected override void Awake()
        {
            base.Awake();
            HowInteratable = Interactabe.Non;
        }

        public override void DoReload(bool m_IsEmpty, int difference)
            => StartCoroutine(Reload(m_IsEmpty));


        private IEnumerator Reload(bool m_IsEmpty)
        {
            IsReloading = true;
            string animParamName = m_IsEmpty == true ? "Empty Reload" : "Reload";

            WeaponSoundScriptable.DelaySoundClip[] soundClips;
            if (m_IsEmpty) soundClips = m_RangeWeaponSound.emptyReloadSoundClips;
            else soundClips = m_RangeWeaponSound.reloadSoundClips;

            m_EquipmentAnimator.SetTrigger(animParamName);
            m_ArmAnimator.SetTrigger(animParamName);

            yield return base.DelaySound(soundClips, 1, 0.2f);

            m_PlayerData.RangeWeaponReload();
            base.InstanceMagazine();
            IsReloading = false;
        }

        public override bool CanFire() => !IsReloading;
    }
}
