using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
public class Boids : MonoBehaviour
{
    #region Variables & Initializer

    ObjectPoolManager.PoolingObject poolingObj;

    [Header("Boid Options")]
    [SerializeField] private BoidsMonster boidUnitPrefab;
    [SerializeField] private Transform boidsPool;
    [SerializeField] private Transform target;

    [Range(5, 5000)]
    public int boidCount;
    [Range(5, 100)]
    public float spawnRange = 20;
    
    void Start()
    {
        poolingObj = ObjectPoolManager.objectPoolManager.Register(boidUnitPrefab,boidsPool);
        poolingObj.GenerateObj(boidCount);

        Vector3 randomVec;
        Quaternion randomRot;
        BoidsMonster currUnit;
        for (int i = 0; i < boidCount; i++)
        {
            randomVec = Random.insideUnitSphere * spawnRange;
            randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            currUnit = (BoidsMonster)poolingObj.GetObject();
            currUnit.transform.SetPositionAndRotation(transform.position + randomVec, randomRot);
            currUnit.InitializeUnit(this, target);
        }
    }

    public void ReturnObj(PoolableScript _poolableScript) => poolingObj.ReturnObject(_poolableScript);
    #endregion
}
