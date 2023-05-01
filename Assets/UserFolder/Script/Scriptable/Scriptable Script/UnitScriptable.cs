using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scriptable
{
    public class UnitScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Stat value")]
        [Tooltip("체력")]
        public int m_HP;

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
    }
}
