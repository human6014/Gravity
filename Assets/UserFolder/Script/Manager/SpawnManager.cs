using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private BoxCollider[] spawnAreaXDown;
        [SerializeField] private BoxCollider[] spawnAreaXUp;

        [SerializeField] private BoxCollider[] spawnAreaYDown;
        [SerializeField] private BoxCollider[] spawnAreaYUp;

        [SerializeField] private BoxCollider[] spawnAreaZDown;
        [SerializeField] private BoxCollider[] spawnAreaZUp;
        private UnitManager unitManager;

        private float timer;
        private void Awake()
        {
            unitManager = GetComponent<UnitManager>();
        }

        private BoxCollider GetClosetArea()
        {
            
            if (spawnAreaYDown[0].GetComponent<Area>().gravitiesType == GravitiesManager.currentGravityType)
            {

            }
            return spawnAreaYDown[0];
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

                GameObject obj = Instantiate(unitManager.UrbanZombie, GetRandomPos(), Quaternion.identity);
            }
        }
    }
}
