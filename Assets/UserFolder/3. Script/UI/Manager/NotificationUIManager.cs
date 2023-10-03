using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class NotificationUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_NotificationObject;
    [SerializeField] private TextMeshProUGUI[] m_NotificationPos;

    private GamePlaySetting m_GamePlaySetting;
    private TextMeshProUGUI m_CurrentText;
    private LocalizeStringEvent m_CurrentLocalizeStringEvent;

    private readonly string m_TableReference = "Language Table";
    private readonly string[] m_TableEntryReference =
        { 
            "GameScene_Notification1",
            "GameScene_Notification2",
            "GameScene_Notification3",
            "GameScene_Notification4",
        };

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
        if (m_GamePlaySetting.m_Notification == 0)
        {
            m_NotificationObject.SetActive(false);
            return;
        }

        m_NotificationObject.SetActive(true);
        m_CurrentText.gameObject.SetActive(false);
        m_CurrentText = m_NotificationPos[m_GamePlaySetting.m_NotificationPosition];
        m_CurrentText.gameObject.SetActive(true);

        m_CurrentLocalizeStringEvent = m_CurrentText.GetComponent<LocalizeStringEvent>();
        m_CurrentLocalizeStringEvent.StringReference.TableReference = m_TableReference;
        m_CurrentLocalizeStringEvent.StringReference.TableEntryReference = m_TableEntryReference[0];
    }

    public void UpdateText(int textNumber)
    {
        if (m_GamePlaySetting.m_Notification == 0) return;

        m_CurrentLocalizeStringEvent.StringReference.TableReference = m_TableReference;
        m_CurrentLocalizeStringEvent.StringReference.TableEntryReference = m_TableEntryReference[textNumber];
    }
}
