using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySettingController : MonoBehaviour
{
    public void ChangeEnavleNotification(bool value)
        => GamePlaySetting.m_Notification = value;
    
    public void ChangeLanguage(int value)
        => GamePlaySetting.m_Language = value;
    
    public void ChangeNotificationPosition(int value)
        => GamePlaySetting.m_NotificationPosition = value;
    
    public void ChangeEnableHUD(bool value)
        => GamePlaySetting.m_EnableHUD = value;
}
