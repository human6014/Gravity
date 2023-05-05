using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Event
{
    public enum EventType
    {
        GetWeapon,
        GetSupply,
        Attack,
        Defense,
        Special
    }

    public class SkillEvent : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private EventType m_EventType;
        [SerializeField] private int m_Index;
        [SerializeField] private int m_Rating;
        [SerializeField] private int m_Level;

        public UnityAction PointerDownAction { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDownAction?.Invoke();
            Debug.Log("Clicked");
        }
    }
}
