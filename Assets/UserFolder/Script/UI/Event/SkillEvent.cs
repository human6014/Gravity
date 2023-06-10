using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Manager.AI;

namespace UI.Event
{
    public abstract class SkillEvent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
        public event System.Action PointerDownAction;

        public int Level 
        {
            get => m_CurrentLevel;
            private set { m_CurrentLevel = Mathf.Clamp(value, 1, m_MaxLevel); }
        }
        public Scriptable.UI.EventType EventType { get; }
        protected PlayerSkillReceiver m_PlayerSkillReceiver { get; private set; }
        

        #region PointerHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDownAction?.Invoke();
            Level = m_CurrentLevel + 1;
            //Debug.Log(Level);
        }

        public void OnPointerEnter(PointerEventData eventData) 
            => m_OutlineUI.SetActive(true);
        

        public void OnPointerExit(PointerEventData eventData) 
            => m_OutlineUI.SetActive(false);
        
        #endregion

        private void Awake() => PointerDownAction += DoSkill;
        
        private void Start()
        {
            m_PlayerSkillReceiver = AIManager.PlayerTransform.GetComponent<PlayerSkillReceiver>();
        }

        public virtual void Init()
        {
            m_CallingCount++;
            gameObject.SetActive(true);
        }

        public abstract void DoSkill();

        public virtual void Dispose()
        {
            gameObject.SetActive(false);
        }
    }
}
