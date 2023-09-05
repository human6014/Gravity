using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum AttackEventType
    {
        AttackDamageUp, //int
        AttackSpeedUp,  //float
        MaxBulletUp,    //int
        ReloadSpeedUp,  //float
    }

    public class AttackSkill : SkillUp
    {
        [SerializeField] private AttackEventType m_AttackEventType;
        [SerializeField] private float m_Amount;

        protected override void DoSkillUp()
            => PlayerSkillReceiver.AttackSkillEvent(m_AttackEventType, m_Amount);
    }
}
