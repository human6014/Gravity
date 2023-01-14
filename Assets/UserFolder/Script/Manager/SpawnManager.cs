using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    [RequireComponent(typeof(UnitManager))]
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        [Header("Polling info")]
        [Tooltip("활성화된 유닛의 transform")]
        [SerializeField] private Transform activeUnitPool;

        [Tooltip("미리 생성할 유닛 수 urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)] [SerializeField] private int[] poolingCount;

        private BoxCollider[] spawnAreaXDown;
        private BoxCollider[] spawnAreaXUp;

        private BoxCollider[] spawnAreaYDown;
        private BoxCollider[] spawnAreaYUp;

        private BoxCollider[] spawnAreaZDown;
        private BoxCollider[] spawnAreaZUp;

        private UnitManager unitManager;
        private Customization customization;
        private List <ObjectPoolManager.PoolingObject> poolingObj;
        // LIst -> 배열

        private float timer;
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
            //순서대로 넣어야 해~
            //urban -> oldman -> women -> big -> giant
            poolingObj = new List<ObjectPoolManager.PoolingObject>
            {
                ObjectPoolManager.Register(unitManager.UrbanZombie, activeUnitPool),
                ObjectPoolManager.Register(unitManager.OldmanZombie, activeUnitPool),
                ObjectPoolManager.Register(unitManager.WomenZombie, activeUnitPool),
                ObjectPoolManager.Register(unitManager.BigZomibe, activeUnitPool),
                ObjectPoolManager.Register(unitManager.GiantZombie, activeUnitPool)
                //등등
            };

            for (int i = 0; i < poolingObj.Count; i++)
            {
                poolingObj[i].GenerateObj(poolingCount[i]);
            }
        }

        /// <summary>
        /// 현재 중력에 맞는 땅의 BoxCollider 랜덤으로 가져옴
        /// GravityManager의 중력 방향과 반대니 햇깔리지 말도록
        /// </summary>
        /// <returns></returns>
        private BoxCollider GetClosetArea()
        {
            BoxCollider[] currentArea = null;
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

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 3)
            {
                timer = 0;

                NormalMonster currentUnit = (NormalMonster)poolingObj[4].GetObject();
                customization.Customize(currentUnit);
                currentUnit.Init(GetRandomPos());

            }
        }
    }
}
