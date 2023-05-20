using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Event
{
    public class SkillEvent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Scriptable.UI.SkillEventScriptable m_SkillEventScriptable;
        [SerializeField] private int m_Index;

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
            Init();
            Debug.Log("Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
        }

        public virtual void Init()
        {

        }

        public virtual void DoSkill()
        {

        }
    }
}
