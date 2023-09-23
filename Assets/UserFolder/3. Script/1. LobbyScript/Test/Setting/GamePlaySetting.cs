using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlaySetting : Setting
{
    private bool m_Notification { get; set; }
    private int m_Language { get; set; } //0 : English, 1 : Korean

    private int m_NotificationPosition { get; set; }
    private bool m_EnableHUD { get; set; }

    private int m_DifficultyIndex { get; set; }

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
                case 4: return m_DifficultyIndex;
                default: Debug.Log("Indexer name is null"); return null;
            }
        }
        set
        {
            switch (index) 
            {
                case 0: m_Notification = (bool)value; break;
                case 1: m_Language = (int)value; break;
                case 2: m_NotificationPosition = (int)value; break;
                case 3: m_EnableHUD = (bool)value; break;
                case 4: m_DifficultyIndex = (int)value; break;
                default: Debug.Log("Indexer name is null"); break;
            }
        }
    }

    public override void LoadDefault()
    {
        Debug.Log("Load Default GamePlaySettings");

        m_Notification = true;      //true
        m_Language = 1;             //0
        m_NotificationPosition = 2; //0
        m_EnableHUD = true;         //true
    }

    public override void LoadData()
    {
        Debug.Log("Load GamePlaySettings");

        m_Notification = PlayerPrefs.GetInt("Notification") == 1;
        m_Language = PlayerPrefs.GetInt("Language");
        m_NotificationPosition = PlayerPrefs.GetInt("NotificationPosition");
        m_EnableHUD = PlayerPrefs.GetInt("EnableHUD") == 1;
    }

    public override void SaveData()
    {
        Debug.Log("Save GamePlaySettings");

        PlayerPrefs.SetInt("Notification", m_Notification ? 1 : 0);
        PlayerPrefs.SetInt("Language", m_Language);
        PlayerPrefs.SetInt("NotificationPosition", m_NotificationPosition);
        PlayerPrefs.SetInt("EnableHUD", m_EnableHUD ? 1 : 0);
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_Notification : " + m_Notification);
        Debug.Log("m_Language : " + m_Language);
        Debug.Log("m_NotificationPosition : " + m_NotificationPosition);
        Debug.Log("m_EnableHUD : " + m_EnableHUD);
    }
}
