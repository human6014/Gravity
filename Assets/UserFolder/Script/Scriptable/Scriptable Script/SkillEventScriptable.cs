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
        Defense,
        Support,
        Specific,
        Special
    }

    [CreateAssetMenu(fileName = "SkillEventSetting", menuName = "Scriptable Object/SkillEventSettings", order = int.MaxValue - 14)]
    public class SkillEventScriptable : ScriptableObject
    {
        [Header("Visible")]
        [Tooltip("��� ������")]
        public Sprite m_Sprite;

        [Tooltip("�ؽ�Ʈ ����")]
        [TextArea]
        public string m_Text;


        [Header("Non Visible")]
        [Tooltip("��ų Ÿ��")]
        public EventType m_EventType;

        [Tooltip("���")]
        public int m_Rating = 1;

        [Tooltip("����")]
        public int m_Level = 1;
    }
}
