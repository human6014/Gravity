using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Normal;
using Entity.Unit.Flying;
using Entity.Unit.Special;
using Entity.Object;

namespace Manager
{
    public class EntityManager : MonoBehaviour
    {
        // 유닛 소환 순서
        // urban -> oldman -> women -> big -> giant
        [Header("SerializeField Unit")]
        [SerializeField] private NormalMonster [] normalZombies;

        [Space(12)]
        [SerializeField] private FlyingMonster[] flyingMonsters;
        [SerializeField] private PoisonSphere poisonSphere;

        [Space(15)]
        [SerializeField] private GameObject specialMonster1;
        [SerializeField] private GameObject specialMonster2;
        [SerializeField] private GameObject specialMonster3;

        #region Property
        public PoisonSphere GetPoisonSphere => poisonSphere;
        public GameObject GetSpecialMonster1 => specialMonster1;
        public GameObject GetSpecialMonster2 => specialMonster2;
        public GameObject GetSpecialMonster3  => specialMonster3;
        #endregion

        #region Getter
        public NormalMonster GetNormalMonster(int i) => normalZombies[i];
        public FlyingMonster GetFlyingMonster(int i) => flyingMonsters[i];
        
        public int GetFlyingMonsterArrayLength() => flyingMonsters.Length;
        public int GetNormalMonsterArrayLength() => normalZombies.Length;
        #endregion
        
    }
}
