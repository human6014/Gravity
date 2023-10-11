using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameClearUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CountText;
    [SerializeField] private float m_FadeInTime = 1;
    [SerializeField] private float m_LastCount;

    [SerializeField] private UnityEvent CountOnEvent;

    private CanvasGroup m_CanvasGroup;

    private void Awake()
        => m_CanvasGroup = GetComponent<CanvasGroup>();
    
    public void SetGameClearEnd()
    {
        gameObject.SetActive(true);

        if (DataManager.Instance != null)
        {
            GamePlaySetting gamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            gamePlaySetting.m_HasHardClearData = 1;
            gamePlaySetting.SaveData();
        }
        StartCoroutine(FadeInCoroutine());
        StartCoroutine(CountDown());
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0;
        while (elapsedTime < m_FadeInTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            m_CanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / m_FadeInTime);
            yield return null;
        }
    }

    private IEnumerator CountDown()
    {
        while (m_LastCount >= 0)
        {
            m_LastCount -= Time.deltaTime;
            m_CountText.text = Mathf.CeilToInt(m_LastCount).ToString();

            yield return null;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        CountOnEvent?.Invoke();
    }
}
