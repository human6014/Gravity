using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamePlaySettingController : SettingController
{
    [SerializeField] private UnityEvent m_ApplySettingEvent;
    private GamePlaySetting m_GamePlaySetting;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
        UpdateSettings();
    }

    public void ChangeEnableNotification(int value)
        => m_GamePlaySetting[0] = value;
    
    public void ChangeLanguage(int value)
        => m_GamePlaySetting[1] = value;
    
    public void ChangeNotificationPosition(int value)
        => m_GamePlaySetting[2] = value;
    
    public void ChangeEnableHUD(int value)
        => m_GamePlaySetting[3] = value;

    public void ChangeDifficultyIndex(int value)
        => m_GamePlaySetting[4] = value;

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_GamePlaySetting[i]);
    }

    public override void SaveSettings()
    {
        if (m_GamePlaySetting == null) return;
        m_GamePlaySetting.SaveData();
        m_ApplySettingEvent?.Invoke();
    }
}
