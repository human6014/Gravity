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
        [SerializeField] private float castHeight = 1.9f;
        [SerializeField] private float castRadius = 0.5f;

        
        private NavMeshAgent m_NavMeshAgent;
        private Rigidbody m_Rigidbody;

        private const float m_MaximumFallingTime = 10;
        private float m_FallingTimer;

        public NormalMonsterState NormalMonsterState { get; set; }
        public bool IsBatch { get; private set; } = false;
        public bool IsFalling { get; private set; } = false;
        public bool IsAutoMode { get; private set; } = true;
        public bool IsClimbing { get; private set; } = false;
        public bool IsMalfunction { get; private set; } = false;
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
            IsBatch = true;
            m_FallingTimer = 0;
            IsMalfunction = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.Warp(pos);
            transform.rotation = Quaternion.LookRotation(transform.forward, -GravityManager.GravityVector);
        }

        public bool CheckCanBehaviorState()
        {
            if (!IsBatch) return false;

            if (GravityManager.IsGravityChanging)
            {
                SetFallingMode(true);
                return false;
            }
            if (IsFalling)
            {
                DetectMalfunction();
                return false;
            }
            else m_FallingTimer = 0;

            if (!IsAutoMode) return false;
            return true;
        }

        private void FixedUpdate()
        {
            if (!IsBatch) return;
            if (IsFalling) DetectWalkableArea();
        }

        private void DetectWalkableArea()
        {
            if (Physics.SphereCast(new Ray(transform.position, transform.up), castRadius, castHeight, climbingDetectLayer))
                SetFallingMode(false);
        }

        private void SetFallingMode(bool isAutoMode)
        {
            m_Rigidbody.useGravity = isAutoMode;
            m_Rigidbody.isKinematic = !isAutoMode;
            if (isAutoMode && !IsFalling)
            {
                RagdollOnOffAction?.Invoke(true);
            }
            else if (!isAutoMode && IsFalling)
            {
                RagdollOnOffAction?.Invoke(false);
            }

            IsFalling = isAutoMode;

            m_NavMeshAgent.enabled = !isAutoMode;
            IsAutoMode = !isAutoMode;
            if (IsAutoMode)
            {
                NormalMonsterState.SetBoolIdle();
                NormalMonsterState.SetTriggerGettingUp();
            }
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
                if (!IsClimbing) Debug.Log("Climbing Start");
                else SetClimbingRotation();
                IsClimbing = true;
            }
            else
            {
                if (IsClimbing) NormalMonsterState.SetTriggerGettingUp();
                else SetNormalRotation(isMoving);
                IsClimbing = false;
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
            Vector3 autoTargetDir = (target - transform.position).normalized;

            switch (GravityManager.m_CurrentGravityAxis)
            {
                case EnumType.GravityDirection.X:
                    autoTargetDir.x = 0;
                    break;
                case EnumType.GravityDirection.Y:
                    autoTargetDir.y = 0;
                    break;
                case EnumType.GravityDirection.Z:
                    autoTargetDir.z = 0;
                    break;
            }
            transform.rotation = Quaternion.LookRotation(autoTargetDir, -GravityManager.GravityVector);
        }
        #endregion

        public void Dispose()
        {
            m_NavMeshAgent.enabled = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            IsBatch = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, castRadius);
            Gizmos.DrawSphere(transform.position + transform.up * castHeight, castRadius);
        }
    }
}
