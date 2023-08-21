using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scriptable.Monster
{
    public class UnitScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Stat value")]
        [Tooltip("ü��")]
        public int m_HP;

        [Tooltip("����")]
        public int m_Def;

        [Tooltip("�Ϲ� ���ݷ�")]
        public int m_Damage;

        [Tooltip("�̵� �ӵ�")]
        public float m_MovementSpeed;

        [Tooltip("�Ϲ� ���� �ӵ�")]
        public float m_AttackSpeed;

        [Tooltip("�Ϲ� ���� ��Ÿ�")]
        public float m_AttackRange;

        [Tooltip("�Ϲ� ���� ������ ����")]
        public float m_AttackAbleAngle;

        [Tooltip("���� ����")]
        [Range(1,50)]
        public int m_ExplosionResistance = 1;

        [Tooltip("���� ���� ����")]
        [Range(1, 50)]
        public int m_MeleeResistance = 1;

        [Tooltip("�Ϲ� ���� Ÿ��")]
        public AttackType m_NoramlAttackType = AttackType.None;

        [Header("Stat multiplier")]
        [Tooltip("ü�� ���ġ")]
        public int m_HPMultiplier;

        [Tooltip("���� ���ġ")]
        public int m_DefMultiplier;

        [Tooltip("���ݷ� ���ġ")]
        public int m_DamageMultiplier;

        public bool CanNormalAttack(float dist, float curTimer, float angle)
            => dist <= m_AttackRange && curTimer >= m_AttackSpeed && angle <= m_AttackAbleAngle;

        public bool CanNormalAttack(float dist, float angle)
            => dist <= m_AttackRange && angle <= m_AttackAbleAngle;
    }
}
