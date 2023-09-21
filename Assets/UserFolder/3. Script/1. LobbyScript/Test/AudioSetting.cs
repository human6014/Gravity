using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetting : Setting
{
    private static float m_MasterVolume = 100;
    private static float m_MusicVolume = 100;
    private static float m_SFXVolume = 100;

    private float MasterVolume
    {
        get => m_MasterVolume;
        set
        {
            m_MasterVolume = value;
        }
    }

    private float MusicVolume
    {
        get => m_MusicVolume;
        set
        {
            m_MusicVolume = value;
        }
    }

    private float SFXVolume
    {
        get => m_SFXVolume;
        set
        {
            m_SFXVolume = value;
        }
    }

    public override void LoadDefault()
    {
        m_MasterVolume = 100;
        m_MusicVolume = 100;
        m_SFXVolume = 100;
    }

    public override void LoadData()
    {
        m_MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        m_MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        m_SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
    }

    public override void SaveData()
    {
        PlayerPrefs.SetFloat("MasterVolume",m_MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", m_MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", m_SFXVolume);
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_MasterVolume : " + m_MasterVolume);
        Debug.Log("m_MusicVolume : " + m_MusicVolume);
        Debug.Log("m_SFXVolume : " + m_SFXVolume);
    }
}
