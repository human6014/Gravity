using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameClearUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CountText;

    [SerializeField] private float m_LastCount;

    [SerializeField] private UnityEvent CountOnEvent;

    [ContextMenu("SetGameClear")]
    public void SetGameClear()
    {
        gameObject.SetActive(true);
        if (DataManager.Instance != null)
        {
            GamePlaySetting gamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            gamePlaySetting.m_HasHardClearData = 1;
            gamePlaySetting.SaveData();
        }

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (m_LastCount >= 0)
        {
            m_LastCount -= Time.deltaTime;
            m_CountText.text = Mathf.CeilToInt(m_LastCount).ToString();

            yield return null;
        }

        CountOnEvent?.Invoke();
    }
}
