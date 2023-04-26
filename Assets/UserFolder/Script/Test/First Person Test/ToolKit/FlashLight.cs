using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Test;


public class FlashLight : Weapon
{
    [SerializeField] private Illuminant m_Illuminant;

    public enum LightMode
    {
        Narrow = 0,
        Wide = 1
    }
    private LightMode m_LightMode = LightMode.Narrow;

    private Scriptable.FlashLightSoundScriptable m_FlashLightSoundScriptable;
    private Scriptable.FlashLightStatScriptable m_FlashLightStatScriptable;
    private AudioClip audioClip;

    private bool m_IsLightOn = true;
    private int m_LightModeLength;

    protected override void Awake()
    {
        base.Awake();
        m_LightModeLength = System.Enum.GetValues(typeof(LightMode)).Length;
        m_FlashLightSoundScriptable = (Scriptable.FlashLightSoundScriptable)m_WeaponSoundScriptable;
        m_FlashLightStatScriptable = (Scriptable.FlashLightStatScriptable)m_WeaponStatScriptable;
        audioClip = m_FlashLightSoundScriptable.m_SwitchOnSound;
    }

    public override void Init()
    {
        base.Init();
        m_Pivot.localPosition = m_WeaponManager.m_OriginalPivotPosition;
        m_Pivot.localRotation = m_WeaponManager.m_OriginalPivotRotation;
        m_MainCamera.fieldOfView = m_WeaponManager.m_OriginalFOV;
    }

    protected override void AssignKeyAction()
    {
        base.AssignKeyAction();
        m_PlayerInputController.SemiFire += TryLightOnOff;
        m_PlayerInputController.HeavyFire += TryChangeLightMode;
    }

    private void TryLightOnOff()
    {
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");

        m_IsLightOn = !m_IsLightOn;
        audioClip = m_IsLightOn ? m_FlashLightSoundScriptable.m_SwitchOnSound : m_FlashLightSoundScriptable.m_SwitchOffSound;

        m_AudioSource.PlayOneShot(audioClip);

        m_Illuminant.LightOnOff(m_IsLightOn);
    }

    private void TryChangeLightMode()
    {
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");

        m_LightMode = (LightMode)(((int)m_LightMode + 1) % m_LightModeLength);

        m_AudioSource.PlayOneShot(audioClip);

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
        m_PlayerInputController.SemiFire -= TryLightOnOff;
        m_PlayerInputController.HeavyFire -= TryChangeLightMode;
    }
}
