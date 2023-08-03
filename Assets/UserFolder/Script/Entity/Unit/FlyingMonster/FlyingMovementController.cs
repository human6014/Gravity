using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Unit.Flying
{
    public class FlyingMovementController : MonoBehaviour
    {
        [Tooltip("타겟과 직선 상 감지 레이어")]
        [SerializeField] private LayerMask playerSeeLayerMask = -1;

        [Tooltip("")]
        [SerializeField] private float maxDistanceRebuildPath = 1;

        [Tooltip("가속도")]
        [SerializeField] private float acceleration = 1;

        [Tooltip("이동 노드 최소 도달 거리")]
        [SerializeField] private float minReachDistance = 2f;

        [Tooltip("최종 노드 도달 거리")]
        [SerializeField] private float minFollowDistance = 4f;

        [Tooltip("")]
        [SerializeField] private float pathPointRadius = 0.2f;

        private Octree m_Octree;
        private Octree.PathRequest m_OldPath;
        private Octree.PathRequest m_NewPath;

        private Transform m_Target;
        private GameObject m_PlayerObject;
        private Rigidbody m_Rigidbody;
        private SphereCollider m_SphereCollider;

        private Vector3 m_CurrentDestination;
        private Vector3 m_LastDestination;
        private Vector3 m_RandomPos;

        private bool isAlive;
        private bool CanSeePlayer()
        {
            if (Physics.Raycast(transform.position, transform.position - m_Target.position, out RaycastHit hit, Vector3.Distance(transform.position, m_Target.position) + 1, playerSeeLayerMask))
                return hit.transform.gameObject == m_PlayerObject;
            return false;
        }

        private Octree.PathRequest Path
        {
            get
            {
                if ((m_NewPath == null || m_NewPath.isCalculating) && m_OldPath != null) return m_OldPath;
                return m_NewPath;
            }
        }

        public bool AttackableToTarget
        {
            get => Path != null && Path.PathList.Count <= 1;
        }

        public bool CloseToTarget
        {
            get => Path != null && Path.PathList.Count > 3;
        }

        public bool HasTarget
        {
            get => Path != null && Path.PathList.Count > 0;
        }

        public Vector3 CurrentTargetPosition
        {
            get
            {
                if (Path != null && Path.PathList.Count > 0) return m_CurrentDestination;
                return m_Target.position;
            }
        }

        private void Awake()
        {
            m_SphereCollider = GetComponent<SphereCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Octree = FindObjectOfType<Octree>();

            m_PlayerObject = Manager.AI.AIManager.PlayerTransform.gameObject;
            m_Target = Manager.AI.AIManager.PlayerSupportTargetTransform;
        }

        public void Init()
        {
            m_RandomPos = UnityEngine.Random.insideUnitSphere * 3;

            m_LastDestination = m_Target.position;
            m_OldPath = m_NewPath;
            m_NewPath = m_Octree.GetPath(transform.position, m_LastDestination + m_RandomPos);

            isAlive = true;
        }

        public void MoveCurrentTarget()
        {
            if (!isAlive) return;
            if ((m_NewPath == null || !m_NewPath.isCalculating) && Vector3.SqrMagnitude(m_Target.position - m_LastDestination) > maxDistanceRebuildPath &&
                (!CanSeePlayer() || Vector3.Distance(m_Target.position, transform.position) > minFollowDistance) && !m_Octree.IsBuilding)
            {
                m_LastDestination = m_Target.position;

                m_OldPath = m_NewPath;
                m_NewPath = m_Octree.GetPath(transform.position, m_LastDestination + m_RandomPos);
            }

            var curPath = Path;

            if (!curPath.isCalculating && curPath != null && curPath.PathList.Count > 0)
            {
                if (Vector3.Distance(transform.position, m_Target.position) < minFollowDistance && CanSeePlayer())
                    curPath.Reset();

                m_CurrentDestination = curPath.PathList[0] + Vector3.ClampMagnitude(m_Rigidbody.position - curPath.PathList[0], pathPointRadius);

                m_Rigidbody.velocity += acceleration * Time.deltaTime * Vector3.ClampMagnitude(m_CurrentDestination - transform.position, 1);
                float sqrMinReachDistance = minReachDistance * minReachDistance;

                Vector3 predictedPosition = m_Rigidbody.position + m_Rigidbody.velocity * Time.deltaTime;
                float shortestPathDistance = Vector3.SqrMagnitude(predictedPosition - m_CurrentDestination);
                int shortestPathPoint = 0;

                float sqrDistance;
                float sqrPredictedDistance;
                for (int i = 0; i < curPath.PathList.Count; i++)
                {
                    sqrDistance = Vector3.SqrMagnitude(m_Rigidbody.position - curPath.PathList[i]);
                    if (sqrDistance <= sqrMinReachDistance)
                    {
                        if (i < curPath.PathList.Count) curPath.PathList.RemoveRange(0, i + 1);

                        shortestPathPoint = 0;
                        break;
                    }

                    sqrPredictedDistance = Vector3.SqrMagnitude(predictedPosition - curPath.PathList[i]);
                    if (sqrPredictedDistance < shortestPathDistance)
                    {
                        shortestPathDistance = sqrPredictedDistance;
                        shortestPathPoint = i;
                    }
                }

                if (shortestPathPoint > 0) curPath.PathList.RemoveRange(0, shortestPathPoint);
            }
            else m_Rigidbody.velocity -= acceleration * Time.deltaTime * m_Rigidbody.velocity;

        }

        public void Dispose()
        {
            isAlive = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (m_Rigidbody != null)
            {
                Gizmos.color = Color.blue;
                Vector3 predictedPosition = m_Rigidbody.position + m_Rigidbody.velocity * Time.deltaTime;
                Gizmos.DrawWireSphere(predictedPosition, m_SphereCollider.radius);
            }

            if (Path != null)
            {
                var path = Path;
                for (int i = 0; i < path.PathList.Count - 1; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(path.PathList[i], minReachDistance);
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(path.PathList[i], Vector3.ClampMagnitude(m_Rigidbody.position - path.PathList[i], pathPointRadius));
                    Gizmos.DrawWireSphere(path.PathList[i], pathPointRadius);
                    Gizmos.DrawLine(path.path[i], path.PathList[i + 1]);
                }
            }
        }
    }
}
