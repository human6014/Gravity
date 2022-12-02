using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Detector;
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
    [SerializeField] private Transform forwardRay;
    [SerializeField] private Transform backRay;
    [SerializeField] private Transform DownRay;
    [SerializeField] private LayerMask forwardLayerMask;
    [SerializeField] private LayerMask backLayerMask;

    private readonly float rotAdjustRatio = 0.1f;
    private readonly float maxSpeed = 10f;
    private readonly float minSpeed = 7f;
    private float targetDistance;
    public bool IsOnMeshLink { get; private set; } = false;
    public bool IsClimbing { get; private set; } = false;
    public bool IsSyncMove { get; set; } = false;
    public Vector3 UpAngle { get; set; }
    public bool GetIsOnOffMeshLink() => navMeshAgent.isOnOffMeshLink;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentLinkMover = GetComponent<AgentLinkMover>();
        legController = FindObjectOfType<LegController>();
        aiController = FindObjectOfType<AIController>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        waitUntil = new WaitUntil(()=> !navMeshAgent.isOnOffMeshLink);
    }

    Vector3 targetDirection;
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
            targetDirection = (navMeshAgent.steeringTarget - transform.position).normalized;

            navRotation = Quaternion.LookRotation(targetDirection, UpAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, navRotation, rotAdjustRatio);

            targetDistance = Vector3.Distance(target.position, transform.position);

            if (navMeshAgent.isOnOffMeshLink && !IsOnMeshLink) StartCoroutine(MeshLinkOffDelay());
            if (targetDistance <= navMeshAgent.stoppingDistance)
            {
                animationController.SetIdle();
                transform.rotation = Quaternion.Lerp(transform.rotation, navRotation, rotAdjustRatio);
            }
            else animationController.SetWalk();
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
        navMeshAgent.speed = minSpeed;
        yield return waitUntil;
        /*
        while (navMeshAgent.isOnOffMeshLink)
        {
            yield return null;
        }
        */
        navMeshAgent.speed = maxSpeed;
        
        IsOnMeshLink = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + targetUpDirection * 15);
        Gizmos.DrawLine(transform.position, transform.position + targetDirection * 5);
        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + targetRightDirection * 15);
    }
}
