using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralSettingController : SettingController
{
    private GameControlSetting m_GameControlSetting;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_GameControlSetting = (GameControlSetting)DataManager.Instance.Settings[1];
        UpdateSettings();
    }

    public void ChangeLookSensitivitySlider(float value)
        => m_GameControlSetting[0] = value;

    public void ChangeAimSensitivitySlider(float value)
        => m_GameControlSetting[1] = value;

    public void ChangeAimMode(int index)
        => m_GameControlSetting[2] = index;

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_GameControlSetting[i]);
    }

    public override void SaveSettings()
    {
        if (m_GameControlSetting == null) return;
        m_GameControlSetting.SaveData();
    }
}
