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

        private Transform m_MoveCenter;
        private Transform m_Target;

        private float m_Speed;
        private float m_AdditionalSpeed;
        private float m_PatternSpeed;
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
            m_PatternSpeed = m_IsTracePlayer ? -2 : 0;
        }

        public void TryPatrol(bool value)
        {
            m_IsPatrol = value;
            m_CurrentMaxMovementRange = m_IsPatrol ? settings.patrolMovementRange : settings.maxMovementRange;
            m_PatternSpeed = m_IsPatrol ? 4 : 0;
        }

        private void Awake()
        {
            calcEgoWaitSeconds = new WaitForSeconds(Random.Range(1f, 3f));
            m_CurrentMaxMovementRange = settings.maxMovementRange;
        }

        private void Start()
        {
            m_Target = AIManager.PlayerTransform;
        }

        public void Init(Transform moveCenter, bool isTrace, bool isPatrol)
        {
            m_MoveCenter = moveCenter;
            m_Speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
            TryTracePlayer(isTrace);
            TryPatrol(isPatrol);

            //StartCoroutine(FindNeighbourCoroutine());
            StartCoroutine(CalculateEgoVectorCoroutine());
            //StartCoroutine(CalculateObstacleVectorCoroutine());
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
            CalculateObstacleVector(out Vector3 obstacleVector);

            m_TargetVec = CohesionVector + AlignmentVector + SeparationVector + 
                m_BoundsVec + obstacleVector + (m_EgoVector * settings.egoWeight) + m_TargetForwardVec;

            if (m_TargetVec == Vector3.zero) m_TargetVec = m_EgoVector;
            else m_TargetVec = Vector3.Lerp(transform.forward, m_TargetVec, Time.deltaTime).normalized;

            transform.SetPositionAndRotation(transform.position + (m_Speed + m_AdditionalSpeed + m_PatternSpeed) * Time.deltaTime * m_TargetVec,
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

        private void CalculateObstacleVector(out Vector3 obstacleVector)
        {
            obstacleVector = Vector3.zero;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, settings.obstacleDistance, settings.obstacleLayer))
            {
                obstacleVector = hit.normal * settings.obstacleWeight;
                m_AdditionalSpeed = 10;
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
    }
}