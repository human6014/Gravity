using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySetting : Setting
{
    private static bool m_Notification = true;
    private static int m_Language = 0; //0 : English, 1 : Korean

    private static int m_NotificationPosition = 0;
    private static bool m_EnableHUD = true;

    public bool Notification
    {
        get => m_Notification;
        set
        {
            m_Notification = value;
        }
    }

    private int Language
    {
        get => m_Language;
        set
        {
            m_Language = value;
        }
    }

    private int NotificationPosition
    {
        get => m_NotificationPosition;
        set
        {
            m_NotificationPosition = value;
        }
    }

    private bool EnableHUD
    {
        get => m_EnableHUD;
        set
        {
            m_EnableHUD = value;
        }
    }
    public override void LoadDefault()
    {
        m_Notification = true;
        m_Language = 0;
        m_NotificationPosition = 0;
        m_EnableHUD = true;
    }

    public override void LoadData()
    {
        m_Notification = PlayerPrefs.GetInt("Notification") == 1;
        m_Language = PlayerPrefs.GetInt("Language");
        m_NotificationPosition = PlayerPrefs.GetInt("NotificationPosition");
        m_EnableHUD = PlayerPrefs.GetInt("EnableHUD") == 1;
    }

    public override void SaveData()
    {
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
