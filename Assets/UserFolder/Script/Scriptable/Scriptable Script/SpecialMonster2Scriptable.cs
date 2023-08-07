using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SpecialMonster2Setting", menuName = "Scriptable Object/SpecialMonster2Settings", order = int.MaxValue - 1)]
    public class SpecialMonster2Scriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("���� Ÿ��")]
        public EnumType.SpecialMonsterType m_MonsterType;

        [Tooltip("���� ��ֹ� ���� ���̾�")]
        public LayerMask m_ObstacleDetectLayer;
    }
}
