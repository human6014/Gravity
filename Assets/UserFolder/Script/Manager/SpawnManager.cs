using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        private BoxCollider[] spawnAreaXDown;
        private BoxCollider[] spawnAreaXUp;

        private BoxCollider[] spawnAreaYDown;
        private BoxCollider[] spawnAreaYUp;

        private BoxCollider[] spawnAreaZDown;
        private BoxCollider[] spawnAreaZUp;
        private UnitManager unitManager;

        private float timer;
        private void Awake()
        {
            unitManager = GetComponent<UnitManager>();

            spawnAreaXDown = spawnAreaTransform[0].GetComponentsInChildren<BoxCollider>();
            spawnAreaXUp = spawnAreaTransform[1].GetComponentsInChildren<BoxCollider>();

            spawnAreaYDown = spawnAreaTransform[2].GetComponentsInChildren<BoxCollider>();
            spawnAreaYUp = spawnAreaTransform[3].GetComponentsInChildren<BoxCollider>();

            spawnAreaZDown = spawnAreaTransform[4].GetComponentsInChildren<BoxCollider>();
            spawnAreaZUp = spawnAreaTransform[5].GetComponentsInChildren<BoxCollider>();
        }

        /// <summary>
        /// ���� �߷¿� �´� ���� BoxCollider �������� ������
        /// GravityManager�� �߷� ����� �ݴ�� �ޱ��� ������
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
            //�Ź� �� �ʿ���ٰ� ������ -> �߷��� �ٲ𶧸� ���ָ� ��

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

                GameObject obj = Instantiate(unitManager.UrbanZombie, GetRandomPos(), Quaternion.identity);
                obj.GetComponent<NormalMonsterAI>().Init();

            }
        }
    }
}
