using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSettingController : SettingController
{
    public void ChangeLookSensitivitySlider(float value)
        => GameControlSetting.m_LookSensitivity = value;

    public void ChangeAimSensitivitySlider(float value)
        => GameControlSetting.m_AimSensitivity = value;
    
    public void ChangeRunMode(int index)
        => GameControlSetting.m_RunMode = index;
    
    public void ChangeAimMode(int index)
        => GameControlSetting.m_AimMode = index;
}
