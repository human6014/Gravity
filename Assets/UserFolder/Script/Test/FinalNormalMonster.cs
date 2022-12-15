using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager;

public class FinalNormalMonster : MonoBehaviour
{
    Transform cachedTransform;
    NavMeshAgent navMeshAgent;
    new Rigidbody rigidbody;

    [SerializeField] private FinalNormalMonsterSupport navSupporter;
    [SerializeField] private LayerMask climbingDetectLayer;

    public bool IsNoneMode { get; private set; } = false;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public bool HasPath { get; private set; } = true;
    public bool IsSameFloor { get; private set; } = false;

    private Quaternion climbingLookRot = Quaternion.identity;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 targetPosition;
    private Vector3 autoTargetDir;
    private Vector3 manualTargetDir;

    private float currentSpeed;
    private float remainingDistance;

    /*
     * 
     * 
     * 
     * !!!!! 코드 정리 필요함 !!!!!
     * 
     * 
     * 
     * 
     * !!!!! 중력 변경 시 위치 이상함 !!!!!
     * 
     * 
     * 
     * 
     * !!!!! 중력 변경 시 mode 전환 제대로 안됨 !!!!!
     * 
     * 
     * 
     */
    private void Start()
    {
        cachedTransform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();

        currentSpeed = navMeshAgent.speed;

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void FixedUpdate()
    {
        if (GravitiesManager.IsGravityChange)
        {
            navMeshAgent.isStopped = true;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            IsAutoMode = false;
            IsNoneMode = true;
            navMeshAgent.updatePosition = false;
            navSupporter.OnNoneMode(true);
        }

        IsSameFloor = AIManager.IsSameFloor(navMeshAgent);

        if (IsNoneMode)
        {
            CheckMode();
            return;
        }

        if (navMeshAgent.isPathStale)
        {
            IsAutoMode = false;
        }

        navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);
        HasPath = !navMeshAgent.isPathStale;
        //위 두줄 중요함
        //IsSameFloor = AIManager.IsSameFloor(navMeshAgent);

        navMeshAgent.isStopped = !IsAutoMode;
        navMeshAgent.updatePosition = IsAutoMode;
        if (IsAutoMode) AutoMode();
        else            ManualMode();
        navSupporter.SyncNav(transform.position);
    }

    private void CheckMode()
    {
        navMeshAgent.transform.position = navSupporter.GetPos();
        navMeshAgent.Warp(navSupporter.GetPos());
        if (IsSameFloor)
        {
            navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);
            HasPath = !navMeshAgent.isPathStale;
            navMeshAgent.updatePosition = true;
            IsAutoMode = HasPath;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            IsNoneMode = false;
            navSupporter.OnNoneMode(false);
        }
    }

    private void AutoMode()
    {
        Debug.Log("AutoMode");

        targetPosition = navMeshAgent.steeringTarget;

        autoTargetDir = (targetPosition - cachedTransform.position).normalized;
        autoTargetDir.y = 0;
        IsClimbing = !IsSameFloor;

        if (IsClimbing) autoTargetRot = climbingLookRot;
        else            autoTargetRot = Quaternion.LookRotation(autoTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, autoTargetRot, 0.2f);
    }

    private void ManualMode()
    {
        Debug.Log("ManualMode");

        if(IsSameFloor && HasPath)
        {
            IsAutoMode = true;
            return;
        }
        manualTargetDir = (AIManager.CurrentTargetDirection(cachedTransform) - cachedTransform.position).normalized;
        manualTargetRot = Quaternion.LookRotation(manualTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, manualTargetRot, 0.2f);

        remainingDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerTransfrom.position);

        if (remainingDistance < navMeshAgent.stoppingDistance) return;
        navMeshAgent.nextPosition += Time.deltaTime * currentSpeed * manualTargetDir;
        cachedTransform.position = navMeshAgent.nextPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsClimbing) return;

        if (Physics.Raycast(cachedTransform.position, other.transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
            climbingLookRot = Quaternion.LookRotation(-hit.normal, -GravitiesManager.GravityVector);
    }
}
