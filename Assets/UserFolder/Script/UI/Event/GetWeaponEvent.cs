using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public class GetWeaponEvent : SkillEvent
    {
        [Header("Child")]
        [SerializeField] private int m_SlotNumber;
        [SerializeField] private int m_WeaponIndex;

        public override void DoSkill()
        {
            m_PlayerSkillReceiver.GetWeaponEvent(m_SlotNumber, m_WeaponIndex);
        }
    }
}
