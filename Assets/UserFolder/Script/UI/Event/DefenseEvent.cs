using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum DefenseEventType
    {
        GetHealKit,         //int
        HealKitRateUp,      //int
        HealthRecoverUp,    //int
        MaxHealthUp         //int
    }
    public class DefenseEvent : SkillEvent
    {
        [SerializeField] private DefenseEventType m_DefenseEventType;
        [SerializeField] private int m_Amount;

        public override void DoSkill()
        {
            PlayerSkillReceiver.DefenseSkillEvent(m_DefenseEventType, m_Amount);
        }
    }
}
