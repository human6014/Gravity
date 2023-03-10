using Entity.Unit.Normal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    [RequireComponent(typeof(UnitManager))]
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private bool isActiveSpawn;

        #region SerializeField
        [Header("Spawn Area")]
        [Tooltip("스폰영역을 자식으로 가지는 Transform")]
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        [Header("Polling info")]
        [Tooltip("활성화된 유닛의 transform")]
        [SerializeField] private Transform activeUnitPool;

        [Tooltip("미리 생성할 유닛 수 urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)][SerializeField] private int[] poolingCount;

        [Header("Spawn info")]
        [Tooltip("최대로 생성될 유닛의 총 개수")]
        [Range(0, 100)][SerializeField] private int maxUnitCount = 30;
        #endregion

        #region Object Value
        private BoxCollider[] spawnAreaXDown;
        private BoxCollider[] spawnAreaXUp;

        private BoxCollider[] spawnAreaYDown;
        private BoxCollider[] spawnAreaYUp;

        private BoxCollider[] spawnAreaZDown;
        private BoxCollider[] spawnAreaZUp;

        private UnitManager unitManager;
        private Customization customization;

        private ObjectPoolManager.PoolingObject[] poolingObjectArray;
        #endregion

        #region Normal Value
        private readonly float[] probs = new float[] { 45, 20, 20, 10, 5 };
        private float total = 0;

        private float timer;
        private int randomUnitIndex;
        private int currentUnitCount;
        #endregion
        private void Awake()
        {
            unitManager = GetComponent<UnitManager>();
            customization = GetComponent<Customization>();

            spawnAreaXDown  = spawnAreaTransform[0].GetComponentsInChildren<BoxCollider>();
            spawnAreaXUp    = spawnAreaTransform[1].GetComponentsInChildren<BoxCollider>();

            spawnAreaYDown  = spawnAreaTransform[2].GetComponentsInChildren<BoxCollider>();
            spawnAreaYUp    = spawnAreaTransform[3].GetComponentsInChildren<BoxCollider>();

            spawnAreaZDown  = spawnAreaTransform[4].GetComponentsInChildren<BoxCollider>();
            spawnAreaZUp    = spawnAreaTransform[5].GetComponentsInChildren<BoxCollider>();
        }

        private void Start()
        {
            foreach (float elem in probs) total += elem;

            int unitLength = unitManager.GetNormalMonsterArrayLength();
            if (unitLength != poolingCount.Length)
                Debug.LogWarning("Polling Count is different from the number of PoolingObject's length");

            //인덱스 순서
            //urban -> oldman -> women -> big -> giant
            
            poolingObjectArray = new ObjectPoolManager.PoolingObject[unitLength];
            for (int i = 0; i < unitLength; i++)
            {
                poolingObjectArray[i] = ObjectPoolManager.Register(unitManager.GetNormalMonster(i), activeUnitPool);
                poolingObjectArray[i].GenerateObj(poolingCount[i]);
            }
        }

        /// <summary>
        /// 현재 중력에 맞는 땅의 BoxCollider 랜덤으로 가져옴
        /// GravityManager의 중력 방향과 반대니 햇깔리지 말도록
        /// </summary>
        /// <returns>현재 중력에 맞는 땅에서 배열에서 랜덤으로 선정된 BoxCollider</returns>
        private BoxCollider GetClosetArea()
        {
            BoxCollider[] currentArea = null;

            //배열로 바꾸면 코드 깔끔해질 수도?
            //중력 바꿀때만 설정해줘도 됌
            //지금은 유닛 소환할 때마다 수행함
            switch (GravitiesManager.currentGravityType)
            {
                case EnumType.GravitiesType.xUp:
                    currentArea = spawnAreaXDown;
                    break;
                case EnumType.GravitiesType.xDown:
                    currentArea = spawnAreaXUp;
                    break;

                case EnumType.GravitiesType.yUp:
                    currentArea = spawnAreaYDown;
                    break;
                case EnumType.GravitiesType.yDown:
                    currentArea = spawnAreaYUp;
                    break;

                case EnumType.GravitiesType.zUp:
                    currentArea = spawnAreaZDown;
                    break;
                case EnumType.GravitiesType.zDown:
                    currentArea = spawnAreaZUp;
                    break;
            }

            int rand = Random.Range(0,currentArea.Length);
            return currentArea[rand];
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

            timer += Time.deltaTime;
            if (timer >= 3 && currentUnitCount <= maxUnitCount)
            {
                timer = 0;
                randomUnitIndex = RandomUnitIndex();
                currentUnitCount++;

                NormalMonster currentUnit = (NormalMonster)poolingObjectArray[randomUnitIndex].GetObject(false);
                customization.Customize(currentUnit);
                currentUnit.Init(GetRandomPos(), poolingObjectArray[randomUnitIndex]);
                currentUnit.gameObject.SetActive(true);
            }
        }
    }
}
