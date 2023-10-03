using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingController : SettingController
{
    [SerializeField] private AudioMixer m_AudioMixer;

    private AudioSetting m_AudioSetting;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_AudioSetting = (AudioSetting)DataManager.Instance.Settings[2];
        
        UpdateSettings();
    }

    public void ChangeMasterVolume(float value)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        m_AudioSetting.m_MasterVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        m_AudioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
        m_AudioSetting.m_MusicVolume = value;
    }

    public void ChangeSFXUIVolume(float value)
    {
        m_AudioMixer.SetFloat("SFX_UI", Mathf.Log10(value) * 20);
        m_AudioSetting.m_SFXUIVolume = value;
    }

    public void ChangeSFXNormalVolume(float value)
    {
        m_AudioMixer.SetFloat("SFX_Normal", Mathf.Log10(value) * 20);
        m_AudioSetting.m_SFXNormalVolume = value;
    }

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_AudioSetting[i]);
    }

    public override void SaveSettings()
    {
        if (m_AudioSetting == null) return;
        m_AudioSetting.SaveData();
    }
}
