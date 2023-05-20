using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Event;

namespace Scriptable.UI
{
    public enum EventType
    {
        GetWeapon,
        GetSupply,
        Attack,
        Specific,
        Defense,
        Support,
        Special
    }

    [CreateAssetMenu(fileName = "SkillEventSetting", menuName = "Scriptable Object/SkillEventSettings", order = int.MaxValue - 14)]
    public class SkillEventScriptable : ScriptableObject
    {
        [Header("Visible")]
        [Tooltip("상단 아이콘")]
        public Sprite m_Sprite;

        [Tooltip("텍스트 설명")]
        [TextArea]
        public string m_Text;


        [Header("Non Visible")]
        [Tooltip("스킬 타입")]
        public EventType m_EventType;

        [Tooltip("등급")]
        public int m_Rating;

        [Tooltip("레벨")]
        public int m_Level;


    }
}
