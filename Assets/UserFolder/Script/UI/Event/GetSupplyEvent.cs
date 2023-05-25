using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public class GetSupplyEvent : SkillEvent
    {
        [SerializeField] private PlayerData m_PlayerData;

        [SerializeField] private int m_SlotNumber;
        [SerializeField] private int m_WeaponIndex;

        public override void Init()
        {
            base.Init();


        }

        public override void DoSkill()
        {
            base.DoSkill();


        }
    }
}
