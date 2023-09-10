using Entity.Unit.Flying;
using Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidsContollerCPU : MonoBehaviour
{
    #region SerializeField
    [Header("Boid Options")]
    [SerializeField] private BoidsMonster m_BoidUnitPrefab;

    [Tooltip("미리 생성할 유닛 수")]
    [Range(5, 1000)] [SerializeField] private int m_BoidPollingCount = 1000;

    [Tooltip("생성 범위")]
    [Range(1, 20)] [SerializeField] private float m_SpawnRange = 5;

    [Header("Patterns related")]
    [Tooltip("플레이어 추적 지속시간")]
    [SerializeField] private float m_BoidsMonsterTraceTime = 7.5f;
    [Tooltip("순찰 지속시간")]
    [SerializeField] private float m_PatrolTime = 25;
    #endregion

    private ObjectPoolManager.PoolingObject poolingObj;
    private Transform m_BoidsPool;
    private WaitForSeconds m_TraceOffSeconds;
    private WaitForSeconds m_PatrolOffSeconds;

    private readonly List<BoidsMonster> m_BoidMonsters = new List<BoidsMonster>();
    private readonly List<BoidsMovementCPU> m_BoidMovement = new List<BoidsMovementCPU>();
    private readonly System.Random m_MyRandom = new System.Random();

    private const string m_ActivePoolName = "BoidsPool";

    private const int m_TraceDividingCount = 100;

    public bool IsTraceAndBackPlayer { get; private set; }
    public bool IsPatrolBoids { get; private set; }

    public System.Action<int> ReturnChildObject { get; set; }

    private void Awake()
    {
        m_BoidsPool = GameObject.Find(m_ActivePoolName).transform;
        m_TraceOffSeconds = new WaitForSeconds(m_BoidsMonsterTraceTime);
        m_PatrolOffSeconds = new WaitForSeconds(m_PatrolTime);
    }

    public void Init(float traceTime, float patrolTime)
    {
        m_TraceOffSeconds = new WaitForSeconds(traceTime);
        m_PatrolOffSeconds = new WaitForSeconds(patrolTime);

        poolingObj = ObjectPoolManager.Register(m_BoidUnitPrefab, m_BoidsPool);
        poolingObj.GenerateObj(m_BoidPollingCount);
    }

    public int GenerateBoidMonster(int spawnCount)
    {
        if (spawnCount == 0) return 0;
        Vector3 randomVec;
        Quaternion randomRot;
        BoidsMonster currUnit = null;
        for (int i = 0; i < spawnCount; i++)
        {
            randomVec = Random.insideUnitSphere * m_SpawnRange;
            randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            currUnit = (BoidsMonster)poolingObj.GetObject(true);
            currUnit.transform.SetPositionAndRotation(transform.position + randomVec, randomRot);
            currUnit.DieAction += (int HP) => ReturnChildObject?.Invoke(HP);
            currUnit.ReturnAction += ReturnChildObj;
            currUnit.Init(transform, IsTraceAndBackPlayer, IsPatrolBoids);

            m_BoidMonsters.Add(currUnit);
            m_BoidMovement.Add(currUnit.GetComponent<BoidsMovementCPU>());
        }
        return currUnit.MaxHP * spawnCount;
    }

    #region Pattern
    public void TraceAttack(bool isActive)
    {
        if (m_BoidMonsters.Count == 0) return;
        int tracingCount = (m_BoidMonsters.Count / m_TraceDividingCount) + 1;

        int[] randomIndex = Enumerable.Range(0, m_BoidMovement.Count)
                                      .OrderBy(x => m_MyRandom.Next())
                                      .Take(tracingCount)
                                      .ToArray();

        for (int i = 0; i < randomIndex.Length; i++)
            m_BoidMonsters[i].TracePatternAction?.Invoke(isActive);
    }

    public void StartTraceAndBackPlayer()
    {
        IsTraceAndBackPlayer = true;
        StartCoroutine(TraceAndBackPlayer());
    }

    public void StartPatrolBoids()
    {
        IsPatrolBoids = true;
        StartCoroutine(PatrolBoids());
    }

    private IEnumerator TraceAndBackPlayer()
    {
        foreach (BoidsMonster bm in m_BoidMonsters)
            bm.TracePatternAction?.Invoke(IsTraceAndBackPlayer);

        yield return m_TraceOffSeconds;

        IsTraceAndBackPlayer = false;
        foreach (BoidsMonster bm in m_BoidMonsters)
            bm.TracePatternAction?.Invoke(IsTraceAndBackPlayer);
    }

    private IEnumerator PatrolBoids()
    {
        foreach (BoidsMonster bm in m_BoidMonsters)
            bm.PatrolPatternAction?.Invoke(IsPatrolBoids);

        yield return m_PatrolOffSeconds;

        IsPatrolBoids = false;
        foreach (BoidsMonster bm in m_BoidMonsters)
            bm.PatrolPatternAction?.Invoke(IsPatrolBoids);
    }
    #endregion

    public void ReturnChildObj(PoolableScript poolableScript)
    {
        m_BoidMovement.Remove(poolableScript.GetComponent<BoidsMovementCPU>());
        m_BoidMonsters.Remove((BoidsMonster)poolableScript);
        poolingObj.ReturnObject(poolableScript);
    }

    public void Dispose()
    {
        StopAllCoroutines();

        foreach (BoidsMonster bm in m_BoidMonsters)
            bm.Die();
    }
}
