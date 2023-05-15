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


        [Tooltip("특수 공격 타입")]
        public AttackType m_GrabAttack = AttackType.Grab;

        [Tooltip("근접 강공격 사거리")]
        public float HeavyAttackRange;

        [Tooltip("도약 공격 속도")]
        public float JumpBiteAttackSpeed;

        [Tooltip("도약 공격 최소 사거리")]
        public float m_JumpBittAttackMinRange;

        [Tooltip("도약 공격 최대 사거리")]
        public float m_JumpBiteAttackMaxRange;
    }
}
