using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SpecialMonster2Setting", menuName = "Scriptable Object/SpecialMonster2Settings", order = int.MaxValue - 1)]
    public class SpecialMonster2Scriptable : UnitScriptable
    {
        //Hitbox Tentacle제외하고 30개
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("몬스터 타입")]
        public SpecialMonsterType m_MonsterType;

        [Tooltip("직선 장애물 감지 레이어")]
        public LayerMask m_ObstacleDetectLayer;

        [Tooltip("공격 가능 레이어")]
        public LayerMask m_AttackableLayer;

        [Tooltip("제자리 돌기 애니메이션 가능 각도")]
        public float m_SelfRotateAbleAngle = 5;

        [Header("Rush Attack")]
        [Tooltip("돌진 공격 충돌 레이어")]
        public LayerMask m_RushObstacleLayer;

        [Tooltip("돌진 공격 데미지")]
        public int m_RushAttackDamage;

        [Tooltip("돌진 공격 쿨타임")]
        public float m_RushAttackTime;

        [Tooltip("돌진 공격 최소 사거리")]
        public float m_RushAttackMinRange;

        [Tooltip("돌진 공격 최대 사거리")]
        public float m_RushAttackMaxRange;

        [Tooltip("돌진 공격 이동 속도")]
        public float m_RushAttackMovementSpeed;

        [Tooltip("돌진 공격 가능 각도")]
        public float m_RushAttackAbleAngle = 7.5f;

        [Tooltip("돌긴 공격 레이 Radius")]
        public float m_RushCheckRadius = 3.5f;

        [Header("Grab Attack")]
        [Tooltip("잡기 공격 데미지")]
        public int m_GrabAttackDamage;

        [Tooltip("잡기 공격 쿨타임")]
        public float m_GrabAttackTime;

        [Tooltip("잡기 공격 최소 사거리")]
        public float m_GrabAttackMinRange;

        [Tooltip("잡기 공격 최대 사거리")]
        public float m_GrabAttackMaxRange;

        [Tooltip("잡기 공격 해제 거리")]
        public float m_GrabCancellationDist = 3;

        [Tooltip("잡기 공격 플레이어 도달 시간")]
        public float m_GrabAttachedTime = 1;

        [Tooltip("잡기 공격 당기는 힘")]
        public float m_GrabForce = 30;

        [Tooltip("잡기 공격가능 각도")]
        public float m_GrabAttackAbleAngle = 15;

        [Tooltip("잡기 공격 해제 누적 데미지")]
        public int m_GrabCancellationDamage = 500;


        [Header("Ground down")]
        [Tooltip("")]
        public float m_GroundDownForceMultiplier = 5;

        [Tooltip("땅으로 끌기 최대 힘")]
        public float m_MaxGroundDownForce = 35;

        [Header("Hide and Recovery")]
        [Tooltip("발동 체력 비율")] [Range(0,1)]
        public float m_RecoveryTriggerHP = 0.4f;

        [Tooltip("회복 이동 속도")]
        public float m_RecoveryMovementSpeed = 12;

        [Tooltip("회복 회전 속도")]
        public float m_RecoveryAngularSpeed = 120;

        [Tooltip("회복량 비율")] [Range(0,1)]
        public float m_RecoveryHPAmount = 0.4f;

        [Tooltip("회복 시간")]
        public float m_RecoveryTime = 20;

        public bool CanRushAttack(float dist, float curTimer, float angle)
            => dist <= m_RushAttackMaxRange && dist >= m_RushAttackMinRange && 
            curTimer >= m_RushAttackTime && angle <= m_RushAttackAbleAngle;

        public bool CanGrabAttack(float dist, float curTimer, float angle)
            => dist <= m_GrabAttackMaxRange && dist >= m_GrabAttackMinRange && 
            curTimer >= m_GrabAttackTime && angle <= m_GrabAttackAbleAngle;
    }
}
