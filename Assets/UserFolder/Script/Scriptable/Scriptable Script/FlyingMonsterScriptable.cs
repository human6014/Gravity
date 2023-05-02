using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "FlyingMonsterSetting", menuName = "Scriptable Object/FlyingMonsterSettings", order = int.MaxValue - 2)]
    public class FlyingMonsterScriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("몬스터 타입")]
        public EnumType.FlyingMonsterType m_MonsterType;
    }
}
