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
        public NoramlMonsterType m_MonsterType;

        [Tooltip("달리기가 가능한 Stat값")]
        public float m_CanRunStat = 2;

        [Tooltip("달리기 속도")]
        public float m_RunningSpeed = 5.5f;
    }
}
