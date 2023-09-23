using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySettingController : SettingController
{
    protected override void Start()
    {
        base.Start();
        base.UpdateSettings();
    }

    public void ChangeEnavleNotification(bool value)
        => m_GamePlaySetting[0] = value;
    
    public void ChangeLanguage(int value)
        => m_GamePlaySetting[1] = value;
    
    public void ChangeNotificationPosition(int value)
        => m_GamePlaySetting[2] = value;
    
    public void ChangeEnableHUD(bool value)
        => m_GamePlaySetting[3] = value;

    public void ChangeDifficultyIndex(int value)
        => m_GamePlaySetting[4] = value;
}
