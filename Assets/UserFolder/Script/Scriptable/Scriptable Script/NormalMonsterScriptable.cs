using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "NormalMonsterSetting", menuName = "Scriptable Object/NormalMonsterSettings", order = int.MaxValue - 1)]
    public class NormalMonsterScriptable : UnitScriptable
    {
        [Header("Child")]
        [Header("Script info")]
        [Tooltip("몬스터 타입")] 
        public EnumType.NoramlMonsterType m_MonsterType;
    }
}
