using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game
{
    public enum EventType
    {

    }

    public class SkillEvent : MonoBehaviour
    {
        [SerializeField] private EventType m_EventType;
        [SerializeField] private int m_Index;
        [SerializeField] private int m_Rating;
        [SerializeField] private int m_Level;
    }
}
