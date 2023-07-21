using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

namespace Entity.Unit.Flying
{
    public class BoidsMovement : MonoBehaviour
    {
        #region Variables & Initializer
        [Header("Info")]
        [SerializeField] private Scriptable.Monster.BoidsScriptable settings;
        [SerializeField] private ComputeShader m_ComputeShader;
        private readonly List<BoidsMovement> m_Neighbours = new();

        private WaitForSeconds calcEgoWaitSeconds;
        private WaitForSeconds findNeighbourSeconds;

        private Transform m_MoveCenter;
        private Transform m_Target;

        private float m_Speed;
        private float m_AdditionalSpeed;
        private float m_CurrentMaxMovementRange;

        private Vector3 m_TargetVec;
        private Vector3 m_EgoVector;

        private Vector3 m_TargetForwardVec;
        private Vector3 m_BoundsVec;
        #endregion

        private bool m_IsTracePlayer;
        private bool m_IsPatrol;

        public void TryTracePlayer(bool value)
        {
            m_IsTracePlayer = value;
        }

        public void TryPatrol(bool value)
        {
            m_IsPatrol = value;
            m_CurrentMaxMovementRange = m_IsPatrol ? settings.patrolMovementRange : settings.maxMovementRange;
        }

        private void Awake()
        {
            calcEgoWaitSeconds = new WaitForSeconds(Random.Range(1f, 3f));
            findNeighbourSeconds = new WaitForSeconds(Random.Range(1.5f, 2f));
            m_CurrentMaxMovementRange = settings.maxMovementRange;
        }

        private void Start()
        {
            m_Target = AIManager.PlayerTransform;
        }

        public void Init(Transform moveCenter)
        {
            m_MoveCenter = moveCenter;
            m_Speed = Random.Range(settings.speedRange.x, settings.speedRange.y);

            m_Neighbours.Clear();
            StartCoroutine(FindNeighbourCoroutine());
            StartCoroutine(CalculateEgoVectorCoroutine());
        }

        public void CalcAndMove()
        {
            if (m_AdditionalSpeed > 0) m_AdditionalSpeed -= Time.deltaTime;

            // Calculate all the vectors we need
            CalculateVectors(out Vector3 cohesionVector, out Vector3 alignmentVector, out Vector3 separationVector);

            // 추가적인 방향
            if (m_IsTracePlayer && !m_IsPatrol && m_Target != null) //공격 패턴 주기시마다 하게 함
                m_TargetForwardVec = CalculateTargetVector() * settings.targetWeight;
            else m_BoundsVec = CalculateBoundsVector() * settings.boundsWeight;
            CalculateObstacleVector(out Vector3 obstacleVector);

            m_TargetVec = cohesionVector + alignmentVector + separationVector + m_BoundsVec + obstacleVector + (m_EgoVector * settings.egoWeight) + m_TargetForwardVec;

            // Steer and Move
            if (m_TargetVec == Vector3.zero) m_TargetVec = m_EgoVector;
            else m_TargetVec = Vector3.Lerp(transform.forward, m_TargetVec, Time.deltaTime).normalized;

            transform.SetPositionAndRotation(transform.position + (m_Speed + m_AdditionalSpeed) * Time.deltaTime * m_TargetVec,
                                            Quaternion.LookRotation(m_TargetVec));
        }

        #region Calculate Vectors
        private IEnumerator CalculateEgoVectorCoroutine()
        {
            while (true)
            {
                m_Speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
                m_EgoVector = Random.insideUnitSphere;
                yield return calcEgoWaitSeconds;
            }
        }

        private IEnumerator FindNeighbourCoroutine()
        {
            Collider[] colls;
            BoidsMovement neighbour;
            while (true)
            {
                m_Neighbours.Clear();

                colls = Physics.OverlapSphere(transform.position, settings.neighbourDistance, settings.boidUnitLayer);
                for (int i = 0; i < colls.Length && i <= settings.maxNeighbourCount; i++)
                {
                    if (Vector3.Angle(transform.forward, colls[i].transform.position - transform.position) <= settings.FOVAngle)
                    {
                        neighbour = colls[i].GetComponent<BoidsMovement>();
                        
                        m_Neighbours.Add(neighbour);
                    }
                }
                yield return findNeighbourSeconds;
            }
        }
        #region Compute Shader
        //private ComputeBuffer neighbourPositionsBuffer;
        //private ComputeBuffer neighbourForwardsBuffer;
        //private ComputeBuffer cohesionVectorBuffer;
        //private ComputeBuffer alignmentVectorBuffer;
        //private ComputeBuffer separationVectorBuffer;
        //private int kernelID;
        //private void InitSetting()
        //{
        //    kernelID = m_ComputeShader.FindKernel("CalculateVectors");

