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
    private WaitUntil waitUntil;
    private new Rigidbody rigidbody;
    private Transform cachedTransform;
    private NavMeshAgent navMeshAgent;
    private LegController legController;
    private AnimationController animationController;

    private bool isInit;
    #region Adjustment factor
    [Tooltip("회전 강도")]
    private readonly float rotAdjustRatio = 0.3f;

    [Tooltip("최대 이동 속도")]
    private readonly float maxSpeed = 15f;

    [Tooltip("최소 이동 속도")]
    private readonly float minSpeed = 5f;
    #endregion

    #region Property
    public bool IsOnMeshLink { get; private set; } = false;

    public bool IsClimbing { get; private set; } = false;

    public Vector3 ProceduralForwardAngle { get; set; }

    public Vector3 ProceduralUpAngle { get; set; }

    public Vector3 ProceduralPosition { get; set; }
    #endregion

    public bool GetIsOnOffMeshLink() => navMeshAgent.isOnOffMeshLink;

    public void SetNavMeshEnable(bool isOn) => navMeshAgent.enabled = isOn;

    public void SetNavMeshPos(Vector3 pos) => navMeshAgent.Warp(pos);
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        cachedTransform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        legController = FindObjectOfType<LegController>();

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        waitUntil = new WaitUntil(() => !navMeshAgent.isOnOffMeshLink);
    }

    public void Init(Quaternion roatation)
    {
        isInit = true;
        transform.rotation = roatation;
        navMeshAgent.enabled = true;
        Debug.Log("rotation : " + transform.rotation.eulerAngles);
    }

    /// <summary>
    /// AI 행동 실시 (Recommended calling from FixedUpdate())
    /// </summary>
    public void OperateAIBehavior()
    {
        if (!isInit) return;
        if (!legController.GetIsNavOn())
        {
            animationController.SetIdle();
            return;
        }

        if (!navMeshAgent.isActiveAndEnabled) //점프 끝날 때 작동
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            return;
        }

        //if (navMeshAgent.pathPending == true) return;

        SetDestination();

        if (navMeshAgent.isOnOffMeshLink)
        {
            //navMeshAgent.speed = minSpeed;
            //NavMeshLink link = (NavMeshLink)navMeshAgent.navMeshOwner;
            //navMeshAgent.updateUpAxis = false;
            //여기서 NavMeshLink 감지 가능
        }
        else
        {
            //navMeshAgent.speed = normalSpeed;
            //navMeshAgent.updateUpAxis = true;
        }
        Vector3 targetDirection = (navMeshAgent.steeringTarget - cachedTransform.position).normalized;
        //targetForward = IsOnMeshLink == true ? ProceduralForwardAngle : targetDirection;
        Vector3 targetForward = ProceduralForwardAngle + targetDirection;

        Quaternion navRotation = Quaternion.LookRotation(targetForward, ProceduralUpAngle);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, navRotation, rotAdjustRatio);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.nextPosition = transform.position;
            animationController.SetIdle();
        }
        else
        {
            animationController.SetWalk();
            navMeshAgent.nextPosition = ProceduralPosition + Time.deltaTime * navMeshAgent.speed * targetDirection;
            cachedTransform.position = navMeshAgent.nextPosition;
        }

    }

    private void SetDestination()
    {
        if (AIManager.PlayerRerversePosition != Vector3.zero)
        {
            float reversedDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerRerversePosition);
            float normalDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerTransform.position);

            if (reversedDistance < normalDistance)
            {
                navMeshAgent.SetDestination(AIManager.PlayerRerversePosition);
                return;
            }
        }
        navMeshAgent.SetDestination(AIManager.PlayerTransform.position);
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
