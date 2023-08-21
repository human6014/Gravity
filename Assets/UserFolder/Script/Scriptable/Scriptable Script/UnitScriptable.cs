using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scriptable.Monster
{
    public class UnitScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Stat value")]
        [Tooltip("체력")]
        public int m_HP;

        [Tooltip("방어력")]
        public int m_Def;

        [Tooltip("일반 공격력")]
        public int m_Damage;

        [Tooltip("이동 속도")]
        public float m_MovementSpeed;

        [Tooltip("일반 공격 속도")]
        public float m_AttackSpeed;

        [Tooltip("일반 공격 사거리")]
        public float m_AttackRange;

        [Tooltip("일반 공격 가능한 각도")]
        public float m_AttackAbleAngle;

        [Tooltip("폭발 저항")]
        [Range(1,50)]
        public int m_ExplosionResistance = 1;

        [Tooltip("근접 무기 저항")]
        [Range(1, 50)]
        public int m_MeleeResistance = 1;

        [Tooltip("일반 공격 타입")]
        public AttackType m_NoramlAttackType = AttackType.None;

        [Header("Stat multiplier")]
        [Tooltip("체력 상승치")]
        public int m_HPMultiplier;

        [Tooltip("방어력 상승치")]
        public int m_DefMultiplier;

        [Tooltip("공격력 상승치")]
        public int m_DamageMultiplier;

        public bool CanNormalAttack(float dist, float curTimer, float angle)
            => dist <= m_AttackRange && curTimer >= m_AttackSpeed && angle <= m_AttackAbleAngle;

        public bool CanNormalAttack(float dist, float angle)
            => dist <= m_AttackRange && angle <= m_AttackAbleAngle;
    }
}
