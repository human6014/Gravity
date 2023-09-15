using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeySetEvent : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int m_KeyIndex;
    [SerializeField] private UnityEvent<int> m_KeySettingEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_KeySettingEvent?.Invoke(m_KeyIndex);
    }
}
