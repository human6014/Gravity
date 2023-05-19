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

        [Tooltip("공격 가능 레이어")]
        public LayerMask m_AttackableLayer;

        [Tooltip("직선 장애물 감지 레이어")]
        public LayerMask m_ObstacleDetectLayer;


        [Header("Grab Attack")]
        [Tooltip("잡기 공격 타입")]
        public AttackType m_GrabAttackType = AttackType.Grab;

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
        public float m_JumpAttackSpeed = 12;

        [Tooltip("도약 공격 최소 사거리")]
        public float m_JumpAttackMinRange = 25;

        [Tooltip("도약 공격 최대 사거리")]
        public float m_JumpAttackMaxRange = 50;

        [Tooltip("도약 공격 목표지점에서 차이값")]
        public float m_DestinationDist = 3.5f;

        [Tooltip("도약 공격 준비 시간")]
        public float m_PreJumpAttackTime = 0.7f;

        [Tooltip("도약 공격 높이 보정 값")][Range(1,20)]     //클수록 더 낮게 점프함
        public float m_JumpAttackHeightRatio = 11;

        [Tooltip("도약 공격 발동 확률")][Range(0, 100)]
        public float m_JumpAttackPercentage = 70;           //ex) 70퍼 확률로 점프 공격 수행


        [Header("Jump")]
        [Tooltip("일반 도약 속도")]
        public float m_JumpSpeed = 30;

        [Tooltip("일반 도약 최소 사거리")]
        public float m_JumpMinRange = 60;

        [Tooltip("일반 도약 최대 사거리")]
        public float m_JumpMaxRange = 130;

        [Tooltip("일반 도약 준비 시간")]
        public float m_PreJumpTime = 2;

        [Tooltip("일반 도약 높이 보정 값")][Range(1,20)]
        public float m_JumpHeightRatio = 2;

        [Tooltip("일반 도약 발동 확률")][Range(0, 100)]
        public float m_JumpPercentage = 60;


        [Header("Hit")]
        [Tooltip("피격 시 경직이 가능한 체력")]
        public float m_HitHP = 15000;

        [Tooltip("피격 시 경직을 일으킬 수 있는 데미지")]
        public float m_HitDamage = 900;

        [Tooltip("피격 시 경직 발동 확률")] [Range(0,100)]
        public float m_HitPercentage = 25;

        [Tooltip("체력별 BaseColor")]
        public Color m_MaxInjuryColor = new Color(255, 180, 180);

        public bool CanJumpAttack(float dist, float curTimer)
            => dist > m_JumpAttackMinRange && dist < m_JumpAttackMaxRange && curTimer >= m_JumpAttackSpeed;

        public bool CanJump(float dist, float curTimer)
            => dist > m_JumpMinRange && dist < m_JumpMaxRange && curTimer >= m_JumpSpeed;

        public bool CanJumpPercentage() => Random.Range(0, 100) <= m_JumpPercentage;

        public bool CanJumpAttackPercentage() => Random.Range(0, 100) <= m_JumpAttackPercentage;
    }
}
