using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
{
    [CreateAssetMenu(fileName = "MeleeWeaponStatSetting", menuName = "Scriptable Object/MeleeWeaponStatSettings", order = int.MaxValue - 8)]
    public class MeleeWeaponStatScriptable : WeaponStatScriptable
    {
        [Space(15)]
        [Header("Child")]
        [Header("Attack Info")]
        [Tooltip("약공격 속도")]
        public float m_LightFireTime = 1f;

        [Tooltip("강공격 속도")]
        public float m_HeavyFireTime = 1.5f;

        [Header("SphereCast")]
        [Tooltip("공격 범위")]
        public float m_SwingRadius;

        [Tooltip("최대 사거리")]
        public float m_MaxDistance;

        [Tooltip("평타 캔슬 여부")]
        public bool m_CanComboAttack;

        [Tooltip("공격시 화면 흔들림 여부")]
        public bool m_DoShake;
    }
}
