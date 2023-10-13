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
    [SerializeField] private float m_TextTime = 7.5f;

    private CanvasGroup m_CanvasGroup;
    private GamePlaySetting m_GamePlaySetting;
    private TextMeshProUGUI m_CurrentText;
    private LocalizeStringEvent m_CurrentLocalizeStringEvent;
    private static NotificationUIManager m_NotificationUIManager;

    private readonly string m_TableReference = "Language Table";
    private readonly string[] m_TableEntryReference =
        { 
            "GameScene_Notification1",      //Tab 눌러봐            //완
            "GameScene_Notification2",      //미정2
            "GameScene_Notification3",      //약점이 있어용         //완
            "GameScene_Notification4",      //미정4
            "GameScene_Notification5",      //SP1 공격              //완
            "GameScene_Notification6",      //SP1 반피              //생각중
            "GameScene_Notification7",      //SP2 소환 후 잠깐      //완
            "GameScene_Notification8",      //SP2 촉수 공격         //완
            "GameScene_Notification9",      //SP2 도망 패턴         //완
            "GameScene_Notification10",     //SP3 소환              //완
            "GameScene_Notification11",     //SP3 짤몹 공격         //완
        };

    private bool m_HasData;
    
    private int m_EntryReferenceNumber;

    private void Awake()
    {
        m_CanvasGroup = m_NotificationObject.GetComponent<CanvasGroup>();
        m_NotificationUIManager = GetComponent<NotificationUIManager>();

        if (DataManager.Instance == null) m_HasData = false;
        else
        {
            m_HasData = true;
            m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            m_CurrentText = m_NotificationPos[0];   //일관성 때문에 일부러 이럼
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
        m_CurrentLocalizeStringEvent.StringReference.TableEntryReference = m_TableEntryReference[m_EntryReferenceNumber];
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        UpdateText(0);
    }

    public static void CallUpdateText(int referenceNumber)
    {
        if (m_NotificationUIManager == null) return;
        m_NotificationUIManager.UpdateText(referenceNumber);
    }

    private void UpdateText(int referenceNumber)
    {
        if (!m_HasData) return;
        m_EntryReferenceNumber = referenceNumber;
        if (m_GamePlaySetting.m_Notification == 0) return;

        m_CurrentLocalizeStringEvent.StringReference.TableReference = m_TableReference;
        m_CurrentLocalizeStringEvent.StringReference.TableEntryReference = m_TableEntryReference[referenceNumber];
        
        StopAllCoroutines();
        StartCoroutine(DisplayNotification(m_TextTime));
    }

    private IEnumerator DisplayNotification(float duringTime)
    {
        yield return FadeCanvas(0, 1);

        float elapsedTime = 0;
        while (elapsedTime <= duringTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return FadeCanvas(1, 0);
    }

    private IEnumerator FadeCanvas(float fromAlpha, float toAlpha)
    {
        float elapsedTime = 0;
        while (elapsedTime <= 1)
        {
            elapsedTime += Time.deltaTime;
            m_CanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime);
            yield return null;
        }
        m_CanvasGroup.alpha = toAlpha;
    }
}
