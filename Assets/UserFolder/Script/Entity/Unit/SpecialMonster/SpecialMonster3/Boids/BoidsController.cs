using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
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
        [Range(5, 5000)] [SerializeField] private int m_BoidPollingCount = 1000;

        [Tooltip("생성 범위")]
        [Range(5, 100)] [SerializeField] private float m_SpawnRange = 10;

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
        private List<BoidsMonster> m_BoidMonsters = new List<BoidsMonster>();
        private List<BoidsMovement> m_BoidMovement = new List<BoidsMovement>();

        private const string m_ActivePoolName = "BoidsPool";
        private const int m_ThreadGroupSize = 1024;

        private bool m_IsTracingPlayer;
        private bool m_IsPatrol;

        private BoidData[] m_BoidData;
        private ComputeBuffer m_ComputeBuffer;
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

        private void Start()
        {
            poolingObj = ObjectPoolManager.Register(m_BoidUnitPrefab, m_BoidsPool);
            poolingObj.GenerateObj(m_BoidPollingCount);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                StartCoroutine(TracePlayer());
            else if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PatrolBoids());

            Dispatch();
        }

        private void Dispatch()
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
                m_BoidMovement.Add(currUnit.GetComponent<BoidsMovement>());
            }
        }

        private void OnDestroy()
        {
            if (m_ComputeBuffer != null) m_ComputeBuffer.Release();
        }

        #region Pattern
        public IEnumerator TracePlayer()
        {
            m_IsTracingPlayer = true;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.TracePatternAction?.Invoke(m_IsTracingPlayer);

            yield return m_TraceOffSeconds;

            m_IsTracingPlayer = false;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.TracePatternAction?.Invoke(m_IsTracingPlayer);
        }

        public IEnumerator PatrolBoids()
        {
            m_IsPatrol = true;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.PatrolPatternAction?.Invoke(m_IsPatrol);

            yield return m_PatrolOffSeconds;

            m_IsPatrol = false;
            foreach (BoidsMonster bm in m_BoidMonsters)
                bm.PatrolPatternAction?.Invoke(m_IsPatrol);
        }
        #endregion
        public void ReturnObj(PoolableScript poolableScript)
        {
            m_BoidMovement.Remove(poolableScript.GetComponent<BoidsMovement>());
            m_BoidMonsters.Remove((BoidsMonster)poolableScript);
            poolingObj.ReturnObject(poolableScript);
        }
    }
}

