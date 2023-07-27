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

        [Tooltip("Boids 추적 공격 시간")]
        [SerializeField] private float m_BoidsMonsterTraceSpeed = 20;

        [Tooltip("Boids 추적 공격 지속 시간")]
        [SerializeField] private float m_BoidsMonsterTraceTime = 7.5f;

        [Tooltip("Boids 퍼뜨리기 공격 시간")]
        [SerializeField] private float m_BoidsPatrolSpeed = 20;

        [Tooltip("Boids 퍼뜨리기 공격 지속시간")]
        [SerializeField] private float m_BoidsPatrolTime = 25;
    }
}
