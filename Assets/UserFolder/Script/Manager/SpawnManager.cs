using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
using Entity.Unit.Normal;
using Entity.Unit.Special;
using System.Linq;

namespace Manager
{
    [System.Serializable]
    public struct StageInfo
    {
        [Tooltip("현재 스테이지에서 특수 몬스터가 출현하는 Wave")]
        public int m_SpawnSpecialWave;

        [Tooltip("다음 Wave로 넘어가기 위한 시간")]
        public float[] m_WaveTiming;
    }

    [RequireComponent(typeof(EntityManager))]
    public class SpawnManager : MonoBehaviour
    {
        #region SerializeField
        [Tooltip("특수 유닛 스폰 여부")]
        [SerializeField] private bool m_IsActiveSpecialSpawn;

        [Tooltip("일반 유닛 스폰 여부")]
        [SerializeField] private bool m_IsActiveNormalSpawn;

        [Tooltip("공중 유닛 스폰 여부")]
        [SerializeField] private bool m_IsActiveFlyingSpawn;

        [Tooltip("스테이지, 웨이브 정보")]
        [SerializeField] private StageInfo[] m_StageInfo;

        [Tooltip("무한 웨이브 시간")]
        [SerializeField] private float m_InfintyWaveTiming;

        [Header("Spawn Area")]
        [Tooltip("스폰영역을 자식으로 가지는 Transform")]
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        [Tooltip("SpecialMonster2 스폰 위치 부모 Transform")]
        [SerializeField] private Transform m_SP2SpawnPos;

        [Tooltip("SpecialMonster3 스폰 위치")]
        [SerializeField] private Transform m_SP3SpawnPos;

        [Header("Pooling Transform")]
        [Tooltip("활성화된 유닛의 transform")]
        [SerializeField] private Transform activeUnitPool;

