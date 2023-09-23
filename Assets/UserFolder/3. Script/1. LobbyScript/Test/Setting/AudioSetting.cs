using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetting : Setting
{
    public static float m_MasterVolume { get; set; }    //0.001 ~ 1
    public static float m_MusicVolume { get; set; }     //0.001 ~ 1
    public static float m_SFXVolume { get; set; }       //0.001 ~ 1

    public float this[string name]
    {
        get
        {
            switch (name)
            {
                case "MasterVolume": return m_MasterVolume;
                case "MusicVolume": return m_MusicVolume;
                case "SFXVolume": return m_SFXVolume;
                default: Debug.Log("Indexer name is null"); return -1;
            }
        }
        set
        {
            switch (name)
            {
                case "MasterVolume":m_MasterVolume = value;break;
                    case "MusicVolume": m_MusicVolume = value; break;
                case "SFXVolume": m_SFXVolume = value; break;
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
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_MasterVolume : " + m_MasterVolume);
        Debug.Log("m_MusicVolume : " + m_MusicVolume);
        Debug.Log("m_SFXVolume : " + m_SFXVolume);
    }
}
