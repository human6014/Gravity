using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable.Equipment;

namespace Entity.Object.Weapon
{
    public class FlashLight : Weapon
    {
        [SerializeField] private Illuminant m_Illuminant;

        public enum LightMode
        {
            Narrow = 0,
            Wide = 1
        }
        private LightMode m_LightMode = LightMode.Narrow;

        private FlashLightSoundScriptable m_FlashLightSoundScriptable;
        private FlashLightStatScriptable m_FlashLightStatScriptable;
        private AudioClip audioClip;

        private bool m_IsLightOn = true;
        private int m_LightModeLength;

        protected override void Awake()
        {
            base.Awake();
            m_LightModeLength = System.Enum.GetValues(typeof(LightMode)).Length;
            m_FlashLightSoundScriptable = (FlashLightSoundScriptable)m_WeaponSoundScriptable;
            m_FlashLightStatScriptable = (FlashLightStatScriptable)m_WeaponStatScriptable;
            audioClip = m_FlashLightSoundScriptable.m_SwitchOnSound;
        }

        public override void Init()
        {
            base.Init();
            m_Pivot.localPosition = WeaponManager.OriginalPivotPosition;
            m_Pivot.localRotation = WeaponManager.OriginalPivotRotation;
            MainCamera.fieldOfView = WeaponManager.OriginalFOV;
        }

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            PlayerInputController.SemiFire += TryLightOnOff;
            PlayerInputController.HeavyFire += TryChangeLightMode;
        }

        private void TryLightOnOff()
        {
            m_ArmAnimator.SetTrigger("Use");
            EquipmentAnimator.SetTrigger("Use");

            m_IsLightOn = !m_IsLightOn;
            audioClip = m_IsLightOn ? m_FlashLightSoundScriptable.m_SwitchOnSound : m_FlashLightSoundScriptable.m_SwitchOffSound;

            AudioSource.PlayOneShot(audioClip);

            m_Illuminant.LightOnOff(m_IsLightOn);
        }

        private void TryChangeLightMode()
        {
            m_ArmAnimator.SetTrigger("Use");
            EquipmentAnimator.SetTrigger("Use");

            m_LightMode = (LightMode)(((int)m_LightMode + 1) % m_LightModeLength);

            AudioSource.PlayOneShot(audioClip);

            m_Illuminant.ChangeLightMode(m_FlashLightStatScriptable.m_ZoomAngle[(int)m_LightMode]);
        }

        public override void Dispose()
        {
            base.Dispose();
            gameObject.SetActive(false);
        }

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            PlayerInputController.SemiFire -= TryLightOnOff;
            PlayerInputController.HeavyFire -= TryChangeLightMode;
        }
    }
}
