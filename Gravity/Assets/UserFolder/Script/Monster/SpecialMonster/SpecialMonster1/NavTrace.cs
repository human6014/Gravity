using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Detector;
public class NavTrace : MonoBehaviour
{
    WaitForSeconds waitSeconds;
    WaitUntil waitUntil;

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
    [SerializeField] private LayerMask forwardLayerMask;
    [SerializeField] private LayerMask backLayerMask;

    private readonly float maxSpeed = 10f;
    private readonly float minSpeed = 5f;
    private float distance;
    public bool IsOnMeshLink { get; private set; } = false;
    public bool IsClimbing { get; private set; } = false;
    public bool IsSyncMove { get; set; } = false;
    public bool GetIsOnOffMeshLink() => navMeshAgent.isOnOffMeshLink;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentLinkMover = GetComponent<AgentLinkMover>();
        legController = FindObjectOfType<LegController>();
        aiController = FindObjectOfType<AIController>();

        //waitUntil = new WaitUntil(() => !navMeshAgent.isOnOffMeshLink);
        //waitSeconds = new(0.4f);
    }

    RaycastHit hit;
    Vector3 targetDirection;
    private void FixedUpdate()
    {
        if (!target) return;
        if (legController.GetIsNavOn())
        {
            if (!navMeshAgent.isActiveAndEnabled) //점프 끝날 때 작동
            {
                navMeshAgent.Warp(aiController.GetPosition());
                transform.rotation = aiController.GetRotation();
            }
            else
            {
                navMeshAgent.SetDestination(target.position);
                targetDirection = (target.position - transform.position).normalized;
                aiController.AIMove();
                distance = Vector3.Distance(target.position, transform.position);
                DetectClimbing();

                if (navMeshAgent.isOnOffMeshLink)
                {
                    if (!IsOnMeshLink) StartCoroutine(MeshLinkOffDelay());
                }
                else
                {
                    if (distance <= navMeshAgent.stoppingDistance)
                    {
                        animationController.SetIdle();
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection),0.1f);
                    }
                    else if(navMeshAgent.velocity == Vector3.zero)
                    {
                        animationController.SetIdle();
                    }
                    else animationController.SetWalk();
                }
                //IsClimbing = !IsSameFloor();
                navMeshAgent.speed = IsClimbing == true ? minSpeed : maxSpeed;
            }
            rigidbody.isKinematic = true;
            navMeshAgent.enabled = true;
            agentLinkMover.enabled = true;
        }
        else animationController.SetIdle();
    }
    NavMeshHit hit1;
    private void DetectClimbing()
    {
        if (Physics.Raycast(forwardRay.position, forwardRay.forward, out hit, 5, forwardLayerMask))
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }
        else if (Physics.Raycast(backRay.position, backRay.forward, out hit, 8, backLayerMask) &&
                 Physics.OverlapSphere(backRay.position, 1, forwardLayerMask).Length == 0 &&
                 navMeshAgent.SamplePathPosition(backLayerMask, 3, out hit1))
                 //모퉁이쪽 갈 경우 2번째 식에서 true가 되어버림 따라서 bake할 때 움직일 땅을 줄여야 할듯
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }
        else
        {
            navMeshAgent.updateRotation = true;
            navMeshAgent.updateUpAxis = true;
        }
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink 타는 중에 점프 하면 버그생김
    private IEnumerator MeshLinkOffDelay()
    {
        IsOnMeshLink = true;
        navMeshAgent.speed = minSpeed;

        while (navMeshAgent.isOnOffMeshLink)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, aiController.GetRotation(),0.5f);
            yield return null;
        }

        transform.rotation = Quaternion.LookRotation(targetDirection, target.up);
        
        navMeshAgent.speed = maxSpeed;
        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;
        
        IsOnMeshLink = false;
    }
}
