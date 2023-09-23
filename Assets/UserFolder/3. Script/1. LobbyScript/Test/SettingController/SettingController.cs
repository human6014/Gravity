using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingController : MonoBehaviour
{
    [SerializeField] protected LoadableSettingComponent[] m_LoadableSettingComponents;

    protected GamePlaySetting m_GamePlaySetting;

    protected virtual void Start()
    {
        m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
    }

    public void UpdateSettings()
    {
        for(int i=0;i< m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_GamePlaySetting[i]);
    }
}
