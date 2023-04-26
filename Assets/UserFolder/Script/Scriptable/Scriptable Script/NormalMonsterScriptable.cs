using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "NormalMonsterSetting", menuName = "Scriptable Object/NormalMonsterSettings", order = int.MaxValue - 1)]
    public class NormalMonsterScriptable : ScriptableObject
    {
        [Header("Script info")]
        [Tooltip("몬스터 타입")] 
        public EnumType.NoramlMonsterType m_MonsterType;

        [Header("Stat value")]
        [Tooltip("체력")] 
        public int m_MaxHp;

        [Tooltip("방어력")]
        public int m_Def;

        [Tooltip("공격력")] 
        public int m_Damage;

        [Tooltip("이동 속도")]
        public float m_MovementSpeed;

        [Tooltip("공격 속도")]
        public float m_AttackSpeed;

        [Tooltip("공격 사거리")]
        public float m_AttackRange;

        //등등...
    }
}