        [Header("Pooling Count")]
        [Tooltip("미리 생성할 유닛 수 urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)][SerializeField] private int[] normalMonsterPoolingCount;

        [Tooltip("미리 생성할 FlyingMonstr 수")]
        [Range(0, 100)] [SerializeField] private int[] flyingMonsterPoolingCount;

        [Tooltip("미리 생성할 PoisonSphere 수")]
        [Range(0, 50)] [SerializeField] private int poisonSpherePoolingCount;

        [Header("Spawn info")]
        [Tooltip("최대로 생성될 일반 몬스터 총 개수")]
        [Range(0, 100)][SerializeField] private int maxNormalMonsterCount = 30;

        [Tooltip("최대로 생성될 공중 몬스터 총 개수")]
        [Range(0, 50)] [SerializeField] private int maxFlyingMonsterCount = 10;

        [Tooltip("일반 몬스터 소환 주기")]
        [SerializeField] private float normalMonsterSpawnTime = 3;

        [Tooltip("공중 몬스터 소환 주기")]
        [SerializeField] private float flyingMonsterSpawnTime = 5;

        [Tooltip("몬스터 생성 확률")]
        [SerializeField] private float [] m_MonsterProbs;
        #endregion

        #region Object Value
        private readonly BoxCollider[][] m_SpawnAreaCollider = new BoxCollider[6][];
        private BoxCollider[] m_CurrentAreaCollider;

        private EnvironmentManager m_EnvironmentManager;
        private EntityManager m_EntityManager;
        private Customization m_Customization;
        private Octree m_Octree;

        private ObjectPoolManager.PoolingObject[] m_NormalMonsterPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_FlyingMonsterPoolingObjectArray;
        private ObjectPoolManager.PoolingObject m_PoisonSpherePooling;
        #endregion

        #region Normal Value
        private EnumType.GravityType m_CurrentGravityType = EnumType.GravityType.yDown;
        private readonly float[] m_Probs = new float[] { 52, 21, 21, 4, 2 };

        private float m_Total = 0;

        private float m_NormalMonsterTimer;
        private float m_FlyingMonsterTimer;

        private float m_WaveTimer;

        private float m_StatMultiplier = 0;

        private int m_RandomNormalMonsterIndex;
        private int m_RandomFlyingMonsterIndex;

        private int m_CurrentStage;
        private int m_CurrentWave;
        
        #endregion

        #region Property
        public static int NormalMonsterCount { get; set; }
        public static int FlyingMonsterCount { get; set; }

        public bool IsSP1MonsterSpawned { get; private set; }
        public bool IsSP2MonsterSpawned { get; private set; }
        public bool IsSP3MonsterSpawned { get; private set; }

        public bool IsSP1MonsterEnd { get; private set; }
        public bool IsSP2MonsterEnd { get; private set; }
        public bool IsSP3MonsterEnd { get; private set; }

        public int CurrentStage 
        {
            get => m_CurrentStage;
            set
            {
                m_CurrentStage = value;
                m_CurrentWave = 1;
                m_StatMultiplier = (m_CurrentStage - 1) + ((m_CurrentWave - 1) * 0.5f);
                m_WaveTimer = 0;
            }
        }

        public int CurrentWave
        {
            get => m_CurrentWave;
            set
            {
                m_CurrentWave = value;
                m_StatMultiplier = (m_CurrentStage - 1) + ((m_CurrentWave - 1) * 0.5f);
                m_WaveTimer = 0;
            }
        }
        #endregion

        #region Init
        private void Awake()
        {
            m_EnvironmentManager = GetComponent<EnvironmentManager>();
            m_EntityManager = GetComponent<EntityManager>();
            m_Customization = GetComponent<Customization>();
            m_Octree = FindObjectOfType<Octree>();

            for (int i = 0; i < spawnAreaTransform.Length; i++)
                m_SpawnAreaCollider[i] = spawnAreaTransform[i].GetComponentsInChildren<BoxCollider>();
            
            m_CurrentAreaCollider = m_SpawnAreaCollider[2]; // YDown

            CurrentStage = 1;
            NormalMonsterCount = 0;
            FlyingMonsterCount = 0;

            foreach (float elem in m_MonsterProbs) m_Total += elem;

            GravityManager.GravityChangeAction += ChangeCurrentArea;
        }

        private void Start()
        {
            RegisterPoolingObject();
        }

        private void RegisterPoolingObject()
        {
            int normalMonsterArrayLength = m_EntityManager.GetNormalMonsterArrayLength();
            int flyingMonsterArrayLength = m_EntityManager.GetFlyingMonsterArrayLength();

            if (normalMonsterArrayLength != normalMonsterPoolingCount.Length)
                Debug.LogError("Normal monster pooling count is different from the number of pooling object's length");
            if (flyingMonsterArrayLength != flyingMonsterPoolingCount.Length)
                Debug.LogError("Flying monster pooling count is different from the number of pooling object's length");


            //normal monster 인덱스 순서
            //urban -> oldman -> women -> big -> giant
            m_NormalMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[normalMonsterArrayLength];
            m_FlyingMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[flyingMonsterArrayLength];
            

            //Unit
            for (int i = 0; i < normalMonsterArrayLength; i++)
            {
                m_NormalMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetNormalMonster(i), activeUnitPool);
                m_NormalMonsterPoolingObjectArray[i].GenerateObj(normalMonsterPoolingCount[i]);
            }

            for (int i = 0; i < flyingMonsterArrayLength; i++)
            {
                m_FlyingMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetFlyingMonster(i), activeUnitPool);
                m_FlyingMonsterPoolingObjectArray[i].GenerateObj(flyingMonsterPoolingCount[i]);
            }

            m_PoisonSpherePooling = ObjectPoolManager.Register(m_EntityManager.GetPoisonSphere, activeUnitPool);
            m_PoisonSpherePooling.GenerateObj(poisonSpherePoolingCount);
        }

        public void ChangeCurrentArea(EnumType.GravityType gravityType)
        {
            m_CurrentGravityType = gravityType;
            m_CurrentAreaCollider = m_SpawnAreaCollider[(int)gravityType];
        }
        #endregion

        #region Random related

        /// <summary>
        /// BoxCollider[] 배열 안의 임의의 BoxCollider 하나를 반환함
        /// </summary>
        /// <param name="boxColliders">BoxCollider[]</param>
        /// <returns>BoxCollider</returns>
        private BoxCollider GetClosetArea(BoxCollider[] boxColliders)
        {
            int rand = Random.Range(0, boxColliders.Length);
            return boxColliders[rand];
        }

