using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum SupportEventType
    {
        MaxStaminaUp,           //int
        MoveSpeedUp,            //float
        StaminaConsumeDown,     //int
        StaminaRecoverUp        //int
    }
    public class SupportSkill : SkillUp
    {
        [SerializeField] private SupportEventType m_SupportEventType;
        [SerializeField] private int m_Amount;

        protected override void DoSkillUp()
            => PlayerSkillReceiver.SupportSkillEvent(m_SupportEventType, m_Amount);
    }
}
