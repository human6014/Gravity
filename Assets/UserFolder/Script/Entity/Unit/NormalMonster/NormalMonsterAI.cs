using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using Manager.AI;
using Manager;


public class NormalMonsterAI : MonoBehaviour
{
    [SerializeField] private LayerMask climbingDetectLayer;
    [SerializeField] private float castHeight = 1.9f;
    [SerializeField] private float castRadius = 0.5f;

    private NavMeshAgent m_NavMeshAgent;
    private Rigidbody m_Rigidbody;

    private const float m_MaximumFallingTime = 10;
    private float m_FallingTimer;
    private bool m_WasNavMeshLink;
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
        m_NavMeshAgent.enabled = true;
        m_NavMeshAgent.Warp(pos);
        transform.rotation = Quaternion.LookRotation(transform.forward, -GravityManager.GravityVector);
    }

    public void Move()
    {
        if (!IsBatch) return;

        if (GravityManager.IsGravityChanging)
        {
            SetFallingMode(true);
            return;
        }
        if (IsFalling)
        {
            DetectWalkableArea();
            DetectMalfunction();
            return;
        }

        if (IsAutoMode) AutoMode();
    }

    private void DetectWalkableArea()
    {
        if (Physics.SphereCast(new Ray(transform.position, transform.up), castRadius, castHeight, climbingDetectLayer))
            SetFallingMode(false);
    }

    private void SetFallingMode(bool isAutoMode)
    {
        if (isAutoMode && !IsFalling) RagdollOnOffAction?.Invoke(true);
        else if (!isAutoMode && IsFalling) RagdollOnOffAction?.Invoke(false);
        
        IsFalling = isAutoMode;
        m_Rigidbody.useGravity = isAutoMode;
        m_Rigidbody.isKinematic = !isAutoMode;
        m_NavMeshAgent.enabled = !isAutoMode;
        IsAutoMode = !isAutoMode;
    }

    private void DetectMalfunction()
    {
        m_FallingTimer += Time.deltaTime;
        if (m_FallingTimer >= m_MaximumFallingTime) IsMalfunction = true; 
    }

    #region Nav Move
    private void AutoMode()
    {
        m_FallingTimer = 0;
        m_NavMeshAgent.SetDestination(AIManager.PlayerTransform.position);

        Quaternion autoTargetRot = Quaternion.identity;
        if (!m_NavMeshAgent.isOnOffMeshLink && !AIManager.IsSameFloor(m_NavMeshAgent)) 
            ClimbingMove(ref autoTargetRot);
        else NormalMove(ref autoTargetRot);
        
        transform.rotation = autoTargetRot;
        m_WasNavMeshLink = m_NavMeshAgent.isOnOffMeshLink;
        //Climbing 시작, 끝부분 잡아내야함
        //-> 애니메이션 연동 + GettingUp 등에서 정지상태로 해야 하기 때문
    }

    private void ClimbingMove(ref Quaternion autoTargetRot)
    {
        if (!m_WasNavMeshLink && !IsClimbing) Debug.Log("StartClimbing");
        IsClimbing = true;
        autoTargetRot = Quaternion.LookRotation(-GravityManager.GravityVector,-(m_NavMeshAgent.navMeshOwner as Component).transform.position);
    }

    private void NormalMove(ref Quaternion autoTargetRot)
    {
        IsClimbing = false;
        Vector3 autoTargetDir = (m_NavMeshAgent.steeringTarget - transform.position).normalized;

        switch (GravityManager.gravityDirection)
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
        autoTargetRot = Quaternion.LookRotation(autoTargetDir, -GravityManager.GravityVector);
    }
    #endregion

    public void Dispose()
    {
        m_NavMeshAgent.enabled = false;
        IsBatch = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, castRadius);
        Gizmos.DrawSphere(transform.position + transform.up * castHeight, castRadius);
    }
}