        /// <summary>
        /// BoxCollider 안의 임의의 위치를 반환함
        /// </summary>
        /// <param name="boxCollider">BoxCollider</param>
        /// <returns>Vector3</returns>
        private Vector3 GetRandomPos(BoxCollider boxCollider, float interporateRadius)
        {
            interporateRadius *= 0.5f;

            float rangeX = Random.Range(
                -boxCollider.bounds.size.x * 0.5f + interporateRadius,
                 boxCollider.bounds.size.x * 0.5f - interporateRadius);
            float rangeZ = Random.Range(
                -boxCollider.bounds.size.z * 0.5f + interporateRadius, 
                 boxCollider.bounds.size.z * 0.5f - interporateRadius);

            return boxCollider.transform.position + new Vector3(rangeX, 0, rangeZ);
        }

        /// <summary>
        /// 서로 다른 확률에서 무작위 index 산출
        /// </summary>
        /// <returns>무작위 int index</returns>
        private int RandomUnitIndex()
        {
            float randomPoint = Random.value * m_Total;

            for (int i = 0; i < m_MonsterProbs.Length; i++)
            {
                if (randomPoint < m_MonsterProbs[i]) return i;
                else randomPoint -= m_MonsterProbs[i];
            }
            return m_MonsterProbs.Length - 1;
        }

        /// <summary>
        /// spawnAreaCollider 배열에서 특정 값을 제외하고 무작위 index 산출
        /// </summary>
        /// <param name="excludeIndex">제외할 index</param>
        /// <param name="specificIndex">excludeIndex를 제외하고 랜덤으로 나온 index</param>
        /// <returns>excludeIndex를 제외하고 랜덤으로 나온 BoxCollider[]</returns>
        private BoxCollider[] ExcludeRandomIndex(int excludeIndex, out int specificIndex)
        {
            HashSet<int> exclude = new() { excludeIndex };
            IEnumerable<int> range = Enumerable.Range(0, m_SpawnAreaCollider.Length).Where(i => !exclude.Contains(i));

            int index = Random.Range(0, m_SpawnAreaCollider.Length - exclude.Count);
            specificIndex = range.ElementAt(index);
            return m_SpawnAreaCollider[specificIndex];
        }
        #endregion

        #region Stage & Wave
        private void Update()
        {
            StageWaveCheck();
            SpawnMonster();
        }

        private void StageWaveCheck()
        {
            m_WaveTimer += Time.deltaTime;

            if (CurrentStage - 1 >= m_StageInfo.Length)
            {
                if (IsSP3MonsterEnd) GameManager.GameClear();
                else if (m_InfintyWaveTiming <= m_WaveTimer) CurrentWave++;
                return;
            }
            StageInfo currentStageInfo = m_StageInfo[CurrentStage - 1];
            if (currentStageInfo.m_WaveTiming[CurrentWave - 1] <= m_WaveTimer)
            {
                CurrentWave++;
                if (CurrentWave - 1 == currentStageInfo.m_SpawnSpecialWave - 1) SpawnSpecialMonster();
                if (currentStageInfo.m_WaveTiming.Length <= CurrentWave - 1) CurrentStage++;
            }
        }
        #endregion

        #region SpawnUnit

        #region SpawnNormalMonster
        private void SpawnMonster()
        {
            if (m_IsActiveNormalSpawn)
            {
                m_NormalMonsterTimer += Time.deltaTime;

                if (m_NormalMonsterTimer >= normalMonsterSpawnTime && NormalMonsterCount < maxNormalMonsterCount) 
                    SpawnNormalMonster();
            }
            if (m_IsActiveFlyingSpawn)
            {
                m_FlyingMonsterTimer += Time.deltaTime;
                if (m_FlyingMonsterTimer >= flyingMonsterSpawnTime && FlyingMonsterCount < maxFlyingMonsterCount) 
                    SpawnFlyingMonster();
            }
        }

        private void SpawnNormalMonster()
        {
            m_NormalMonsterTimer = 0;
            m_RandomNormalMonsterIndex = RandomUnitIndex();
            NormalMonsterCount++;

            BoxCollider boxCollider = GetClosetArea(m_CurrentAreaCollider);
            Vector3 initPosition = GetRandomPos(boxCollider, 1);

            NormalMonster currentNormalMonster = (NormalMonster)m_NormalMonsterPoolingObjectArray[m_RandomNormalMonsterIndex].GetObject(false);
            m_Customization.Customize(currentNormalMonster);

            currentNormalMonster.Init(initPosition, m_NormalMonsterPoolingObjectArray[m_RandomNormalMonsterIndex], m_StatMultiplier);
            currentNormalMonster.gameObject.SetActive(true);
        }

