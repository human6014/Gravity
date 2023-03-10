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
    

    #region Adjustment factor
    [Header("Adjustment factor")]

    [SerializeField]
    [Tooltip("ȸ�� ����")]
    private readonly float rotAdjustRatio = 0.3f;

    [SerializeField]
    [Tooltip("�ִ� �̵� �ӵ�")]
    private readonly float maxSpeed = 15f;

    [SerializeField]
    [Tooltip("�ּ� �̵� �ӵ�")]
    private readonly float minSpeed = 5f;

    private float normalSpeed;
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

        normalSpeed = navMeshAgent.speed;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        waitUntil = new WaitUntil(() => !navMeshAgent.isOnOffMeshLink);
    }

    /// <summary>
    /// AI �ൿ �ǽ� (Recommended calling from FixedUpdate())
    /// </summary>
    /// <param name="rot"></param>
    public void OperateAIBehavior(Quaternion rot)
    {
        if (!legController.GetIsNavOn())
        {
            animationController.SetIdle();
            return;
        }

        if (!navMeshAgent.isActiveAndEnabled) //���� ���� �� �۵�
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            cachedTransform.rotation = rot;
        }
        else
        {
            //if (navMeshAgent.pathPending == true) return;

            SetDestination();

            
            if (navMeshAgent.isOnOffMeshLink)
            {
                //navMeshAgent.speed = minSpeed;
                //NavMeshLink link = (NavMeshLink)navMeshAgent.navMeshOwner;
                //navMeshAgent.updateUpAxis = false;
                //���⼭ NavMeshLink ���� ����
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
        //���� �� ���ϵ��� ���� ���� ����
    }
    private void FixedUpdate()
    {
        /*
        if (!legController.GetIsNavOn())
        {
            animationController.SetIdle();
            return;
        }

        if (!navMeshAgent.isActiveAndEnabled) //���� ���� �� �۵�
        {
            //navMeshAgent.Warp(aiController.GetPosition());
            cachedTransform.rotation = specialMonster1.GetRotation();
        }
        else
        {
            if (navMeshAgent.pathPending == true) return;

            SetDestination();

            if (navMeshAgent.isOnOffMeshLink)
            {
                NavMeshLink link = (NavMeshLink)navMeshAgent.navMeshOwner;
                Debug.Log(link.name);
                navMeshAgent.updateUpAxis = false;
                //���⼭ NavMeshLink ���� ����
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
        }
        //���� �� ���ϵ��� ���� ���� ����
        */
    }

    private void SetDestination()
    {
        if (AIManager.PlayerRerversePosition != Vector3.zero)
        {
            float reversedDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerRerversePosition);
            float normalDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerTransfrom.position);

            if (reversedDistance < normalDistance)
            {
                navMeshAgent.SetDestination(AIManager.PlayerRerversePosition);
                return;
            }
        }
        navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);
    }

    //public bool IsSameFloor() => navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    //MeshLink Ÿ�� �߿� ���� �ϸ� ���׻���

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
