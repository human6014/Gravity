using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetting : Setting
{
    public float m_MasterVolume { get; set; }    //0.001 ~ 1
    public float m_MusicVolume { get; set; }     //0.001 ~ 1
    public float m_SFXVolume { get; set; }       //0.001 ~ 1

    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return m_MasterVolume;
                case 1: return m_MusicVolume;
                case 2: return m_SFXVolume;
                default: Debug.Log("Indexer name is null"); return -1;
            }
        }
        set
        {
            switch (index)
            {
                case 0:m_MasterVolume = value;break;
                case 1: m_MusicVolume = value; break;
                case 2: m_SFXVolume = value; break;
                default: Debug.Log("Indexer name is null");break;
            }
        }
    }

    public override void LoadDefault()
    {
        Debug.Log("Load Default AudioSetting");

        m_MasterVolume = 1;
        m_MusicVolume = 1;
        m_SFXVolume = 1;
    }

    public override void LoadData()
    {
        Debug.Log("Load AudioSetting");

        m_MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        m_MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        m_SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
    }

    public override void SaveData()
    {
        Debug.Log("Save AudioSetting");

        PlayerPrefs.SetFloat("MasterVolume",m_MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", m_MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", m_SFXVolume);

        PlayerPrefs.Save();
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_MasterVolume : " + m_MasterVolume);
        Debug.Log("m_MusicVolume : " + m_MusicVolume);
        Debug.Log("m_SFXVolume : " + m_SFXVolume);
    }
}
