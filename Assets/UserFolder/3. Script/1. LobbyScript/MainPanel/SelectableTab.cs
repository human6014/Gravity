using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectableTab : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent m_OnClickEvnet;
    [SerializeField] private int m_SelectableCondition; //hard = 0 , extreme = 1

    private GamePlaySetting m_GamePlaySetting;

    private void Start()
    {
        m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(m_GamePlaySetting.m_HasHardClearData >= m_SelectableCondition)
            m_OnClickEvnet?.Invoke();
    }
}
