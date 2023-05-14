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
        [Tooltip("���� Ÿ��")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Tooltip("���� ������ ��Ÿ�")]
        public float HeavyAttackRange;

        [Tooltip("���� ���� �ӵ�")]
        public float JumpBiteAttackSpeed;

        [Tooltip("���� ���� ��Ÿ�")]
        public float JumpBiteAttackRange;

    }
}
