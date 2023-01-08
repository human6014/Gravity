using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class UnitManager : MonoBehaviour
    {
        // 유닛 소환 순서
        // urban -> oldman -> women -> big -> giant
        [Header("SerializeField Unit")]
        [SerializeField] private NormalMonster urbanZombie;
        [SerializeField] private NormalMonster oldmanZombie;
        [SerializeField] private NormalMonster womenZombie;
        [SerializeField] private NormalMonster bigZomibe;
        [SerializeField] private NormalMonster giantZombie;

        [Space(15)]
        [SerializeField] private SpecialMonster1 specialMonster1;
        [SerializeField] private SpecialMonster2 specialMonster2;
        [SerializeField] private SpecialMonster3 specialMonster3;

        [Header("info")]
        [Tooltip("미리 생성할 유닛 수 urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)] [SerializeField] private int [] poolingCount;

        #region Property
        public NormalMonster OldmanZombie { get => oldmanZombie; private set => oldmanZombie = value; }
        public NormalMonster UrbanZombie { get => urbanZombie; private set => urbanZombie = value; }
        public NormalMonster WomenZombie { get => womenZombie; private set => womenZombie = value; }
        public NormalMonster BigZomibe { get => bigZomibe; private set => bigZomibe = value; }
        public NormalMonster GiantZombie { get => giantZombie; private set => giantZombie = value; }

        public SpecialMonster1 SpecialMonster1 { get => specialMonster1; private set => specialMonster1 = value; }
        public SpecialMonster2 SpecialMonster2 { get => specialMonster2; private set => specialMonster2 = value; }
        public SpecialMonster3 SpecialMonster3 { get => specialMonster3; private set => specialMonster3 = value; }
        #endregion
        void Start()
        {

        }
    }
}
