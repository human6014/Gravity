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

        [Tooltip("Boids ���� ���� �ð�")]
        [SerializeField] private float m_BoidsMonsterTraceSpeed = 20;

        [Tooltip("Boids ���� ���� ���� �ð�")]
        [SerializeField] private float m_BoidsMonsterTraceTime = 7.5f;

        [Tooltip("Boids �۶߸��� ���� �ð�")]
        [SerializeField] private float m_BoidsPatrolSpeed = 20;

        [Tooltip("Boids �۶߸��� ���� ���ӽð�")]
        [SerializeField] private float m_BoidsPatrolTime = 25;
    }
}
