using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    public static DataManager Instance { get; private set; }
    public Setting[] Settings { get; private set; }

    private bool m_HasData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameAwake();
        }
        else Destroy(gameObject);
    }

    private void GameAwake()
    {
        Settings = new Setting[4];
        Settings[0] = new GamePlaySetting();
        Settings[1] = new GameControlSetting();
        Settings[2] = new AudioSetting(m_AudioMixer);
        Settings[3] = new VisualSetting();

        m_HasData = PlayerPrefs.HasKey("HasData");
        Debug.Log("HasData is " + m_HasData);
        if (m_HasData)
        {
            foreach (Setting setting in Settings)
                setting.LoadData();
        }
        else
        {
            PlayerPrefs.SetInt("HasData", m_HasData ? 1 : 0);
            foreach (Setting setting in Settings)
            {
                setting.LoadDefault();
                setting.SaveData();
            }
        }
    }

    [ContextMenu("DebugGamePlaySetting")]
    public void DebugGamePlaySetting()
    {
        ((GamePlaySetting)Settings[0]).DebugAllSetting();
    }

    [ContextMenu("DebugControlSetting")]
    public void DebugControlSetting()
    {
        ((GameControlSetting)Settings[1]).DebugAllSetting();
    }

    [ContextMenu("DebugAudioSetting")]
    public void DebugAudioSetting()
    {
        ((AudioSetting)Settings[2]).DebugAllSetting();
    }

    [ContextMenu("DebugVisualSetting")]
    public void DebugVisualSetting()
    {
        ((VisualSetting)Settings[3]).DebugAllSetting();
    }
}
