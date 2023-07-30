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

        [Tooltip("Boids �Ϻ� ���� ���� �ð�")]
        public float m_BoidsMonsterAttackSpeed = 10;

        [Tooltip("Boids �Ϻ� ���� ���� ���� �ð�")]
        public float m_BoidsMonsterAttackTime = float.MaxValue;

        [Tooltip("Boids ��ü ���� ���� �ð�")]
        public float m_BoidsMonsterTraceSpeed = 20;

        [Tooltip("Boids ��ü ���� ���� ���� �ð�")]
        public float m_BoidsMonsterTraceTime = 7.5f;

        [Tooltip("Boids �۶߸��� ���� �ð�")]
        public float m_BoidsPatrolSpeed = 20;

        [Tooltip("Boids �۶߸��� ���� ���ӽð�")]
        public float m_BoidsPatrolTime = 25;
    }
}
