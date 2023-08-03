using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using Manager.AI;
using Manager;

namespace Entity.Unit.Normal
{
    public class NormalMonsterAI : MonoBehaviour
    {
        [SerializeField] private LayerMask climbingDetectLayer;
        [SerializeField] private LayerMask m_FallingDetectLayer;

        [SerializeField] private float m_RayStartY = 1.5f;
        [SerializeField] private float m_RayDist = 1;
        
        private NavMeshAgent m_NavMeshAgent;
        private Rigidbody m_Rigidbody;

        private const float m_MaximumFallingTime = 10;
        private float m_FallingTimer;

        private bool m_GravityChangeFlag;
        private bool m_isBatch;
        private bool m_isAutoMode;
        private bool m_isClimbing;
        private bool m_CanRunning;

        public bool IsFalling { get; private set; }
        public NormalMonsterState NormalMonsterState { get; set; }
        public Action<bool> RagdollOnOffAction { get; set; }
        private bool RigidSet
        {
            set
            {
                m_Rigidbody.isKinematic = value;
                m_Rigidbody.useGravity = !value;
            }
        }

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Rigidbody = GetComponent<Rigidbody>();

            m_NavMeshAgent.updateRotation = false;
            m_NavMeshAgent.updateUpAxis = false;
        }

        public void Init(Vector3 pos, bool CanRunning, float movementSpeed)
        {
            m_FallingTimer = 0;
            m_isBatch = true;
            IsFalling = false;
            m_isAutoMode = true;
            m_isClimbing = false;
            m_CanRunning = CanRunning;
            m_NavMeshAgent.speed = movementSpeed;

            RigidSet = true;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.Warp(pos);
            transform.rotation = Quaternion.LookRotation(transform.forward, -GravityManager.GravityVector);
        }

        public bool CheckCanBehaviorState(out bool isMalfunction)
        {
            isMalfunction = false;
            if (!m_isBatch) return false;

            if (GravityManager.IsGravityChanging && !m_GravityChangeFlag)
            {
                m_GravityChangeFlag = true;
                SetFallingMode(true);
                return false;
            }
            else if (!GravityManager.IsGravityChanging) m_GravityChangeFlag = false;

            if (IsFalling)
            {
                if ((m_FallingTimer += Time.deltaTime) >= m_MaximumFallingTime) isMalfunction = true;
                return false;
            }
            else m_FallingTimer = 0;

            return m_isAutoMode;
        }

        private void FixedUpdate()
        {
            if (IsFalling) DetectWalkableArea();
        }
        
        private void DetectWalkableArea()
        {
            if (Physics.Raycast(transform.position + transform.up * m_RayStartY, GravityManager.GravityVector, out RaycastHit hitInfo, m_RayDist, m_FallingDetectLayer))
            {
                //Need normal check
                if (m_isBatch)
                {
                    SetFallingMode(false);
                    NormalMonsterState.SetBoolIdle();
                    NormalMonsterState.SetTriggerGettingUp();
                    m_NavMeshAgent.Warp(hitInfo.point);
                }
                else RigidSet = true;
            }
        }

        private void SetFallingMode(bool isAutoMode)
        {
            RigidSet = !isAutoMode;

            m_NavMeshAgent.enabled = !isAutoMode;
            RagdollOnOffAction?.Invoke(isAutoMode);
            
            IsFalling = isAutoMode;
            m_isAutoMode = !isAutoMode;
        }

        #region Nav Move
        public void AutoBehavior()
        {
            bool isMoving = true;
            if (!NormalMonsterState.CanMoveState)
            {
                m_NavMeshAgent.SetDestination(transform.position);
                isMoving = false;
            }
            else m_NavMeshAgent.SetDestination(AIManager.PlayerGroundPosition);

            if (!m_NavMeshAgent.isOnOffMeshLink && !AIManager.IsSameFloor(m_NavMeshAgent))
            {
                if (!m_isClimbing) Debug.Log("Climbing Start");
                else SetClimbingRotation();
                m_isClimbing = true;
            }
            else
            {
                if (m_isClimbing) NormalMonsterState.SetTriggerGettingUp();
                else SetNormalRotation(isMoving);
                m_isClimbing = false;
            }
        }

        private void SetClimbingRotation()
        {
            NormalMonsterState.SetBoolCrawling();
            transform.rotation = Quaternion.LookRotation(-GravityManager.GravityVector, -(m_NavMeshAgent.navMeshOwner as Component).transform.position);
        }

        private void SetNormalRotation(bool isMoving)
        {
            if (m_CanRunning) NormalMonsterState.SetBoolRunning();
            else NormalMonsterState.SetBoolWalking();
            Vector3 target = isMoving ? m_NavMeshAgent.steeringTarget : AIManager.PlayerTransform.position;
            Vector3 autoTargetDir = AIManager.GetCurrentGravityDirection((target - transform.position));

            if (autoTargetDir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(autoTargetDir, -GravityManager.GravityVector);
                transform.rotation = Quaternion.Slerp(transform.rotation,lookRot, 0.5f);
            }
        }
        #endregion

        public void Dispose()
        {
            if (!IsFalling) RigidSet = true;
            m_NavMeshAgent.enabled = false;
            m_isBatch = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 startPos = transform.position + transform.up * m_RayStartY;
            Vector3 endPos = startPos + (Vector3.up * m_RayDist);
            Gizmos.DrawLine(startPos, endPos);

            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, 1);
        }
    }
}
