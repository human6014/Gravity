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
        [SerializeField] private float castHeight = 1.9f;
        [SerializeField] private float castRadius = 0.5f;
        
        private NavMeshAgent m_NavMeshAgent;
        private Rigidbody m_Rigidbody;
        private Vector3 m_WarpPos;

        private const float m_MaximumFallingTime = 10;
        private float m_FallingTimer;

        private bool m_GravityChangeFlag;
        private bool m_isBatch;
        private bool m_isFalling;
        private bool m_isAutoMode = true;
        private bool m_isClimbing;

        public bool IsMalfunction { get; private set; }
        public NormalMonsterState NormalMonsterState { get; set; }
        public Action<bool> RagdollOnOffAction { get; set; }

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Rigidbody = GetComponent<Rigidbody>();

            m_NavMeshAgent.updateRotation = false;
            m_NavMeshAgent.updateUpAxis = false;
        }

        public void Init(Vector3 pos)
        {
            m_isBatch = true;
            m_FallingTimer = 0;
            IsMalfunction = false;
            m_isFalling = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.Warp(pos);
            transform.rotation = Quaternion.LookRotation(transform.forward, -GravityManager.GravityVector);
        }

        public bool CheckCanBehaviorState()
        {
            if (!m_isBatch) return false;

            if (GravityManager.IsGravityChanging && !m_GravityChangeFlag)
            {
                m_GravityChangeFlag = true;
                SetFallingMode(true);
                return false;
            }
            if (!GravityManager.IsGravityChanging) m_GravityChangeFlag = false;

            if (m_isFalling)
            {
                DetectMalfunction();
                return false;
            }
            else m_FallingTimer = 0;

            if (!m_isAutoMode) return false;
            return true;
        }

        private void FixedUpdate()
        {
            if (!m_isBatch) return;
            if (m_isFalling) DetectWalkableArea();
        }
        
        private void DetectWalkableArea()
        {
            if (Physics.Raycast(transform.position + transform.up * 1.5f, GravityManager.GravityVector, out RaycastHit hitInfo, 1, m_FallingDetectLayer))
            {
                //Need normal check
                m_WarpPos = hitInfo.point;
                SetFallingMode(false);
                NormalMonsterState.SetBoolIdle();
                NormalMonsterState.SetTriggerGettingUp();
                m_NavMeshAgent.Warp(hitInfo.point);
            }
        }

        private void SetFallingMode(bool isAutoMode)
        {
            m_Rigidbody.useGravity = isAutoMode;
            m_Rigidbody.isKinematic = !isAutoMode;
            RagdollOnOffAction?.Invoke(isAutoMode);
            m_NavMeshAgent.enabled = !isAutoMode;

            m_isFalling = isAutoMode;
            m_isAutoMode = !isAutoMode;
        }

        private void DetectMalfunction()
        {
            if ((m_FallingTimer += Time.deltaTime) >= m_MaximumFallingTime) IsMalfunction = true;
        }

        #region Nav Move
        public void AutoMode()
        {
            bool isMoving = true;
            if (!NormalMonsterState.CanMoveState)
            {
                m_NavMeshAgent.SetDestination(transform.position);
                isMoving = false;
            }
            else m_NavMeshAgent.SetDestination(AIManager.PlayerTransform.position);

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
            NormalMonsterState.SetBoolWalking();
            Vector3 target = isMoving ? m_NavMeshAgent.steeringTarget : AIManager.PlayerTransform.position;
            Vector3 autoTargetDir = AIManager.CurrentTargetPosition((target - transform.position).normalized);

            if (autoTargetDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(autoTargetDir, -GravityManager.GravityVector);
        }
        #endregion

        public void Dispose()
        {
            m_NavMeshAgent.enabled = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_isBatch = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 startPos = transform.position + transform.up * 2;
            Vector3 endPos = startPos + (GravityManager.GravityVector * 1);
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
