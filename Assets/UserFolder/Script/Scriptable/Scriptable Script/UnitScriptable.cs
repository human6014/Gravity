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

        [Tooltip("���ݷ�")]
        public int m_Damage;

        [Tooltip("�̵� �ӵ�")]
        public float m_MovementSpeed;

        [Tooltip("���� �ӵ�")]
        public float m_AttackSpeed;

        [Tooltip("���� ��Ÿ�")]
        public float m_AttackRange;

        [Tooltip("���� ����")]
        [Range(1,50)]
        public int m_ExplosionResistance = 1;

        [Tooltip("���� ���� ����")]
        [Range(1, 50)]
        public int m_MeleeResistance = 1;

        [Tooltip("���� Ÿ��")]
        public AttackType m_NoramlAttackType = AttackType.None;

        [Tooltip("Ư�� ���� Ÿ��")]
        public AttackType m_GrabAttack = AttackType.Grab;
    }
}
