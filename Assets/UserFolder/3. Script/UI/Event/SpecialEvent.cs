using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum SpeciaEventType
    {
        GEConsumeDown,
        GEMaxUp,
        GERecoverUp,
        TEConsumeDown,
        TEMaxUp,
        TERecoverUp
    }

    public class SpecialEvent : SkillEvent
    {
        [SerializeField] private SpeciaEventType m_SpeciaEventType;
        [SerializeField] private int amount;

        public override void DoSkill()
            => PlayerSkillReceiver.SpecialSkillEvent(m_SpeciaEventType, amount);
    }
}
