using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetting : Setting
{
    public static float m_MasterVolume { get; set; }    //0.001 ~ 1
    public static float m_MusicVolume { get; set; }     //0.001 ~ 1
    public static float m_SFXVolume { get; set; }       //0.001 ~ 1

    public override void LoadDefault()
    {
        m_MasterVolume = 1;
        m_MusicVolume = 1;
        m_SFXVolume = 1;
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
