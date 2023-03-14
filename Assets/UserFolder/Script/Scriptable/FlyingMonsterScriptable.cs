using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "FlyingMonsterSetting", menuName = "Scriptable Object/FlyingMonsterSettings", order = int.MaxValue - 2)]
    public class FlyingMonsterScriptable : ScriptableObject
    {
        [Header("Script info")]
        [Tooltip("���� Ÿ��")]
        public EnumType.FlyingMonsterType monsterType;

        [Header("Stat value")]
        [Tooltip("�ִ� ü��")]
        public int maxHp;
        [Tooltip("����")]
        public int def;
        [Tooltip("���ݷ�")]
        public int damage;
        [Tooltip("�̵��ӵ�")]
        public int movementSpeed;

        //���...
    }
}
