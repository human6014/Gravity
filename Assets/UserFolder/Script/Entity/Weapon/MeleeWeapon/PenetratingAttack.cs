using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object.Weapon
{
    public class PenetratingAttack : Attackable
    {
        public override bool SwingCast()
        {
            RaycastHit[] hitInfo = Physics.SphereCastAll(m_CameraTransform.position, m_MeleeWeaponStat.m_SwingRadius, m_CameraTransform.forward, m_MeleeWeaponStat.m_MaxDistance, m_MeleeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore);

            bool isHit = false;
            bool doEffect = false;
            for (int i = 0; i < hitInfo.Length; i++)
            {
                isHit = base.ProcessEffect(ref hitInfo[i], ref doEffect) | isHit;
            }
            return isHit;
        }
    }
}