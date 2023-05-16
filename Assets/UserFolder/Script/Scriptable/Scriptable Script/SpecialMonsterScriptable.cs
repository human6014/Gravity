using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SpecialMonsterSetting", menuName = "Scriptable Object/SpecialMonsterSettings", order = int.MaxValue - 1)]
    public class SpecialMonsterScriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("���� Ÿ��")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Header("Grab Attack")]
        [Tooltip("��� ���� Ÿ��")]
        public AttackType m_GrabAttack = AttackType.Grab;

        [Tooltip("��� ���� ������")]
        public int m_GrabAttackDamage = 500;

        [Tooltip("��� ���� �ӵ�")]
        public float m_GrabAttackSpeed = 9;

        [Tooltip("��� ���� ��Ÿ�")]
        public float m_GrabAttackRange = 7;


        [Header("Jump Attack")]
        [Tooltip("���� ���� ������")]
        public int m_JumpAttackDamage = 350;

        [Tooltip("���� ���� ���� ����")]
        public float m_JumpAttackRange = 2;

        [Tooltip("���� ���� �ӵ�")]
        public float m_JumpAttackSpeed = 15;

        [Tooltip("���� ���� �ּ� ��Ÿ�")]
        public float m_JumpAttackMinRange = 15;

        [Tooltip("���� ���� �ִ� ��Ÿ�")]
        public float m_JumpAttackMaxRange = 35;

        [Tooltip("���� ���� ��ǥ�������� ���̰�")]
        public float m_DestinationDist = 2;

        [Header("Jump")]
        [Tooltip("�Ϲ� ���� �ӵ�")]
        public float m_JumpSpeed = 30;

        [Tooltip("�Ϲ� ���� �ּ� ��Ÿ�")]
        public float m_JumpMinRange = 40;

        [Tooltip("�Ϲ� ���� �ִ� ��Ÿ�")]
        public float m_JumpMaxRange = 120;

        public bool CanJumpBiteAttack(float dist, float curTimer)
            => dist > m_JumpAttackMinRange && dist < m_JumpAttackMaxRange && curTimer >= m_JumpAttackSpeed;

        public bool CanJump(float dist, float curTimer)
            => dist > m_JumpMinRange && dist < m_JumpMaxRange && curTimer >= m_JumpSpeed;
    }
}
