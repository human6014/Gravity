using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SkillEventManager : MonoBehaviour
    {
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private RectTransform m_SkillPos;
        [SerializeField] private UI.Game.SkillEvent[] m_SkillEvents;

        public void OccurSkillEvent()
        {

        }
    }
}
