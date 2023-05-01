using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scriptable
{
    public class UnitScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Stat value")]
        [Tooltip("ü��")]
        public int m_HP;

        [Tooltip("����")]
        public int m_Def;

        [Tooltip("���ݷ�")]
        public int m_Damage;

        [Tooltip("�̵� �ӵ�")]
        public float m_MovementSpeed;

        [Tooltip("���� �ӵ�")]
        public float m_AttackSpeed;

        [Tooltip("���� ��Ÿ�")]
        public float m_AttackRange;
    }
}
