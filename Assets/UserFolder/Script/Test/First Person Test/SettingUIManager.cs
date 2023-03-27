using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIManager : MonoBehaviour
{
    private GameObject m_GameUI;
    private GameObject m_SettingUI;

    private bool m_IsActiveSettingUI;
    private void Awake()
    {
        m_GameUI = transform.GetChild(0).gameObject;
        m_SettingUI = transform.GetChild(1).gameObject;

        MouseCursorSetting(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_IsActiveSettingUI = !m_IsActiveSettingUI;
            m_SettingUI.SetActive(m_IsActiveSettingUI);
            MouseCursorSetting(m_IsActiveSettingUI);
            Time.timeScale = m_IsActiveSettingUI ? 0 : 1;
        }
    }

    private void MouseCursorSetting(bool isActiveSettingUI)
    {
        Cursor.visible = isActiveSettingUI;
        if (isActiveSettingUI) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }
}
