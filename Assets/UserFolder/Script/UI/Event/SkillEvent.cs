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

        private int m_CallingCount;
        private int m_PickCount;
        private int m_Level { get; set; }
        public UnityAction PointerDownAction { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDownAction?.Invoke();

            m_PickCount++;
            Debug.Log("Click");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
        }

        public virtual void Init()
        {
            m_CallingCount++;
            gameObject.SetActive(true);
        }

        public virtual void DoSkill()
        {

        }

        public virtual void Dispose()
        {
            gameObject.SetActive(false);
        }
    }
}
