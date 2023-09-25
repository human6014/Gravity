using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingController : SettingController
{
    private AudioSetting m_AudioSetting;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_AudioSetting = (AudioSetting)DataManager.Instance.Settings[2];
        
        UpdateSettings();
    }

    public void ChangeMasterVolume(float value)
    {
        //AudioSetting.m_MasterVolume = Mathf.Log10(value) * 20;
        m_AudioSetting.m_MasterVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        m_AudioSetting.m_MusicVolume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        m_AudioSetting.m_SFXVolume = value;
    }

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_AudioSetting[i]);
    }
}
