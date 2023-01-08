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
        public EnumType.NoramlMonster monsterType;

        [Header("Stat value")]
        [Tooltip("ü��")] 
        public int HP;
        [Tooltip("���ݷ�")] 
        public int damage;
        [Tooltip("�̵��ӵ�")]
        public int movementSpeed;

        //���...
    }
}
