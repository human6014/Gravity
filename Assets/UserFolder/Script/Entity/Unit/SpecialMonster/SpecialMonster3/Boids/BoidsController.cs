using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class BoidsController : MonoBehaviour
{
    #region SerializeField
    [Header("Boid Options")]
    [SerializeField] private BoidsMonster boidUnitPrefab;

    [Tooltip("미리 생성할 유닛 수")]
    [Range(5, 5000)] [SerializeField] private int m_BoidPollingCount = 1000;

    [Tooltip("생성 범위")]
    [Range(5, 100)] [SerializeField] private float m_SpawnRange = 15;
    #endregion

    private ObjectPoolManager.PoolingObject poolingObj;
    private Transform m_BoidsPool;

    private const string m_ActivePoolName = "BoidsPool";

    public float SpawnRange { get => m_SpawnRange; }

    private void Awake()
    {
        m_BoidsPool = GameObject.Find(m_ActivePoolName).transform;
    }

    private void Start()
    {
        poolingObj = ObjectPoolManager.Register(boidUnitPrefab, m_BoidsPool);
        poolingObj.GenerateObj(m_BoidPollingCount);
    }

    public void GenerateBoidMonster(int spawnCount)
    {
        Vector3 randomVec;
        Quaternion randomRot;
        BoidsMonster currUnit;
        for (int i = 0; i < spawnCount; i++)
        {
            randomVec = Random.insideUnitSphere * m_SpawnRange;
            randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            currUnit = (BoidsMonster)poolingObj.GetObject(true);
            currUnit.transform.SetPositionAndRotation(transform.position + randomVec, randomRot);
            currUnit.Init(this);
        }
    }

    public void ReturnObj(PoolableScript _poolableScript) => poolingObj.ReturnObject(_poolableScript);
    
}

