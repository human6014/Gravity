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
    private NavMeshAgent navMeshAgent;
    private AgentLinkMover agentLinkMover;
    private LegController legController;
    private SpecialMonster1 aiController;

    [SerializeField] private AnimationController animationController;

    #region Adjustment factor
    [Header("Adjustment factor")]

    [SerializeField] [Tooltip("ȸ�� ����")] 
    private readonly float rotAdjustRatio = 0.5f;

    [SerializeField] [Tooltip("�ִ� �̵� �ӵ�")]
    private readonly float maxSpeed = 15f;

    [SerializeField] [Tooltip("�ּ� �̵� �ӵ�")] 
    private readonly float minSpeed = 7f;

    private float currentSpeed = 12.5f;
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
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentLinkMover = GetComponent<AgentLinkMover>();
        legController = FindObjectOfType<LegController>();
        aiController = FindObjectOfType<SpecialMonster1>();

        currentSpeed = navMeshAgent.speed;

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

        if (!navMeshAgent.isActiveAndEnabled) //���� ���� �� �۵�
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            transform.rotation = aiController.GetRotation();
        }
        else
        {
            navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);
            navMeshAgent.speed = currentSpeed;

            if (navMeshAgent.isOnOffMeshLink && !IsOnMeshLink) StartCoroutine(MeshLinkOffDelay());

            targetDirection = (navMeshAgent.steeringTarget - transform.position).normalized;
            //targetForward = IsOnMeshLink == true ? ProceduralForwardAngle : targetDirection;
            targetForward = ProceduralForwardAngle + targetDirection;

            navRotation = Quaternion.LookRotation(targetForward, ProceduralUpAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, navRotation, rotAdjustRatio);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
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
        //���� �� ���ϵ��� ���� ���� ����
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink Ÿ�� �߿� ���� �ϸ� ���׻���
    private IEnumerator MeshLinkOffDelay()
    {
        IsOnMeshLink = true;
        //currentSpeed = minSpeed;
        yield return waitUntil;
        /*
        while (navMeshAgent.isOnOffMeshLink)
        {
            yield return null;
        }
        */
        //currentSpeed = maxSpeed;

        IsOnMeshLink = false;
    }

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
