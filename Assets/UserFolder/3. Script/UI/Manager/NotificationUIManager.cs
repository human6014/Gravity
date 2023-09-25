using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_NotificationObject;
    [SerializeField] private Text [] m_NotificationPos;

    private GamePlaySetting m_GamePlaySetting;
    private Text m_CurrentText;

    private bool m_HasData;

    private void Awake()
    {
        if (DataManager.Instance == null) m_HasData = false;
        else
        {
            m_HasData = true;
            m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            m_CurrentText = m_NotificationPos[0];
            ApplySetting();
        }
    }

    public void ApplySetting()
    {
        if (!m_HasData) return;
        if (!m_GamePlaySetting.m_Notification)
        {
            m_NotificationObject.SetActive(false);
            return;
        }

        m_NotificationObject.SetActive(true);
        m_CurrentText.gameObject.SetActive(false);
        m_CurrentText = m_NotificationPos[m_GamePlaySetting.m_NotificationPosition];
        m_CurrentText.gameObject.SetActive(true);
        m_CurrentText.text = "CurrentNotificationPos";
    }

    public void UpdateText()
    {

    }
}
