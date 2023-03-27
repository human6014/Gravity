using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIManager : MonoBehaviour
{
    [SerializeField] private SettingPanel m_SettingPanel;

    private GameObject m_GameUI;
    private GameObject m_SettingUI;

    public bool m_IsActiveSettingUI { get; private set; }
    private void Awake()
    {
        m_GameUI = transform.GetChild(0).gameObject;
        m_SettingUI = transform.GetChild(1).gameObject;

        MouseModeSetting(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_IsActiveSettingUI = !m_IsActiveSettingUI;
            m_SettingPanel.TryActive(m_IsActiveSettingUI);
            MouseModeSetting(m_IsActiveSettingUI);
        }
    }

    private void MouseModeSetting(bool isActiveSettingUI)
    {
        Cursor.visible = isActiveSettingUI;
        Cursor.lockState = isActiveSettingUI ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = isActiveSettingUI ? 0 : 1;
    }

    public void Resume()
    {
        Debug.Log("Resume");
    }

    public void ReturnLobby()
    {
        Debug.Log("ReturnLobby");
    }
}
