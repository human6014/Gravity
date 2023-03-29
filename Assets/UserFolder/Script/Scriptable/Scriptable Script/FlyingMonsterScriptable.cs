using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "FlyingMonsterSetting", menuName = "Scriptable Object/FlyingMonsterSettings", order = int.MaxValue - 2)]
    public class FlyingMonsterScriptable : ScriptableObject
    {
        [Header("Script info")]
        [Tooltip("몬스터 타입")]
        public EnumType.FlyingMonsterType monsterType;

        [Header("Stat value")]
        [Tooltip("최대 체력")]
        public int maxHp;

        [Tooltip("방어력")]
        public int def;

        [Tooltip("공격력")]
        public int damage;

        [Tooltip("이동속도")]
        public int movementSpeed;

        [Tooltip("공격 속도")]
        public float attackSpeed;

        [Tooltip("공격 사거리")]
        public float attackRange;
        //등등...
    }
}
