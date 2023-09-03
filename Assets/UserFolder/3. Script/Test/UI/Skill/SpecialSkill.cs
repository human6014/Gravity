using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public class SpecialSkill : SkillUp
    {
        [SerializeField] private SpeciaEventType m_SpeciaEventType;
        [SerializeField] private int amount;

        protected override void DoSkillUp()
            => PlayerSkillReceiver.SpecialSkillEvent(m_SpeciaEventType, amount);
    }
}
