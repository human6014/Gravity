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
    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavMeshAgent;
    private LegController m_LegController;
    private SP1AnimationController m_AnimationController;

    private bool m_IsInit;
    private bool m_DoingJumpBiteAttacking;
    
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

    public bool GetIsOnOffMeshLink() => m_NavMeshAgent.isOnOffMeshLink;

    public void SetNavMeshEnable(bool isOn) => m_NavMeshAgent.enabled = isOn;

    public void SetNavMeshPos(Vector3 pos) => m_NavMeshAgent.Warp(pos);
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_AnimationController = GetComponent<SP1AnimationController>();
        m_LegController = FindObjectOfType<LegController>();

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
    /// AI 행동 실시 (Recommended calling from FixedUpdate())
    /// </summary>
    public void OperateAIBehavior()
    {
        if (!m_IsInit) return;
        if (!m_LegController.GetIsNavOn())
        {
            m_AnimationController.SetWalk(true);
            return;
        }

        if (!m_NavMeshAgent.isActiveAndEnabled) //점프 끝날 때 작동
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            return;
        }

        //if (m_NavMeshAgent.pathPending) return;

        SetDestination();

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

        Vector3 targetDirection = (m_NavMeshAgent.steeringTarget - transform.position).normalized;
        //targetForward = IsOnMeshLink == true ? ProceduralForwardAngle : targetDirection;
        Vector3 targetForward = ProceduralForwardAngle + targetDirection;

        Quaternion navRotation = Quaternion.LookRotation(targetForward, ProceduralUpAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, navRotation, m_RotAdjustRatio);
        
        if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        {
            if (!m_DoingJumpBiteAttacking) m_AnimationController.SetWalk(false);
            m_NavMeshAgent.nextPosition = transform.position;
        }
        else
        {
            if (!m_DoingJumpBiteAttacking) m_AnimationController.SetWalk(true);
            m_NavMeshAgent.nextPosition = ProceduralPosition + Time.deltaTime * m_NavMeshAgent.speed * targetDirection;
            transform.position = m_NavMeshAgent.nextPosition;
        }
    }

    private void SetDestination()
    {
        if (AIManager.PlayerRerversePosition != Vector3.zero)
        {
            float reversedDistance = (transform.position - AIManager.PlayerRerversePosition).sqrMagnitude;
            float normalDistance = (transform.position - AIManager.PlayerTransform.position).sqrMagnitude;

            if (reversedDistance < normalDistance)
            {
                m_NavMeshAgent.SetDestination(AIManager.PlayerRerversePosition);
                return;
            }
        }
        m_NavMeshAgent.SetDestination(AIManager.PlayerTransform.position);
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink 타는 중에 점프 하면 버그생김

#if UNITY_EDITOR
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
