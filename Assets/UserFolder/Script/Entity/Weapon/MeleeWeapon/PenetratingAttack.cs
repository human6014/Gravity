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
                RaycastHit hit = hitInfo[i];
                isHit = base.ProcessEffect(ref hit, ref doEffect) | isHit;
            }
            return isHit;
        }
    }
}

// Apply an impact impulse
//if (hitInfo.rigidbody != null)
//    hitInfo.rigidbody.AddForceAtPosition(itemUseRays.direction * swing.HitImpact, hitInfo.point, ForceMode.Impulse);
