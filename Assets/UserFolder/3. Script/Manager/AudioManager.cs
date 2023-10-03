using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;

    private AudioSetting m_AudioSetting;

    private bool m_HasData;

    private void Awake()
    {
        if (DataManager.Instance == null) m_HasData = false;
        else
        {
            m_HasData = true;
            m_AudioSetting = (AudioSetting)DataManager.Instance.Settings[2];
            ApplySetting();
        }
    }

    public void ApplySetting()
    {
        if (!m_HasData) return;
        m_AudioMixer.SetFloat("Master", Mathf.Log10(m_AudioSetting.m_MasterVolume) * 20);
        m_AudioMixer.SetFloat("Music", Mathf.Log10(m_AudioSetting.m_MusicVolume) * 20);
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(m_AudioSetting.m_SFXUIVolume) * 20);
    }
}
