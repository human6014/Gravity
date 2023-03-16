using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
using Entity.Unit.Normal;
using Entity.Unit.Special;

namespace Manager
{
    [RequireComponent(typeof(UnitManager))]
    public class SpawnManager : MonoBehaviour
    {
        public static int NormalMonsterCount { get; set; }
        public static int FlyingMonsterCount { get; set; }

        [SerializeField] private bool isActiveSpawn;

        #region SerializeField
        [Header("Spawn Area")]
        [Tooltip("스폰영역을 자식으로 가지는 Transform")]
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        [Header("Polling info")]
        [Tooltip("활성화된 유닛의 transform")]
        [SerializeField] private Transform activeUnitPool;

        [Tooltip("미리 생성할 유닛 수 urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)][SerializeField] private int[] normalMonsterPoolingCount;

        [Tooltip("미리 생성할 FlyingMonstr 수")]
        [Range(0, 100)] [SerializeField] private int[] flyingMonsterPoolingCount;

        [Header("Spawn info")]
        [Tooltip("최대로 생성될 일반 몬스터 총 개수")]
        [Range(0, 100)][SerializeField] private int maxNormalMonsterCount = 30;

        [Tooltip("일반 몬스터 소환 주기")]
        [SerializeField] private float normalMonsterSpawnTime = 3;

        [Tooltip("최대로 생성될 공중 몬스터 총 개수")]
        [Range(0, 50)] [SerializeField] private int maxFlyingMonsterCount = 10;

        [Tooltip("공중 몬스터 소환 주기")]
        [SerializeField] private float flyingMonsterSpawnTime = 5;
        #endregion

        #region Object Value
        private BoxCollider[][] spawnAreaCollider = new BoxCollider[6][];
        private BoxCollider[] currentAreaCollider;

        private UnitManager unitManager;
        private Customization customization;

        private ObjectPoolManager.PoolingObject[] normalMonsterPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] flyingMonsterPoolingObjectArray;
        #endregion

        #region Normal Value
        private EnumType.GravityType currentGravityType = EnumType.GravityType.yUp;
        private readonly float[] probs = new float[] { 45, 20, 20, 10, 5 };
        private float total = 0;

        private float normalMonsterTimer;
        private float flyingMonsterTimer;

        private int randomNormalMonsterIndex;
        private int currentNormalMonsterCount;

        private int randomFlyingMonsterIndex;
        private int currentFlyingMonsterCount;
        #endregion

        //temp
        [SerializeField] Octree octree;
        private void Awake()
        {
            unitManager = GetComponent<UnitManager>();
            customization = GetComponent<Customization>();

            for(int i = 0; i < spawnAreaTransform.Length; i++)
                spawnAreaCollider[i] = spawnAreaTransform[i].GetComponentsInChildren<BoxCollider>();
            
            currentAreaCollider = spawnAreaCollider[2]; // YDown

            NormalMonsterCount = 0;
            FlyingMonsterCount = 0;

            foreach (float elem in probs) total += elem;
        }

        private void Start()
        {
            int normalMonsterArrayLength = unitManager.GetNormalMonsterArrayLength();
            int flyingMonsterArrayLength = unitManager.GetFlyingMonsterArrayLength();
            if (normalMonsterArrayLength != normalMonsterPoolingCount.Length)
                Debug.LogError("Normal monster pooling Count is different from the number of PoolingObject's length");
            if (flyingMonsterArrayLength != flyingMonsterPoolingCount.Length)
                Debug.LogError("Flying monster pooling Count is different from the number of PoolingObject's length");

            //인덱스 순서
            //urban -> oldman -> women -> big -> giant
            
            normalMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[normalMonsterArrayLength];
            flyingMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[flyingMonsterArrayLength];
            for (int i = 0; i < normalMonsterArrayLength; i++)
            {
                normalMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(unitManager.GetNormalMonster(i), activeUnitPool);
                normalMonsterPoolingObjectArray[i].GenerateObj(normalMonsterPoolingCount[i]);
            }
            for(int i = 0; i < flyingMonsterArrayLength; i++)
            {
                flyingMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(unitManager.GetFlyingMonster(i), activeUnitPool);
                flyingMonsterPoolingObjectArray[i].GenerateObj(flyingMonsterPoolingCount[i]);
            }
            GravityManager.GravityChangeAction += (GravityType) => ChangeCurrentArea(GravityType);
        }

        public void ChangeCurrentArea(EnumType.GravityType gravityType)
        {
            currentGravityType = gravityType;
            Debug.Log(currentGravityType + "\tcurrentArea : " + (int)currentGravityType);
            currentAreaCollider = spawnAreaCollider[(int)gravityType];
        }

        /// <summary>
        /// 현재 중력에 맞는 땅의 BoxCollider 랜덤으로 가져옴
        /// GravityManager의 중력 방향과 반대니 햇깔리지 말도록
        /// </summary>
        /// <returns>현재 중력에 맞는 땅에서 배열에서 랜덤으로 선정된 BoxCollider</returns>
        private BoxCollider GetClosetArea()
        {
            int rand = Random.Range(0, currentAreaCollider.Length);
            return currentAreaCollider[rand];
        }

        private Vector3 GetRandomPos()
        {
            BoxCollider currentCol = GetClosetArea();

            float rangeX = Random.Range(-currentCol.bounds.size.x * 0.5f, currentCol.bounds.size.x * 0.5f);
            float rangeZ = Random.Range(-currentCol.bounds.size.z * 0.5f, currentCol.bounds.size.z * 0.5f);
            Vector3 randomPos = new(rangeX, 0, rangeZ);

            return currentCol.transform.position + randomPos;
        }

        private int RandomUnitIndex()
        {
            float randomPoint = Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i]) return i;
                else randomPoint -= probs[i];
            }
            return probs.Length - 1;
        }

        private void Update()
        {
            if (!isActiveSpawn) return;

            normalMonsterTimer += Time.deltaTime;
            flyingMonsterTimer += Time.deltaTime;

            if (normalMonsterTimer >= normalMonsterSpawnTime && NormalMonsterCount < maxNormalMonsterCount) SpawnNormalMonster();
            if (flyingMonsterTimer >= flyingMonsterSpawnTime && FlyingMonsterCount < maxFlyingMonsterCount) SpawnFlyingMonster();
        }

        private void SpawnNormalMonster()
        {
            normalMonsterTimer = 0;
            randomNormalMonsterIndex = RandomUnitIndex();
            NormalMonsterCount++;

            NormalMonster currentNormalMonster = (NormalMonster)normalMonsterPoolingObjectArray[randomNormalMonsterIndex].GetObject(false);
            customization.Customize(currentNormalMonster);
            currentNormalMonster.Init(GetRandomPos(), normalMonsterPoolingObjectArray[randomNormalMonsterIndex]);
            currentNormalMonster.gameObject.SetActive(true);
        }

        private void SpawnFlyingMonster()
        {
            flyingMonsterTimer = 0;
            //index 할당 아직 필요 x
            FlyingMonsterCount++;

            FlyingMonster currentFlyingMonster = (FlyingMonster)flyingMonsterPoolingObjectArray[0].GetObject(false);
            currentFlyingMonster.Init(octree.GetRandomSpawnableArea(), flyingMonsterPoolingObjectArray[0]);
            currentFlyingMonster.gameObject.SetActive(true);
        }

        private void SpawnSpecialMonster()
        {
            //
        }
    }
}
