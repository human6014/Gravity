using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSetting : Setting
{
    private readonly AudioMixer m_AudioMixer;

    public float m_MasterVolume { get; set; }    //0.001 ~ 1
    public float m_MusicVolume { get; set; }     //0.001 ~ 1
    public float m_SFXUIVolume { get; set; }       //0.001 ~ 1
    public float m_SFXNormalVolume { get; set; }
    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return m_MasterVolume;
                case 1: return m_MusicVolume;
                case 2: return m_SFXUIVolume;
                case 3: return m_SFXNormalVolume;
                default: Debug.Log("Indexer name is null"); return -1;
            }
        }
        set
        {
            switch (index)
            {
                case 0:m_MasterVolume = value;break;
                case 1: m_MusicVolume = value; break;
                case 2: m_SFXUIVolume = value; break;
                case 3: m_SFXNormalVolume = value;break;
                default: Debug.Log("Indexer name is null");break;
            }
        }
    }

    public AudioSetting(AudioMixer audioMixer)
    {
        m_AudioMixer = audioMixer;
    }

    public override void LoadDefault()
    {
        Debug.Log("Load Default AudioSetting");

        m_MasterVolume = 1;
        m_MusicVolume = 1;
        m_SFXUIVolume = 1;
        m_SFXNormalVolume = 1;
    }

    public override void LoadData()
    {
        Debug.Log("Load AudioSetting");

        m_MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        m_MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        m_SFXUIVolume = PlayerPrefs.GetFloat("SFXUIVolume");
        m_SFXNormalVolume = PlayerPrefs.GetFloat("SFXNormalVolume");

        m_AudioMixer.SetFloat("Master", Mathf.Log10(m_MasterVolume) * 20);
        m_AudioMixer.SetFloat("Music", Mathf.Log10(m_MusicVolume) * 20);
        m_AudioMixer.SetFloat("SFX_UI", Mathf.Log10(m_SFXUIVolume) * 20);
        m_AudioMixer.SetFloat("SFX_Normal", Mathf.Log10(m_SFXNormalVolume) * 20);
    }

    public override void SaveData()
    {
        Debug.Log("Save AudioSetting");

        PlayerPrefs.SetFloat("MasterVolume",m_MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", m_MusicVolume);
        PlayerPrefs.SetFloat("SFXUIVolume", m_SFXUIVolume);
        PlayerPrefs.SetFloat("SFXNormalVolume", m_SFXNormalVolume);

        PlayerPrefs.Save();
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_MasterVolume : " + m_MasterVolume);
        Debug.Log("m_MusicVolume : " + m_MusicVolume);
        Debug.Log("m_SFXUIVolume : " + m_SFXUIVolume);
        Debug.Log("m_SFXNormalVolume : " + m_SFXNormalVolume);
    }
}
