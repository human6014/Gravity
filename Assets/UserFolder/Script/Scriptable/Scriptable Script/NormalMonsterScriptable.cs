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
        public EnumType.NoramlMonsterType monsterType;

        [Header("Stat value")]
        [Tooltip("체력")] 
        public int hp;

        [Tooltip("방어력")]
        public int def;

        [Tooltip("공격력")] 
        public int damage;

        [Tooltip("이동 속도")]
        public float movementSpeed;

        [Tooltip("공격 속도")]
        public float attackSpeed;

        [Tooltip("공격 사거리")]
        public float attackRange;

        //등등...
    }
}
