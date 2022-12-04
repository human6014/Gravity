using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Detector;

[RequireComponent(typeof(NavMeshAgent))]
public class NavTrace : MonoBehaviour
{
    private WaitUntil waitUntil;
    private new Rigidbody rigidbody;
    private NavMeshAgent navMeshAgent;
    private AgentLinkMover agentLinkMover;
    private LegController legController;
    private AIController aiController;

    [SerializeField] private FloorDetector floorDetector;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private Transform target;

    private readonly float rotAdjustRatio = 0.5f;
    private readonly float maxSpeed = 15f;
    private readonly float minSpeed = 7f;
    private float currentSpeed = 12.5f;
    private float targetDistance;
    private Vector3 navPosition;
    public bool IsOnMeshLink { get; private set; } = false;
    public bool IsClimbing { get; private set; } = false;
    public Vector3 ProceduralForwardAngle { get; set; }
    public Vector3 ProceduralUpAngle { get; set; }
    public Vector3 ProceduralPosition { get; set; }
    public bool GetIsOnOffMeshLink() => navMeshAgent.isOnOffMeshLink;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentLinkMover = GetComponent<AgentLinkMover>();
        legController = FindObjectOfType<LegController>();
        aiController = FindObjectOfType<AIController>();

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        waitUntil = new WaitUntil(()=> !navMeshAgent.isOnOffMeshLink);
    }

    Vector3 targetDirection;
    Vector3 targetForward;
    Quaternion navRotation;
    private void FixedUpdate()
    {
        if (!legController.GetIsNavOn())
        {
            animationController.SetIdle();
            return;
        }

        if (!navMeshAgent.isActiveAndEnabled) //점프 끝날 때 작동
        {
            navMeshAgent.Warp(aiController.GetPosition());
            transform.rotation = aiController.GetRotation();
        }
        else
        {
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.speed = currentSpeed;

            if (navMeshAgent.isOnOffMeshLink && !IsOnMeshLink) StartCoroutine(MeshLinkOffDelay());

            targetDistance = Vector3.Distance(target.position, transform.position);
            targetDirection = (navMeshAgent.steeringTarget - transform.position).normalized;
            //targetForward = IsOnMeshLink == true ? ProceduralForwardAngle : targetDirection;
            targetForward = ProceduralForwardAngle + targetDirection;

            navRotation = Quaternion.LookRotation(targetForward, ProceduralUpAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, navRotation, rotAdjustRatio);
            if (targetDistance <= navMeshAgent.stoppingDistance)
            {
                animationController.SetIdle();
            }
            else
            {
                animationController.SetWalk();
                navMeshAgent.nextPosition = ProceduralPosition + Time.deltaTime * currentSpeed * targetDirection;
                transform.position = navMeshAgent.nextPosition;
            }

            //IsClimbing = !IsSameFloor();
            //navMeshAgent.speed = IsClimbing == true ? minSpeed : maxSpeed;
        }
        rigidbody.isKinematic = true;
        navMeshAgent.enabled = true;
        agentLinkMover.enabled = true;
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink 타는 중에 점프 하면 버그생김
    private IEnumerator MeshLinkOffDelay()
    {
        IsOnMeshLink = true;
        //navMeshAgent.speed = minSpeed;
        yield return waitUntil;
        /*
        while (navMeshAgent.isOnOffMeshLink)
        {
            yield return null;
        }
        */
        //navMeshAgent.speed = maxSpeed;
        
        IsOnMeshLink = false;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + targetUpDirection * 15);
        Gizmos.DrawLine(transform.position, transform.position + targetDirection * 5);
        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + targetRightDirection * 15);
    }
#endif
}
