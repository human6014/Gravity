using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager;
using Manager.AI;
using Contoller.Floor;

[RequireComponent(typeof(NavMeshAgent))]
public class NavTrace : MonoBehaviour
{
    private WaitUntil waitUntil;
    private new Rigidbody rigidbody;
    private Transform cachedTransform;
    private NavMeshAgent navMeshAgent;
    private AgentLinkMover agentLinkMover;
    private LegController legController;
    private SpecialMonster1 specialMonster1;

    [SerializeField] private AnimationController animationController;

    #region Adjustment factor
    [Header("Adjustment factor")]

    [SerializeField]
    [Tooltip("회전 강도")]
    private readonly float rotAdjustRatio = 0.3f;

    [SerializeField]
    [Tooltip("최대 이동 속도")]
    private readonly float maxSpeed = 15f;

    [SerializeField]
    [Tooltip("최소 이동 속도")]
    private readonly float minSpeed = 7f;

    private float currentSpeed = 10;

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
        agentLinkMover = GetComponent<AgentLinkMover>();
        legController = FindObjectOfType<LegController>();
        specialMonster1 = FindObjectOfType<SpecialMonster1>();

        currentSpeed = navMeshAgent.speed;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        waitUntil = new WaitUntil(() => !navMeshAgent.isOnOffMeshLink);
    }

    private void FixedUpdate()
    {
        if (!legController.GetIsNavOn())
        {
            animationController.SetIdle();
            return;
        }

        if (!navMeshAgent.isActiveAndEnabled) //점프 끝날 때 작동
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            cachedTransform.rotation = specialMonster1.GetRotation();
        }
        else
        {
            if (navMeshAgent.pathPending == true) return;

            if(AIManager.PlayerRerversePosition != Vector3.zero)
                navMeshAgent.SetDestination(AIManager.PlayerRerversePosition);
            else
                navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);

            if (navMeshAgent.isOnOffMeshLink)
            {
                navMeshAgent.updateUpAxis = false;
                NavMeshLink link = (NavMeshLink)navMeshAgent.navMeshOwner;
                Debug.Log(link.name);
                //여기서 NavMeshLink 감지 가능
            }
            else
            {
                navMeshAgent.updateUpAxis = true;
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
                navMeshAgent.nextPosition = ProceduralPosition + Time.deltaTime * currentSpeed * targetDirection;
                cachedTransform.position = navMeshAgent.nextPosition;
            }

            //IsClimbing = !IsSameFloor();
        }
        //점프 등 패턴따라 사용될 수도 있음
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