        private void SpawnFlyingMonster()
        {
            m_FlyingMonsterTimer = 0;
            //index 할당 아직 필요 x
            FlyingMonsterCount++;

            FlyingMonster currentFlyingMonster = (FlyingMonster)m_FlyingMonsterPoolingObjectArray[0].GetObject(false);
            currentFlyingMonster.Init(m_Octree.GetRandomSpawnableArea(), m_StatMultiplier, m_FlyingMonsterPoolingObjectArray[0], m_PoisonSpherePooling);
            currentFlyingMonster.gameObject.SetActive(true);
        }
        #endregion

        #region SpawnSpecialMonsters
        private void SpawnSpecialMonster()
        {
            if (!m_IsActiveSpecialSpawn) return;
            switch (CurrentStage)
            {
                case 1:
                    //SpawnSpecialMonster3();
                    SpawnSpecialMonster1();
                    break;
                case 2:
                    //SpawnSpecialMonster2();
                    break;
                case 3:
                    //SpawnSpecialMonster3();
                    break;
            }
        }

        public async void SpawnSpecialMonster1()
        {
            await m_EnvironmentManager.FogDensityChange(0.015f, 10);

            BoxCollider[] initColliders = ExcludeRandomIndex((int)m_CurrentGravityType, out int specificIndex);
            BoxCollider initCollider = GetClosetArea(initColliders);

            Vector3 initPosition = GetRandomPos(initCollider, 4);
            Quaternion initRotation = GravityManager.GetSpecificGravityRotation(specificIndex);

            SpecialMonster1 specialMonster1 = Instantiate(m_EntityManager.GetSpecialMonster1, initPosition, Quaternion.identity).GetComponent<SpecialMonster1>();
            specialMonster1.EndSpecialMonsterAction += () => IsSP1MonsterEnd = true;
            specialMonster1.EndSpecialMonsterAction += () => m_EnvironmentManager.OnRainParticle();
            specialMonster1.Init(initRotation, m_StatMultiplier);

            IsSP1MonsterSpawned = true;
        }

        public async void SpawnSpecialMonster2()
        {
            await m_EnvironmentManager.FogDensityChange(0.05f, 10);

            StartCoroutine(ChangeGravityToYDown());
            
            Vector3 initPosition = Vector3.zero;
            float farDist = 0;
            float dist = 0;
            foreach(Transform t in m_SP2SpawnPos)
            {
                dist = Vector3.SqrMagnitude(t.position - AI.AIManager.PlayerTransform.position);
                if (dist > farDist)
                {
                    farDist = dist;
                    initPosition = t.position;
                }
            }

            SpecialMonster2 specialMonster2 = Instantiate(m_EntityManager.GetSpecialMonster2, initPosition, Quaternion.identity).GetComponent<SpecialMonster2>();
            specialMonster2.EndSpecialMonsterAction += () => IsSP2MonsterEnd = true;
            specialMonster2.EndSpecialMonsterAction += () => GravityManager.CantGravityChange = false;
            specialMonster2.Init(m_SP2SpawnPos,m_StatMultiplier);

            IsSP2MonsterSpawned = true;

            await m_EnvironmentManager.FogDensityChange(0.025f, 10);
        }

        private IEnumerator ChangeGravityToYDown()
        {
            yield return new WaitWhile(() => GravityManager.IsGravityChanging);
            FindObjectOfType<GravityManager>().GravityChange(1, -0.1f);
            GravityManager.CantGravityChange = true;
        }

        public async void SpawnSpecialMonster3()
        {
            m_EnvironmentManager.OffRainParticle();
            await m_EnvironmentManager.FogDensityChange(0.1f,15);

            Camera.main.farClipPlane = 120;

            Vector3 initPosition = m_SP3SpawnPos.position;
            PathCreation.PathCreator pathCreator = m_SP3SpawnPos.parent.GetComponent<PathCreation.PathCreator>();

            SpecialMonster3 specialMonster3 = Instantiate(m_EntityManager.GetSpecialMonster3, initPosition, Quaternion.identity).GetComponent<SpecialMonster3>();
            specialMonster3.EndSpecialMonsterAction += () => IsSP3MonsterEnd = true;
            specialMonster3.Init(pathCreator, m_StatMultiplier);

            IsSP3MonsterSpawned = true;
            await m_EnvironmentManager.FogDensityChange(0.04f,5);
        }
        #endregion

        #endregion
    }
}
