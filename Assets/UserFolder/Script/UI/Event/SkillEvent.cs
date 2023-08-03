using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Event
{
    public abstract class SkillEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Parent")]
        [Tooltip("마우스를 올렸을 때 활성화 할 오브젝트")]
        [SerializeField] private GameObject m_OutlineUI;

        [Tooltip("스킬 타입")]
        [SerializeField] private Scriptable.UI.EventType m_EventType;

        [Tooltip("등급")]
        [SerializeField] private int m_Rating = 1;

        [Tooltip("스킬 번호")]
        [SerializeField] private int m_Index;

        [Tooltip("최대 레벨")]
        [SerializeField] private int m_MaxLevel = 3;

        private int m_CurrentLevel = 1;
        private int m_CallingCount = 0;
        public event System.Action PointerClickAction;

        public bool IsActivePointer { get; set; }
        public int CurrentLevel 
        {
            get => m_CurrentLevel;
            private set { m_CurrentLevel = Mathf.Clamp(value, 1, m_MaxLevel); }
        }
        public Scriptable.UI.EventType EventType { get; }
        protected PlayerSkillReceiver PlayerSkillReceiver { get; private set; }

        #region PointerHandler
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActivePointer) return;
            PointerClickAction?.Invoke();
            CurrentLevel++;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActivePointer) return;
            m_OutlineUI.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActivePointer) return;
            m_OutlineUI.SetActive(false);
        }

        #endregion

        private void Awake() 
        {
            PointerClickAction += DoSkill;
            PlayerSkillReceiver = FindObjectOfType<PlayerSkillReceiver>();
        }

        public virtual void Init()
        {
            m_CallingCount++;
            IsActivePointer = true;
            m_OutlineUI.SetActive(false);
            gameObject.SetActive(true);
        }

        public abstract void DoSkill();

        public virtual void Dispose()
        {
            PointerClickAction = null;
            gameObject.SetActive(false);
        }
    }
}
