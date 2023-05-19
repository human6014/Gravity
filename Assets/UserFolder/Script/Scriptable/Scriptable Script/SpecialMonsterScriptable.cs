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

        [Tooltip("���� ���� ���̾�")]
        public LayerMask m_AttackableLayer;

        [Tooltip("���� ��ֹ� ���� ���̾�")]
        public LayerMask m_ObstacleDetectLayer;


        [Header("Grab Attack")]
        [Tooltip("��� ���� Ÿ��")]
        public AttackType m_GrabAttackType = AttackType.Grab;

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
        public float m_JumpAttackSpeed = 12;

        [Tooltip("���� ���� �ּ� ��Ÿ�")]
        public float m_JumpAttackMinRange = 25;

        [Tooltip("���� ���� �ִ� ��Ÿ�")]
        public float m_JumpAttackMaxRange = 50;

        [Tooltip("���� ���� ��ǥ�������� ���̰�")]
        public float m_DestinationDist = 3.5f;

        [Tooltip("���� ���� �غ� �ð�")]
        public float m_PreJumpAttackTime = 0.7f;

        [Tooltip("���� ���� ���� ���� ��")][Range(1,20)]     //Ŭ���� �� ���� ������
        public float m_JumpAttackHeightRatio = 11;

        [Tooltip("���� ���� �ߵ� Ȯ��")][Range(0, 100)]
        public float m_JumpAttackPercentage = 70;           //ex) 70�� Ȯ���� ���� ���� ����


        [Header("Jump")]
        [Tooltip("�Ϲ� ���� �ӵ�")]
        public float m_JumpSpeed = 30;

        [Tooltip("�Ϲ� ���� �ּ� ��Ÿ�")]
        public float m_JumpMinRange = 60;

        [Tooltip("�Ϲ� ���� �ִ� ��Ÿ�")]
        public float m_JumpMaxRange = 130;

        [Tooltip("�Ϲ� ���� �غ� �ð�")]
        public float m_PreJumpTime = 2;

        [Tooltip("�Ϲ� ���� ���� ���� ��")][Range(1,20)]
        public float m_JumpHeightRatio = 2;

        [Tooltip("�Ϲ� ���� �ߵ� Ȯ��")][Range(0, 100)]
        public float m_JumpPercentage = 60;


        [Header("Hit")]
        [Tooltip("�ǰ� �� ������ ������ ü��")]
        public float m_HitHP = 15000;

        [Tooltip("�ǰ� �� ������ ����ų �� �ִ� ������")]
        public float m_HitDamage = 900;

        [Tooltip("�ǰ� �� ���� �ߵ� Ȯ��")] [Range(0,100)]
        public float m_HitPercentage = 25;

        [Tooltip("ü�º� BaseColor")]
        public Color m_MaxInjuryColor = new Color(255, 180, 180);

        public bool CanJumpAttack(float dist, float curTimer)
            => dist > m_JumpAttackMinRange && dist < m_JumpAttackMaxRange && curTimer >= m_JumpAttackSpeed;

        public bool CanJump(float dist, float curTimer)
            => dist > m_JumpMinRange && dist < m_JumpMaxRange && curTimer >= m_JumpSpeed;

        public bool CanJumpPercentage() => Random.Range(0, 100) <= m_JumpPercentage;

        public bool CanJumpAttackPercentage() => Random.Range(0, 100) <= m_JumpAttackPercentage;
    }
}
