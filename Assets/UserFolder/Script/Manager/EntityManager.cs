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

        [Space(15)]
        [SerializeField] private GameObject specialMonster1;
        [SerializeField] private SpecialMonster2 specialMonster2;
        [SerializeField] private SpecialMonster3 specialMonster3;

        [Space(15)]
        [SerializeField] private DefaultPoolingScript[] m_EffectObject;
        [SerializeField] private DefaultPoolingScript[] m_CasingObject;
        [SerializeField] private DefaultPoolingScript[] m_MagazineObject;

        #region Property
        public GameObject GetSpecialMonster1 => specialMonster1;
        public SpecialMonster2 GetSpecialMonster2 => specialMonster2;
        public SpecialMonster3 GetSpecialMonster3  => specialMonster3;
        #endregion

        #region Getter
        public NormalMonster GetNormalMonster(int i) => normalZombies[i];
        public FlyingMonster GetFlyingMonster(int i) => flyingMonsters[i];
        public DefaultPoolingScript GetEffectObject(int i) => m_EffectObject[i];
        public DefaultPoolingScript GetCasingObject(int i) => m_CasingObject[i];
        public DefaultPoolingScript GetMagazineObject(int i) => m_MagazineObject[i];
        
        public int GetFlyingMonsterArrayLength() => flyingMonsters.Length;
        public int GetNormalMonsterArrayLength() => normalZombies.Length;
        public int GetEffectArrayLength() => m_EffectObject.Length;
        public int GetCasingArrayLength() => m_CasingObject.Length;
        public int GetMagazineArrayLength() => m_MagazineObject.Length;
        #endregion
        
    }
}
