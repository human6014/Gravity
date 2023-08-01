using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SpecialMonster3Setting", menuName = "Scriptable Object/SpecialMonster3Settings", order = int.MaxValue - 1)]
    public class SpecialMonster3Scriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("몬스터 타입")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Tooltip("직선 장애물 감지 레이어")]
        public LayerMask m_ObstacleDetectLayer;

        [Tooltip("최초 Boids 생성 수")] [Range(0,1000)]
        public int m_BoidsSpawnCount = 450;

        [Tooltip("반피 Boids 생성 수 ")] [Range(0, 1000)]
        public int m_BoidsRespawnCount = 250;

        [Header("Attack")]
        [Tooltip("Boids 일부 추적 공격 시간(쿨타임)")]
        public float m_BoidsMonsterAttackSpeed = 15;


        [Header("Trace All")]
        [Tooltip("Boids 전체 추적 공격 시간(쿨타임)")]
        public float m_BoidsMonsterTraceAndBackSpeed = 20;

        [Tooltip("Boids 전체 추적 공격 지속 시간")]
        public float m_BoidsMonsterTraceTime = 7.5f;

        [Tooltip("Boids 전체 추적 공격 발동 확률")]
        [Range(0, 100)]
        public float m_BoidsTraceAndBackPercentage = 60;


        [Header("Spread Patrol")]
        [Tooltip("Boids 퍼뜨리기 공격 시간(쿨타임)")]
        public float m_BoidsPatrolSpeed = 30;

        [Tooltip("Boids 퍼뜨리기 공격 지속시간")]
        public float m_BoidsPatrolTime = 25;

        [Tooltip("Boids 퍼뜨리기 발동 확률")]
        [Range(0, 100)]
        public float m_BoidsPatrolPercentage = 70;

        public bool CanBoidsTraceAndBackTime(float timer)
            => m_BoidsMonsterTraceAndBackSpeed <= timer;

        public bool CanBoidsTraceAndBackPercentage() 
            => Random.Range(0, 100) <= m_BoidsTraceAndBackPercentage;

        public bool CanBoidsPatrolTime(float timer) 
            => m_BoidsPatrolSpeed <= timer;

        public bool CanBoidsPatrolPercentage() 
            => Random.Range(0, 100) <= m_BoidsPatrolPercentage;
    }
}
