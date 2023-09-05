using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Event
{
    public enum SpecificEventType
    {

    }
    public class SpecificSkill : SkillUp
    {
        [SerializeField] private SpecificEventType m_SupportEventType;
        [SerializeField] private int m_Amount;

        protected override void DoSkillUp()
        {
            
        }
    }
}
