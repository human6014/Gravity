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
        public EnumType.NoramlMonster monsterType;

        [Header("Stat value")]
        [Tooltip("체력")] 
        public int HP;
        [Tooltip("공격력")] 
        public int damage;
        [Tooltip("이동속도")]
        public int movementSpeed;

        //등등...
    }
}
