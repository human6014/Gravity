using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySettingController : SettingController
{
    private GamePlaySetting m_GamePlaySetting;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
        UpdateSettings();
    }

    public void ChangeEnableNotification(bool value)
        => m_GamePlaySetting[0] = value;
    
    public void ChangeLanguage(int value)
        => m_GamePlaySetting[1] = value;
    
    public void ChangeNotificationPosition(int value)
        => m_GamePlaySetting[2] = value;
    
    public void ChangeEnableHUD(bool value)
        => m_GamePlaySetting[3] = value;

    public void ChangeDifficultyIndex(int value)
        => m_GamePlaySetting[4] = value;

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_GamePlaySetting[i]);
    }
}
