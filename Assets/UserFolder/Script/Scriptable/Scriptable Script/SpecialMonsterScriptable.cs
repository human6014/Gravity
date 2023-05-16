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
        [Tooltip("몬스터 타입")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Header("Grab Attack")]
        [Tooltip("잡기 공격 타입")]
        public AttackType m_GrabAttack = AttackType.Grab;

        [Tooltip("잡기 공격 데미지")]
        public int m_GrabAttackDamage = 500;

        [Tooltip("잡기 공격 속도")]
        public float m_GrabAttackSpeed = 9;

        [Tooltip("잡기 공격 사거리")]
        public float m_GrabAttackRange = 7;


        [Header("Jump Attack")]
        [Tooltip("도약 공격 데미지")]
        public int m_JumpAttackDamage = 350;

        [Tooltip("도약 공격 판정 범위")]
        public float m_JumpAttackRange = 2;

        [Tooltip("도약 공격 속도")]
        public float m_JumpAttackSpeed = 15;

        [Tooltip("도약 공격 최소 사거리")]
        public float m_JumpAttackMinRange = 15;

        [Tooltip("도약 공격 최대 사거리")]
        public float m_JumpAttackMaxRange = 35;

        [Tooltip("도약 공격 목표지점에서 차이값")]
        public float m_DestinationDist = 2;

        [Header("Jump")]
        [Tooltip("일반 도약 속도")]
        public float m_JumpSpeed = 30;

        [Tooltip("일반 도약 최소 사거리")]
        public float m_JumpMinRange = 40;

        [Tooltip("일반 도약 최대 사거리")]
        public float m_JumpMaxRange = 120;

        public bool CanJumpBiteAttack(float dist, float curTimer)
            => dist > m_JumpAttackMinRange && dist < m_JumpAttackMaxRange && curTimer >= m_JumpAttackSpeed;

        public bool CanJump(float dist, float curTimer)
            => dist > m_JumpMinRange && dist < m_JumpMaxRange && curTimer >= m_JumpSpeed;
    }
}
