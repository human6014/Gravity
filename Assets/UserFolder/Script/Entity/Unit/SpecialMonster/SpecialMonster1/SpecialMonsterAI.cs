using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager;
using Manager.AI;
using Contoller.Floor;

[RequireComponent(typeof(NavMeshAgent))]
public class SpecialMonsterAI : MonoBehaviour
{
    private WaitUntil m_WaitUntil;
    private NavMeshAgent m_NavMeshAgent;

    private bool m_IsInit;

    #region Adjustment factor
    [Tooltip("회전 강도")]
    private readonly float m_RotAdjustRatio = 0.3f;

    [Tooltip("기본 이동 속도")]
    private float m_OriginalSpeed = 9;

    [Tooltip("최대 이동 속도")]
    private readonly float m_MaxSpeed = 10f;

    [Tooltip("최소 이동 속도")]
    private readonly float m_MinSpeed = 3f;
    #endregion

    #region Property
    public bool IsOnMeshLink { get; private set; } = false;
    public bool IsClimbing { get; private set; } = false;
    public Vector3 ProceduralForwardAngle { get; set; }
    public Vector3 ProceduralUpAngle { get; set; }
    public Vector3 ProceduralPosition { get; set; }
    #endregion

    public bool GetIsOnOffMeshLink { get => m_NavMeshAgent.isOnOffMeshLink; }
    public bool SetNavMeshEnable { set => m_NavMeshAgent.enabled = value; }
    public void SetNavMeshPos(Vector3 pos) => m_NavMeshAgent.Warp(pos);
    
    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_NavMeshAgent.updatePosition = false;
        m_NavMeshAgent.updateRotation = false;
        m_NavMeshAgent.updateUpAxis = false;

        m_WaitUntil = new WaitUntil(() => !m_NavMeshAgent.isOnOffMeshLink);
    }

    public void Init(Quaternion roatation)
    {
        m_IsInit = true;
        transform.rotation = roatation;
        m_NavMeshAgent.enabled = true;
        m_OriginalSpeed = m_NavMeshAgent.speed;
    }

    public void Dispose()
    {
        m_NavMeshAgent.enabled = false;
    }

    /// <summary>
    /// AI 행동 실시
    /// </summary>
    public bool OperateAIBehavior(ref bool changeFlag)
    {
        bool isWalk = false;
        if (!m_IsInit) return isWalk;

        m_NavMeshAgent.isStopped = false;
        SetDestination(out float remainingDistance);

        if (m_NavMeshAgent.isOnOffMeshLink)
        {
            m_NavMeshAgent.speed = m_MinSpeed;
            //NavMeshLink link = (NavMeshLink)navMeshAgent.navMeshOwner;
            //navMeshAgent.updateUpAxis = false;
            //여기서 NavMeshLink 감지 가능
        }
        else
        {
            m_NavMeshAgent.speed = m_OriginalSpeed;
            //navMeshAgent.updateUpAxis = true;
        }
        bool isCloseToTarget = remainingDistance <= m_NavMeshAgent.stoppingDistance;

        Vector3 targetVec = isCloseToTarget ? AIManager.PlayerGroundPosition : m_NavMeshAgent.steeringTarget;
        Vector3 targetDirection = (targetVec - transform.position).normalized;
        //targetForward = IsOnMeshLink == true ? ProceduralForwardAngle : targetDirection;
        Vector3 targetForward = (ProceduralForwardAngle + targetDirection).normalized;
        //위쪽으로 기움

        Quaternion navRotation = Quaternion.LookRotation(targetForward, ProceduralUpAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, navRotation, m_RotAdjustRatio);

        if (isCloseToTarget)
        {
            m_NavMeshAgent.isStopped = true;
            Debug.Log("Closer");
            
            changeFlag = true;
            isWalk = false;
            m_NavMeshAgent.destination = transform.position;
        }
        else
        {
            changeFlag = true;
            isWalk = true;
            m_NavMeshAgent.nextPosition = ProceduralPosition + Time.deltaTime * m_NavMeshAgent.speed * targetDirection;
            transform.position = m_NavMeshAgent.nextPosition;
        }
        return isWalk;
    }

    private void SetDestination(out float remainingDistance)
    {
        Vector3 finalDestination = AIManager.PlayerGroundPosition;
        remainingDistance = (transform.position - AIManager.PlayerGroundPosition).sqrMagnitude;
        if (AIManager.PlayerRerversePosition != Vector3.zero)
        {
            float reversedDistance = (transform.position - AIManager.PlayerRerversePosition).sqrMagnitude;

            if (reversedDistance < remainingDistance)
            {
                finalDestination = AIManager.PlayerRerversePosition;
                remainingDistance = reversedDistance;
            }
        }
        remainingDistance = Mathf.Sqrt(remainingDistance);
        m_NavMeshAgent.SetDestination(finalDestination);
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink 타는 중에 점프 하면 버그생김

#if UNITY_EDITOR

    public void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, targetForward);
    }

    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + targetUpDirection * 15);
        //Gizmos.DrawLine(transform.position, transform.position + targetDirection * 5);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + targetRightDirection * 15);
    }
#endif
}
