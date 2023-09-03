using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public class DefenseSkill : SkillUp
    {
        [SerializeField] private DefenseEventType m_DefenseEventType;
        [SerializeField] private int m_Amount;

        protected override void DoSkillUp()
            => PlayerSkillReceiver.DefenseSkillEvent(m_DefenseEventType, m_Amount);
        
    }
}
