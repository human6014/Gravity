using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public class GetSupplyEvent : SkillEvent
    {
        [Header("Child")]
        [SerializeField] private int m_SlotNumber;
        [SerializeField] private int m_BulletAmount;

        public override void DoSkill()
        {
            PlayerSkillReceiver.GetSupplyEvent(m_SlotNumber, m_BulletAmount);
        }
    }
}
