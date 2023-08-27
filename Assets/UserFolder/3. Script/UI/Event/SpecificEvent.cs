using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum SpecificEventType
    {

    }

    public class SpecificEvent : SkillEvent
    {
        [SerializeField] private SpecificEventType m_SpecificEventType;
        [SerializeField] private int amount;

        public override void DoSkill()
            => PlayerSkillReceiver.SpecificSkillEvent(m_SpecificEventType, amount);
    }
}
