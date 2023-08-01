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
        [Tooltip("���� Ÿ��")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Tooltip("���� ��ֹ� ���� ���̾�")]
        public LayerMask m_ObstacleDetectLayer;

        [Tooltip("���� Boids ���� ��")] [Range(0,1000)]
        public int m_BoidsSpawnCount = 450;

        [Tooltip("���� Boids ���� �� ")] [Range(0, 1000)]
        public int m_BoidsRespawnCount = 250;

        [Header("Attack")]
        [Tooltip("Boids �Ϻ� ���� ���� �ð�(��Ÿ��)")]
        public float m_BoidsMonsterAttackSpeed = 15;


        [Header("Trace All")]
        [Tooltip("Boids ��ü ���� ���� �ð�(��Ÿ��)")]
        public float m_BoidsMonsterTraceAndBackSpeed = 20;

        [Tooltip("Boids ��ü ���� ���� ���� �ð�")]
        public float m_BoidsMonsterTraceTime = 7.5f;

        [Tooltip("Boids ��ü ���� ���� �ߵ� Ȯ��")]
        [Range(0, 100)]
        public float m_BoidsTraceAndBackPercentage = 60;


        [Header("Spread Patrol")]
        [Tooltip("Boids �۶߸��� ���� �ð�(��Ÿ��)")]
        public float m_BoidsPatrolSpeed = 30;

        [Tooltip("Boids �۶߸��� ���� ���ӽð�")]
        public float m_BoidsPatrolTime = 25;

        [Tooltip("Boids �۶߸��� �ߵ� Ȯ��")]
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
