using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class Boids : MonoBehaviour
{
    #region Variables & Initializer

    private ObjectPoolManager.PoolingObject poolingObj;

    [Header("Boid Options")]
    [SerializeField] private BoidsMonster boidUnitPrefab;
    [SerializeField] private Transform boidsPool;

    [Tooltip("미리 생성할 유닛 수")]
    [Range(5, 5000)] [SerializeField] private int boidCount;

    [Tooltip("생성 범위")]
    [Range(5, 100)] [SerializeField] private float spawnRange = 20;

    public float SpawnRange { get => spawnRange; private set => spawnRange = value; }
    #endregion
    private void Start()
    {
        poolingObj = ObjectPoolManager.Register(boidUnitPrefab, boidsPool);
        poolingObj.GenerateObj(boidCount);
    }

    public void GenerateBoidMonster(int spawnCount)
    {
        Vector3 randomVec;
        Quaternion randomRot;
        BoidsMonster currUnit;
        for (int i = 0; i < spawnCount; i++)
        {
            randomVec = Random.insideUnitSphere * spawnRange;
            randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            currUnit = (BoidsMonster)poolingObj.GetObject(true);
            currUnit.transform.SetPositionAndRotation(transform.position + randomVec, randomRot);
            currUnit.Init(this);
        }
    }

    public void ReturnObj(PoolableScript _poolableScript) => poolingObj.ReturnObject(_poolableScript);
    
}

