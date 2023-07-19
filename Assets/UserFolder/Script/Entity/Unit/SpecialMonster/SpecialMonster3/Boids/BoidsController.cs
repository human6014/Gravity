using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
using Manager;

namespace Entity.Unit.Special
{
    public class BoidsController : MonoBehaviour
    {
        #region SerializeField
        [Header("Boid Options")]
        [SerializeField] private BoidsMonster m_BoidUnitPrefab;

        [Tooltip("미리 생성할 유닛 수")]
        [Range(5, 5000)] [SerializeField] private int m_BoidPollingCount = 1000;

        [Tooltip("생성 범위")]
        [Range(5, 100)] [SerializeField] private float m_SpawnRange = 10;

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
        private List<BoidsMonster> m_BoidMonsters = new List<BoidsMonster>();

        private const string m_ActivePoolName = "BoidsPool";

        private bool m_IsTracingPlayer;
        private bool m_IsPatrol;
        private void Awake()
        {
            m_BoidsPool = GameObject.Find(m_ActivePoolName).transform;
            m_TraceOffSeconds = new WaitForSeconds(m_BoidsMonsterTraceTime);
            m_PatrolOffSeconds = new WaitForSeconds(m_PatrolTime);
        }

        private void Start()
        {
            poolingObj = ObjectPoolManager.Register(m_BoidUnitPrefab, m_BoidsPool);
            poolingObj.GenerateObj(m_BoidPollingCount);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine(TracePlayer());
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(PatrolBoids());
            }
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
                currUnit.ReturnAction += ReturnObj;
                currUnit.Init(transform);

                m_BoidMonsters.Add(currUnit);
            }
        }

        public IEnumerator TracePlayer()
        {
            m_IsTracingPlayer = true;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.TracePatternAction.Invoke(m_IsTracingPlayer);

            yield return m_TraceOffSeconds;

            m_IsTracingPlayer = false;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.TracePatternAction.Invoke(m_IsTracingPlayer);
        }

        public IEnumerator PatrolBoids()
        {
            m_IsPatrol = true;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.PatrolPatternAction.Invoke(m_IsPatrol);

            yield return m_PatrolOffSeconds;

            m_IsPatrol = false;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.PatrolPatternAction.Invoke(m_IsPatrol);
        }

        public void ReturnObj(PoolableScript poolableScript)
        {
            m_BoidMonsters.Remove((BoidsMonster)poolableScript);
            poolingObj.ReturnObject(poolableScript);
        }
    }
}

