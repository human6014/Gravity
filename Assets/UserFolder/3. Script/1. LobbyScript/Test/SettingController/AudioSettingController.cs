using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingController : MonoBehaviour
{
    public void ChangeMasterVolume(float value)
    {
        //AudioSetting.m_MasterVolume = Mathf.Log10(value) * 20;
        AudioSetting.m_MasterVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        AudioSetting.m_MusicVolume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        AudioSetting.m_SFXVolume = value;
    }
}
