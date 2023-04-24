using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Test;

public enum LightMode
{
    Narrow = 1,
    Wide = 2
}
public class FlashLight : MonoBehaviour
{
    [SerializeField] private Animator m_ArmAnimator;
    [SerializeField] private AnimatorOverrideController m_ArmOverrideController;

    [SerializeField] private PlayerInputController m_PlayerInputController;
    [SerializeField] private WeaponSway m_WeaponSway;
    [SerializeField] private PlayerData m_PlayerData;
    [SerializeField] private Illuminant m_Illuminant;
    [SerializeField] Scriptable.FlashLightSoundScriptable m_FlashLightSetting;

    private Animator m_EquipmentAnimator;
    private AudioClip audioClip;
    private bool m_IsLightOn = true;
    private LightMode m_LightMode = LightMode.Wide;
    private int m_LightModeLength;

    private void Awake()
    {
        m_EquipmentAnimator = GetComponent<Animator>();
        m_LightModeLength = System.Enum.GetValues(typeof(LightMode)).Length;
    }

    public void Init()
    {
        gameObject.SetActive(true);
        m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;


        AssignKetAction();
    }

    private void AssignKetAction()
    {
        m_PlayerInputController.SemiFire += TryLightOnOff;
        m_PlayerInputController.HeavyFire += TryChangeLightMode;
    }

    private void TryLightOnOff()
    {
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");

        m_IsLightOn = !m_IsLightOn;
        audioClip = m_IsLightOn ? m_FlashLightSetting.m_SwitchOnSound : m_FlashLightSetting.m_SwitchOffSound;
        m_
        m_Illuminant.LightOnOff(m_IsLightOn);
    }

    private void TryChangeLightMode()
    {
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");

        m_LightMode = (LightMode)(((int)m_LightMode + 1) % m_LightModeLength);
        m_Illuminant.ChangeLightMode(m_LightMode);
    }

    public void Dispose()
    {
        DischargeKeyAction();
        gameObject.SetActive(false);
    }

    private void DischargeKeyAction()
    {
        m_PlayerInputController.SemiFire -= TryLightOnOff;
        m_PlayerInputController.HeavyFire -= TryChangeLightMode;
    }
}
