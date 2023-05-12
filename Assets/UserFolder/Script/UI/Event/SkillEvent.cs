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
    [ExecuteInEditMode]
    public class SkillEvent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private EventType m_EventType;
        [SerializeField] private int m_Index;
        [SerializeField] private int m_Rating;
        [SerializeField] private int m_Level;

        public UnityAction PointerDownAction { get; set; }
        public UnityAction PointerEnterAction { get; set; }
        public UnityAction PointerExitAction { get; set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDownAction?.Invoke();
            Debug.Log("Click");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnterAction?.Invoke();
            Debug.Log("Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
        }
    }
}
