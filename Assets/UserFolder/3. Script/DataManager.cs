using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    private Setting[] m_Settings;
    private bool m_HasData;

    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            GameAwake();
        }
        else Destroy(Instance.gameObject);
    }

    private void GameAwake()
    {
        m_Settings = new Setting[4];
        m_Settings[0] = new GamePlaySetting();
        m_Settings[1] = new GameControlSetting();
        m_Settings[2] = new AudioSetting();
        m_Settings[3] = new VisualSetting();

        m_HasData = PlayerPrefs.HasKey("HasData");

        if (m_HasData)
        {
            foreach (Setting setting in m_Settings)
                setting.LoadData();
        }
        else
        {
            PlayerPrefs.SetInt("HasData", m_HasData ? 1 : 0);
            foreach (Setting setting in m_Settings)
                setting.SaveData();
        }
    }

    [ContextMenu("DebugGamePlaySetting")]
    public void DebugGamePlaySetting()
    {
        ((GamePlaySetting)m_Settings[0]).DebugAllSetting();
    }

    [ContextMenu("DebugControlSetting")]
    public void DebugControlSetting()
    {
        ((GameControlSetting)m_Settings[1]).DebugAllSetting();
    }

    [ContextMenu("DebugAudioSetting")]
    public void DebugAudioSetting()
    {
        ((AudioSetting)m_Settings[2]).DebugAllSetting();
    }

    [ContextMenu("DebugVisualSetting")]
    public void DebugVisualSetting()
    {
        ((VisualSetting)m_Settings[3]).DebugAllSetting();
    }
}
