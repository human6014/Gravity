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

        private WaitForSeconds calcEgoWaitSeconds;
        private WaitForSeconds findNeighbourSeconds;
        private WaitForSeconds calcObstacleWaitSeconds;

        private Transform m_MoveCenter;
        private Transform m_Target;

        private float m_Speed;
        private float m_AdditionalSpeed;
        private float m_CurrentMaxMovementRange;

        private Vector3 m_TargetVec;
        private Vector3 m_EgoVector;

        private Vector3 m_ObstacleVector;
        private Vector3 m_TargetForwardVec;
        private Vector3 m_BoundsVec;
        #endregion

        private bool m_IsTracePlayer;
        private bool m_IsPatrol;

        public Vector3 CohesionVector { get; set; }
        public Vector3 AlignmentVector { get; set; }
        public Vector3 SeparationVector { get; set; }

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
            findNeighbourSeconds = new WaitForSeconds(Random.Range(1.5f,2f));
            calcObstacleWaitSeconds = new WaitForSeconds(Random.Range(0.05f,0.1f));
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

            //StartCoroutine(FindNeighbourCoroutine());
            StartCoroutine(CalculateEgoVectorCoroutine());
            StartCoroutine(CalculateObstacleVectorCoroutine());
        }

        public void CalcAndMove()
        {
            if (m_AdditionalSpeed > 0) m_AdditionalSpeed -= Time.deltaTime;

            // Calculate all the vectors we need
            //CalculateVectors();

            // 추가적인 방향
            if (m_IsTracePlayer && !m_IsPatrol && m_Target != null)
                m_TargetForwardVec = CalculateTargetVector() * settings.targetWeight;
            else m_BoundsVec = CalculateBoundsVector() * settings.boundsWeight;

            m_TargetVec = CohesionVector + AlignmentVector + SeparationVector + 
                m_BoundsVec + m_ObstacleVector + (m_EgoVector * settings.egoWeight) + m_TargetForwardVec;

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

        private IEnumerator CalculateObstacleVectorCoroutine()
        {
            while (true)
            {
                m_ObstacleVector = Vector3.zero;
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, settings.obstacleDistance, settings.obstacleLayer))
                {
                    m_ObstacleVector = hit.normal * settings.obstacleWeight;
                    m_AdditionalSpeed = 8;
                }
                yield return calcObstacleWaitSeconds;
            }
        }

        private Vector3 CalculateBoundsVector()
        {
            m_TargetForwardVec = Vector3.zero;
            Vector3 offsetToCenter = m_MoveCenter.position - transform.position;
            return offsetToCenter.magnitude >= m_CurrentMaxMovementRange ? offsetToCenter.normalized : Vector3.zero;
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

        #region CPU version
        
        List<BoidsMovement> m_Neighbours = new List<BoidsMovement>();
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

        private void CalculateVectors()
        {
            CohesionVector = Vector3.zero;
            AlignmentVector = transform.forward;
            SeparationVector = Vector3.zero;

            if (m_Neighbours.Count > 0)
            {
                for (int i = 0; i < m_Neighbours.Count; i++)
                {
                    CohesionVector += m_Neighbours[i].transform.position;
                    AlignmentVector += m_Neighbours[i].transform.forward;
                    SeparationVector += (transform.position - m_Neighbours[i].transform.position);
                }

                CohesionVector /= m_Neighbours.Count;
                AlignmentVector /= m_Neighbours.Count;
                SeparationVector /= m_Neighbours.Count;
                CohesionVector -= transform.position;

                CohesionVector.Normalize();
                AlignmentVector.Normalize();
                SeparationVector.Normalize();
            }

            CohesionVector *= settings.cohesionWeight;
            AlignmentVector *= settings.alignmentWeight;
            SeparationVector *= settings.separationWeight;
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
        
        #endregion
    }
}
