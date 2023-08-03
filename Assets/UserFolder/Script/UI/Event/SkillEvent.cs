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
        [Tooltip("���콺�� �÷��� �� Ȱ��ȭ �� ������Ʈ")]
        [SerializeField] private GameObject m_OutlineUI;

        [Tooltip("��ų Ÿ��")]
        [SerializeField] private Scriptable.UI.EventType m_EventType;

        [Tooltip("���")]
        [SerializeField] private int m_Rating = 1;

        [Tooltip("��ų ��ȣ")]
        [SerializeField] private int m_Index;

        [Tooltip("�ִ� ����")]
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
