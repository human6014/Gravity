using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Event;

namespace UI.Manager
{
    [System.Serializable]
    public struct FixedNonSpecificEvent
    {
        public int m_EventCount;
        public Scriptable.UI.EventType m_EventType;
    }

    [System.Serializable]
    public struct FixedSpecificEvent
    {
        public int m_EventCount;
        public Event.SkillEvent[] m_SkillEvent;
    }

    [System.Serializable]
    public class SkillEvent
    {
        public Scriptable.UI.EventType m_EventType;
        public Event.SkillEvent[] m_SkillEvent;
    }

    public class SkillEventManager : MonoBehaviour
    {
        [SerializeField] private SettingUIManager m_SettingUIManager;
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private RectTransform m_SkillPos;


        [SerializeField] private FixedNonSpecificEvent[] m_FixedNonSpecificEvents;
        [SerializeField] private FixedSpecificEvent[] m_FixedSpecificEvents;
        //위 둘 타이밍 안곂쳐야 함

        [SerializeField] private SkillEvent[] m_SkillEvent;

        private List<int> m_FixedEventTiming;

        private Animator m_Animator;

        private int m_EventCount = 0;
        private int [] m_SkillLevel;

        private void Awake()
        {
            m_Animator = m_SkillPos.GetComponent<Animator>();
            SetFixedEvent();
        }

        private void SetFixedEvent()
        {
            m_FixedEventTiming = new List<int>();

            foreach (FixedNonSpecificEvent fse in m_FixedNonSpecificEvents)
                m_FixedEventTiming.Add(fse.m_EventCount);

            foreach (FixedSpecificEvent fse in m_FixedSpecificEvents)
                m_FixedEventTiming.Add(fse.m_EventCount);

            m_FixedEventTiming.Sort();
        }

        [ContextMenu("OccurSkillEvent")]
        public void OccurSkillEvent()
        {
            m_SkillPos.gameObject.SetActive(true);
            m_Animator.SetTrigger("Show");
            m_SettingUIManager.PauseMode(true);
            m_EventCount++;

            BatchSkillUI();
        }

        private void BatchSkillUI()
        {

        }

        [ContextMenu("EndSkillEvent")]
        public void EndSkillEvent()
        {
            m_Animator.SetTrigger("Hide");
            m_SettingUIManager.PauseMode(false);
        }

        public void EndHideAnimation() => m_SkillPos.gameObject.SetActive(false);
        
    }
}
