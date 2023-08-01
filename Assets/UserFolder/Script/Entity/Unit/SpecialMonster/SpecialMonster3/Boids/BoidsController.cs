using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
using System.Linq;
using Manager;

namespace Entity.Unit.Special
{
    public struct BoidData
    {
        public Vector3 m_Position;
        public Vector3 m_Foward;

        public Vector3 cohesionVector;
        public Vector3 alignmentVector;
        public Vector3 separationVector;
        public static int Size { get => sizeof(float) * 3 * 5; }
    }

    public struct BoidOutput
    {
        public Vector3 cohesionVector;
        public Vector3 alignmentVector;
        public Vector3 separationVector;
        public static int Size { get => sizeof(float) * 3 * 3; }
    }

    public class BoidsController : MonoBehaviour
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

        [Header("Test ComputeShader")]
        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private float m_CohesionWeight = 3;
        [SerializeField] private float m_AlighnmentWeight = 5;
        [SerializeField] private float m_SeparationWeight = 4;
        [SerializeField] private float m_NeighbourDist = 3;     //Awake에서 pow해줌
        #endregion

        private ObjectPoolManager.PoolingObject poolingObj;
        private Transform m_BoidsPool;
        private WaitForSeconds m_TraceOffSeconds;
        private WaitForSeconds m_PatrolOffSeconds;

        private BoidData[] m_BoidData;
        private ComputeBuffer m_ComputeBuffer;

        private readonly List<BoidsMonster> m_BoidMonsters = new List<BoidsMonster>();
        private readonly List<BoidsMovement> m_BoidMovement = new List<BoidsMovement>();
        private readonly System.Random m_MyRandom = new System.Random();

        private const string m_ActivePoolName = "BoidsPool";
        private const int m_ThreadGroupSize = 1024;

        private const int m_TraceDividingCount = 100;

        private bool m_IsAlive;
        public bool IsTraceAndBackPlayer { get; private set; }
        public bool IsPatrolBoids { get; private set; }

        public System.Action<int> ReturnChildObject { get; set; }

        private void Awake()
        {
            m_BoidsPool = GameObject.Find(m_ActivePoolName).transform;
            m_TraceOffSeconds = new WaitForSeconds(m_BoidsMonsterTraceTime);
            m_PatrolOffSeconds = new WaitForSeconds(m_PatrolTime);

            m_NeighbourDist *= m_NeighbourDist;
            m_ComputeShader.SetFloat("detectDist", m_NeighbourDist);
            m_ComputeShader.SetFloat("cohesionWeight", m_CohesionWeight);
            m_ComputeShader.SetFloat("alignmentWeight", m_AlighnmentWeight);
            m_ComputeShader.SetFloat("separationWeight", m_SeparationWeight);
        }

        public void Init(float traceTime, float patrolTime)
        {
            m_IsAlive = true;

            m_TraceOffSeconds = new WaitForSeconds(traceTime);
            m_PatrolOffSeconds = new WaitForSeconds(patrolTime);

            poolingObj = ObjectPoolManager.Register(m_BoidUnitPrefab, m_BoidsPool);
            poolingObj.GenerateObj(m_BoidPollingCount);
        }

        public void BoidsDispatch()
        {
            int numBoids = m_BoidMovement.Count;
            if (numBoids <= 1) return;

            if (m_BoidData == null || m_BoidData.Length != numBoids)
                m_BoidData = new BoidData[numBoids];


            for (int i = 0; i < numBoids; i++)
            {
                m_BoidData[i].m_Position = m_BoidMovement[i].transform.position;
                m_BoidData[i].m_Foward = m_BoidMovement[i].transform.forward;
            }

            m_ComputeBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            m_ComputeBuffer.SetData(m_BoidData);
            m_ComputeShader.SetBuffer(0, "boidInfo", m_ComputeBuffer);
            m_ComputeShader.SetInt("numberBoids", numBoids);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)m_ThreadGroupSize);
            m_ComputeShader.Dispatch(0, threadGroups, 1, 1);

            m_ComputeBuffer.GetData(m_BoidData);

            for (int i = 0; i < m_BoidMovement.Count; i++)
            {
                m_BoidMovement[i].CohesionVector = m_BoidData[i].cohesionVector;
                m_BoidMovement[i].AlignmentVector = m_BoidData[i].alignmentVector;
                m_BoidMovement[i].SeparationVector = m_BoidData[i].separationVector;
            }

            m_ComputeBuffer.Release();
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
                currUnit.Init(transform);

                m_BoidMonsters.Add(currUnit);
                m_BoidMovement.Add(currUnit.GetComponent<BoidsMovement>());
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

            for(int i = 0; i < randomIndex.Length; i++)
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
            m_BoidMovement.Remove(poolableScript.GetComponent<BoidsMovement>());
            m_BoidMonsters.Remove((BoidsMonster)poolableScript);
            poolingObj.ReturnObject(poolableScript);
        }

        public void Dispose()
        {
            m_IsAlive = false;
            StopAllCoroutines();

            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.Die();
        }

        private void OnDestroy()
        {
            if (m_ComputeBuffer != null) m_ComputeBuffer.Release();
        }
    }
}