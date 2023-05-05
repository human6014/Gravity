using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SkillEventManager : MonoBehaviour
    {
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private RectTransform m_SkillPos;
        [SerializeField] private UI.Event.SkillEvent[] m_SkillEvents;

        private int m_EventCount = 0;

        private void Awake()
        {

        }

        public void OccurSkillEvent()
        {
            m_EventCount++;
        }
    }
}
