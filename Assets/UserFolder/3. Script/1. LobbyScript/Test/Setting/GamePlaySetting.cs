using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlaySetting : Setting
{
    public int m_Notification { get; set; } //0 : Disable, 1 : Enable
    public int m_Language { get; set; }     //0 : English, 1 : Korean

    public int m_NotificationPosition { get; set; }
    public int m_EnableHUD { get; set; }    //0 : Disable, 1 : Enable

    public int m_DifficultyIndex { get; set; }

    public int m_HasHardClearData { get; set; } //0 : Not Cleared, 1: Cleared
    public int m_DisplayFrameRate { get; set; } //0 : Disable, 1 : Enable

    public object this[int index]
    {
        get
        {
            switch (index) 
            {
                case 0: return m_Notification;
                case 1: return m_Language;
                case 2: return m_NotificationPosition;
                case 3: return m_EnableHUD;
                case 4: return m_DisplayFrameRate;
                case 5: return m_DifficultyIndex;
                case 6: return m_HasHardClearData;
                default: Debug.Log("Indexer name is null"); return null;
            }
        }
        set
        {
            switch (index) 
            {
                case 0: m_Notification = (int)value; break;
                case 1: m_Language = (int)value; break;
                case 2: m_NotificationPosition = (int)value; break;
                case 3: m_EnableHUD = (int)value; break;
                case 4: m_DisplayFrameRate = (int)value; break;
                case 5: m_DifficultyIndex = (int)value; break;
                case 6: m_HasHardClearData = (int)value; break;
                default: Debug.Log("Indexer name is null"); break;
            }
        }
    }

    public override void LoadDefault()
    {
        Debug.Log("Load Default GamePlaySettings");

        m_Notification = 1;      //1
        m_Language = 0;             //0
        m_NotificationPosition = 2; //0
        m_EnableHUD = 1;         //1
        m_HasHardClearData = 0;
        m_DisplayFrameRate = 0;
    }

    public override void LoadData()
    {
        Debug.Log("Load GamePlaySettings");

        m_Notification = PlayerPrefs.GetInt("Notification");
        m_Language = PlayerPrefs.GetInt("Language");
        m_NotificationPosition = PlayerPrefs.GetInt("NotificationPosition");
        m_EnableHUD = PlayerPrefs.GetInt("EnableHUD");
        m_HasHardClearData = PlayerPrefs.GetInt("HasClearData");
        m_DisplayFrameRate = PlayerPrefs.GetInt("DisplayFrameRate");
    }

    public override void SaveData()
    {
        Debug.Log("Save GamePlaySettings");

        PlayerPrefs.SetInt("Notification", m_Notification);
        PlayerPrefs.SetInt("Language", m_Language);
        PlayerPrefs.SetInt("NotificationPosition", m_NotificationPosition);
        PlayerPrefs.SetInt("EnableHUD", m_EnableHUD);
        PlayerPrefs.SetInt("HasClearData", m_HasHardClearData);
        PlayerPrefs.SetInt("DisplayFrameRate", m_DisplayFrameRate);

        PlayerPrefs.Save();
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_Notification : " + m_Notification);
        Debug.Log("m_Language : " + m_Language);
        Debug.Log("m_NotificationPosition : " + m_NotificationPosition);
        Debug.Log("m_EnableHUD : " + m_EnableHUD);
        Debug.Log("m_HasClearData : " + m_HasHardClearData);
        Debug.Log("m_DisplayFrameRate" + m_DisplayFrameRate);
    }
}
