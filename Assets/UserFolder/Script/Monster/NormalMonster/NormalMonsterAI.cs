using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager.AI;
using Manager;

[RequireComponent(typeof(NavMeshAgent))]
public class NormalMonsterAI : MonoBehaviour
{
    [SerializeField] private LayerMask climbingDetectLayer;

    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private Transform cachedTransform;
    private Rigidbody cachedRigidbody;

    private Quaternion climbingLookRot;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 autoTargetDir;
    private Vector3 manualTargetDir;

    private float currentSpeed;
    private float updateTimer;
    private bool detectCol;
    [SerializeField] private float RayDistance = 2f;

    public bool IsBatch { get; private set; } = false;
    public bool IsFolling { get; private set; } = false;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public bool HasPath { get; private set; } = true;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        cachedTransform = GetComponent<Transform>();
        cachedRigidbody = GetComponent<Rigidbody>();

        currentSpeed = navMeshAgent.speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        path = new NavMeshPath();
    }

    public void Init(Vector3 pos)
    {
        IsBatch = true;
        navMeshAgent.enabled = true;
        navMeshAgent.Warp(pos);
        cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.forward, -GravitiesManager.GravityVector);
    }

    public void Move()
    {
        if (!IsBatch) return;

        if (GravitiesManager.IsGravityChange)
        {
            cachedRigidbody.useGravity = true;
            cachedRigidbody.isKinematic = false;
            navMeshAgent.enabled = false;
            //DetectCol() 연결
            IsFolling = true;
            return;
        }
        if (IsFolling)
        {
            detectCol = true;
            //DetectCol();
            return;
        }


        if (!navMeshAgent.isActiveAndEnabled)
        {
            Debug.LogError("isActiveAndEnabled == false");
            return;
        }

        path.ClearCorners();
        navMeshAgent.CalculatePath(AIManager.PlayerTransfrom.position, path);

        if (path.status == NavMeshPathStatus.PathPartial) ManualMode();
        else if (path.status == NavMeshPathStatus.PathInvalid)
        {

        }
        else AutoMode();
        Debug.Log(path.status);
    }

    /*
    private void DetectCol()
    {
        //건물 오르는 중에 중력이 바뀌면 회전하면서 떨어짐
        //따라서 위쪽방향 직선이 아니라 Collider가 필요
        //레이어도 관리 필요함 (울타리 같은 애들)
        if (Physics.Raycast(cachedTransform.position, cachedTransform.up, RayDistance, climbingDetectLayer))
        {
            IsFolling = false;
            cachedRigidbody.useGravity = false;
            cachedRigidbody.isKinematic = true;
            navMeshAgent.enabled = true;
        }
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        if (!detectCol) return;
        
        Debug.Log("CollisionEnter");
        IsFolling = false;
        detectCol = false;
        cachedRigidbody.useGravity = false;
        cachedRigidbody.isKinematic = true;
        navMeshAgent.enabled = true;
    }

    private void AutoMode()
    {
        //Debug.Log("AutoMode");

        IsAutoMode = true;
        navMeshAgent.isStopped = false;

        updateTimer += Time.deltaTime;
        if (updateTimer >= 0.15f)
        {
            updateTimer = 0;
            //navMeshAgent.SetPath(path);
            navMeshAgent.SetDestination(AIManager.PlayerTransfrom.position);
        }

        if (!navMeshAgent.isOnOffMeshLink)
        {
            if (!AIManager.IsSameFloor(navMeshAgent))
            {
                IsClimbing = true;
                climbingLookRot = Quaternion.LookRotation((navMeshAgent.navMeshOwner as Component).transform.position, -GravitiesManager.GravityVector);
            }
        }
        else IsClimbing = false;

        if (IsClimbing) autoTargetRot = climbingLookRot;
        else
        {
            autoTargetDir = (navMeshAgent.steeringTarget - cachedTransform.position).normalized;
            switch (GravitiesManager.gravityDirection)
            {
                case EnumType.GravityDirection.X:
                    autoTargetDir.x = 0;
                    break;
                case EnumType.GravityDirection.Y:
                    autoTargetDir.y = 0;
                    break;
                case EnumType.GravityDirection.Z:
                    autoTargetDir.z = 0;
                    break;
            }
            autoTargetRot = Quaternion.LookRotation(autoTargetDir, -GravitiesManager.GravityVector);
        }
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, autoTargetRot, 0.2f);
    }

    private Coroutine navDetect = null;
    private void ManualMode()
    {
        //Debug.Log("ManualMode");
        if (navDetect == null) navDetect = StartCoroutine(DetectNavMeshOn());

        IsAutoMode = false;
        navMeshAgent.isStopped = true;

        manualTargetDir = (AIManager.CurrentTargetPosition(cachedTransform) - cachedTransform.position).normalized;
        manualTargetRot = Quaternion.LookRotation(manualTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, manualTargetRot, 0.2f);

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) return;
        cachedTransform.position += Time.deltaTime * currentSpeed * manualTargetDir;
    }

    private IEnumerator DetectNavMeshOn()
    {
        while (true)
        {
            navMeshAgent.enabled = true;
            if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                path.ClearCorners();
                navMeshAgent.CalculatePath(AIManager.PlayerTransfrom.position, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    cachedRigidbody.useGravity = false;
                    cachedRigidbody.isKinematic = true;
                    navDetect = null;
                    yield break;
                }
            }
            navMeshAgent.enabled = false;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
        Gizmos.DrawRay(transform.position, transform.up * RayDistance);
    }
}
