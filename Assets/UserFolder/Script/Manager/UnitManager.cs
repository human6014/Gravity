using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Normal;
using Entity.Unit.Flying;
using Entity.Unit.Special;

namespace Manager
{
    public class UnitManager : MonoBehaviour
    {
        // ���� ��ȯ ����
        // urban -> oldman -> women -> big -> giant
        [Header("SerializeField Unit")]
        [SerializeField] private NormalMonster [] normalZombies;

        [Space(12)]
        [SerializeField] private FlyingMonster[] flyingMonsters;

        [Space(15)]
        [SerializeField] private SpecialMonster1 specialMonster1;
        [SerializeField] private SpecialMonster2 specialMonster2;
        [SerializeField] private SpecialMonster3 specialMonster3;

        #region Property
        public NormalMonster GetNormalMonster(int i) => normalZombies[i];
        public int GetNormalMonsterArrayLength() => normalZombies.Length;

        public FlyingMonster GetFlyingMonster(int i) => flyingMonsters[i];
        public int GetFlyingMonsterArrayLength() => flyingMonsters.Length;

        public SpecialMonster1 SpecialMonster1 { get => specialMonster1; private set => specialMonster1 = value; }
        public SpecialMonster2 SpecialMonster2 { get => specialMonster2; private set => specialMonster2 = value; }
        public SpecialMonster3 SpecialMonster3 { get => specialMonster3; private set => specialMonster3 = value; }
        #endregion
    }
}
