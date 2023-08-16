using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SpecialMonster2Setting", menuName = "Scriptable Object/SpecialMonster2Settings", order = int.MaxValue - 1)]
    public class SpecialMonster2Scriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("���� Ÿ��")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Tooltip("���� ��ֹ� ���� ���̾�")]
        public LayerMask m_ObstacleDetectLayer;

        [Tooltip("���� ���� ���̾�")]
        public LayerMask m_AttackableLayer;

        [Header("Rush Attack")]
        [Tooltip("���� ���� �浹 ���̾�")]
        public LayerMask m_RushObstacleLayer;

        [Tooltip("���� ���� ������")]
        public int m_RushAttackDamage;

        [Tooltip("���� ���� ��Ÿ��")]
        public float m_RushAttackTime;

        [Tooltip("���� ���� �ּ� ��Ÿ�")]
        public float m_RushAttackMinRange;

        [Tooltip("���� ���� �ִ� ��Ÿ�")]
        public float m_RushAttackMaxRange;

        [Tooltip("���� ���� �̵� �ӵ�")]
        public float m_RushAttackMovementSpeed;

        [Header("Grab Attack")]
        [Tooltip("��� ���� ������")]
        public int m_GrabAttackDamage;

        [Tooltip("��� ���� ��Ÿ��")]
        public float m_GrabAttackTime;

        [Tooltip("��� ���� �ּ� ��Ÿ�")]
        public float m_GrabAttackMinRange;

        [Tooltip("��� ���� �ִ� ��Ÿ�")]
        public float m_GrabAttackMaxRange;

        [Tooltip("��� ���� ���� �Ÿ�")]
        public float m_GrabCancellationDist = 1;

        [Tooltip("��� ���� ���� ���� ������")]
        public int m_GrabCancellationDamage = 500;

        [Header("Ground down")]
        [Tooltip("")]
        public float m_GroundDownForceMultiplier = 5;

        [Tooltip("������ ���� �ִ� ��")]
        public float m_MaxGroundDownForce = 35;

        [Header("Hide and Recovery")]
        [Tooltip("�ߵ� ü�� ����")] [Range(0,1)]
        public float m_RecoveryTriggerHP = 0.4f;

        [Tooltip("ȸ�� �̵� �ӵ�")]
        public float m_RecoveryMovementSpeed = 12;

        [Tooltip("ȸ���� ����")] [Range(0,1)]
        public float m_RecoveryHPAmount = 0.4f;

        [Tooltip("ȸ�� �ð�")]
        public float m_RecoveryTime = 20;

        public bool CanNormalAttack(float dist, float curTimer) 
            => dist <= m_AttackRange && curTimer >= m_AttackSpeed;

        public bool CanRushAttack(float dist, float curTimer)
            => dist <= m_RushAttackMaxRange && dist >= m_RushAttackMinRange && curTimer >= m_RushAttackTime;

        public bool CanGrabAttack(float dist, float curTimer)
            => dist <= m_GrabAttackMaxRange && dist >= m_GrabAttackMinRange && curTimer >= m_GrabAttackTime;
    }
}
