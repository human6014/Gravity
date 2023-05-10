using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object.Weapon
{
    public class NonPenetratingAttack : Attackable
    {
        public override bool SwingCast()
        {
            bool doEffect = false;
            if (Physics.SphereCast(m_CameraTransform.position, m_MeleeWeaponStat.m_SwingRadius, m_CameraTransform.forward, out RaycastHit hit, m_MeleeWeaponStat.m_MaxDistance, m_MeleeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                return base.ProcessEffect(ref hit, ref doEffect);
            }
            return false;
        }
    }
}
