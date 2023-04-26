using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "NormalMonsterSetting", menuName = "Scriptable Object/NormalMonsterSettings", order = int.MaxValue - 1)]
    public class NormalMonsterScriptable : ScriptableObject
    {
        [Header("Script info")]
        [Tooltip("���� Ÿ��")] 
        public EnumType.NoramlMonsterType m_MonsterType;

        [Header("Stat value")]
        [Tooltip("ü��")] 
        public int m_MaxHp;

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

        //���...
    }
}