        //    neighbourPositionsBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        //    neighbourForwardsBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        //    cohesionVectorBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        //    alignmentVectorBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        //    separationVectorBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);

        //    // Set data to buffers (example: random positions and forwards)
        //    Vector3[] neighbourPositions = new Vector3[boidCount];
        //    Vector3[] neighbourForwards = new Vector3[boidCount];
        //    for (int i = 0; i < m_Neighbours.Count; i++)
        //    {
        //        neighbourPositions[i] = m_Neighbours[i].transform.position;
        //        neighbourForwards[i] = m_Neighbours[i].transform.forward;
        //    }
        //    neighbourPositionsBuffer.SetData(neighbourPositions);
        //    neighbourForwardsBuffer.SetData(neighbourForwards);

        //    m_ComputeShader.SetBuffer(kernelID, "neighbourPositions", neighbourPositionsBuffer);
        //    m_ComputeShader.SetBuffer(kernelID, "neighbourForwards", neighbourForwardsBuffer);
        //    m_ComputeShader.SetBuffer(kernelID, "cohesionVector", cohesionVectorBuffer);
        //    m_ComputeShader.SetBuffer(kernelID, "alignmentVector", alignmentVectorBuffer);
        //    m_ComputeShader.SetBuffer(kernelID, "separationVector", separationVectorBuffer);
        //}

        //private void Dispatch()
        //{
        //    Vector3 weights = new Vector3(settings.cohesionWeight, settings.alignmentWeight, settings.separationWeight);
        //    m_ComputeShader.SetVector("settings", weights);

        //    // Dispatch compute shader
        //    m_ComputeShader.Dispatch(kernelID, boidCount / 16, 1, 1);

        //    // Read data from compute buffers if needed
        //    Vector3[] cohesionVectors = new Vector3[boidCount];
        //    Vector3[] alignmentVectors = new Vector3[boidCount];
        //    Vector3[] separationVectors = new Vector3[boidCount];
        //    cohesionVectorBuffer.GetData(cohesionVectors);
        //    alignmentVectorBuffer.GetData(alignmentVectors);
        //    separationVectorBuffer.GetData(separationVectors);
        //}
        #endregion
        private void CalculateVectors(out Vector3 cohesionVector, out Vector3 alignmentVector, out Vector3 separationVector)
        {
            cohesionVector = Vector3.zero;
            alignmentVector = transform.forward;
            separationVector = Vector3.zero;
            if (m_Neighbours.Count > 0)
            {
                // 이웃 unit들의 위치 더하기
                for (int i = 0; i < m_Neighbours.Count; i++)
                {
                    cohesionVector += m_Neighbours[i].transform.position;
                    alignmentVector += m_Neighbours[i].transform.forward;
                    separationVector += (transform.position - m_Neighbours[i].transform.position);
                }

                // 중심 위치로의 벡터 찾기
                cohesionVector /= m_Neighbours.Count;
                alignmentVector /= m_Neighbours.Count;
                separationVector /= m_Neighbours.Count;
                cohesionVector -= transform.position;

                cohesionVector.Normalize();
                alignmentVector.Normalize();
                separationVector.Normalize();
            }

            cohesionVector *= settings.cohesionWeight;
            alignmentVector *= settings.alignmentWeight;
            separationVector *= settings.separationWeight;
        }

        private Vector3 CalculateBoundsVector()
        {
            m_TargetForwardVec = Vector3.zero;
            Vector3 offsetToCenter = m_MoveCenter.position - transform.position;
            return offsetToCenter.magnitude >= m_CurrentMaxMovementRange ? offsetToCenter.normalized : Vector3.zero;
        }

        private void CalculateObstacleVector(out Vector3 obstacleVector)
        {
            obstacleVector = Vector3.zero;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, settings.obstacleDistance, settings.obstacleLayer))
            {
                obstacleVector = hit.normal * settings.obstacleWeight;
                m_AdditionalSpeed = 10;
            }
        }

        private Vector3 CalculateTargetVector()
        {
            m_BoundsVec = Vector3.zero;
            return (m_Target.position - transform.position).normalized;
        }
        #endregion

        public void Dispose()
        {
            StopAllCoroutines();
        }
    }
}
