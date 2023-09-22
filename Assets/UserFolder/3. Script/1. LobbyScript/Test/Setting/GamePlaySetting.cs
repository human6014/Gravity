using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySetting : Setting
{
    public static bool m_Notification { get; set; }
    public static int m_Language { get; set; } //0 : English, 1 : Korean

    public static int m_NotificationPosition { get; set; }
    public static bool m_EnableHUD { get; set; }

    public static int m_DifficultyIndex { get; set; }

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
