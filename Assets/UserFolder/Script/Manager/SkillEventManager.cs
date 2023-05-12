using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Event;

namespace UI.Manager
{
    public class SkillEventManager : MonoBehaviour
    {
        [SerializeField] private SettingUIManager m_SettingUIManager;
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private RectTransform m_SkillPos;
        [SerializeField] private SkillEvent[] m_SkillEvents;

        private Animator m_Animator;

        private int m_EventCount = 0;

        private void Awake()
        {
            m_Animator = m_SkillPos.GetComponent<Animator>();
        }

        [ContextMenu("OccurSkillEvent")]
        public void OccurSkillEvent()
        {
            m_SkillPos.gameObject.SetActive(true);
            m_Animator.SetTrigger("Show");
            m_SettingUIManager.PauseMode(true);
            m_EventCount++;
        }
        [ContextMenu("EndSkillEvent")]
        public void EndSkillEvent()
        {
            m_Animator.SetTrigger("Hide");
            //m_SkillPos.gameObject.SetActive(false);
            m_SettingUIManager.PauseMode(false);
        }
    }
}
