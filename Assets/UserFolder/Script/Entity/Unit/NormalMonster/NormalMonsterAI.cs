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

    private WaitForSeconds waitForSeconds = new(0.2f);
    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private Transform cachedTransform;
    private Rigidbody cachedRigidbody;

    private Quaternion climbingLookRot;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 autoTargetDir;
    private Vector3 manualTargetDir;

    private const float maximumFallingTime = 10;
    private float stopDistance;
    private float currentSpeed;
    private float fallingTimer;
    private bool detectCol;
    [SerializeField] private float castHeight = 1.9f;
    [SerializeField] private float castRadius = 0.5f;

    public bool IsBatch { get; private set; } = false;
    public bool IsFolling { get; private set; } = false;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public bool IsMalfunction { get; private set; } = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        cachedTransform = GetComponent<Transform>();
        cachedRigidbody = GetComponent<Rigidbody>();

        stopDistance = navMeshAgent.stoppingDistance;
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
        cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.forward, -GravityManager.GravityVector);
    }

    public void Move()
    {
        if (!IsBatch) return;

        if (GravityManager.IsGravityChanging)
        {
            SetMode(false);
            return;
        }
        if (IsFolling)
        {
            DetectCol();
            DetectMalfunction();
            return;
        }

        if (IsAutoMode)
        {
            AutoMode();
            //path.ClearCorners();
            //navMeshAgent.CalculatePath(AIManager.PlayerTransfrom.position, path);
        }

        /*
        if (path.status == NavMeshPathStatus.PathPartial) { }// ManualMode();
        else if (path.status == NavMeshPathStatus.PathInvalid)
        {

        }
        else AutoMode();
        */
    }

    private void DetectCol()
    {
        //건물 오르는 중에 중력이 바뀌면 회전하면서 떨어짐
        //따라서 위쪽방향 직선이 아니라 Collider가 필요
        //레이어도 관리 필요함 (울타리 같은 애들)

        // 대부분 정상 작동
        if (Physics.SphereCast(new Ray(transform.position, transform.up), castRadius, castHeight, climbingDetectLayer))
            SetMode(true);
    }

    private void SetMode(bool isAutoMode)
    {
        IsFolling = !isAutoMode;
        cachedRigidbody.useGravity = !isAutoMode;
        cachedRigidbody.isKinematic = isAutoMode;
        navMeshAgent.enabled = isAutoMode;
        IsAutoMode = isAutoMode;
    }

    private void DetectMalfunction()
    {
        fallingTimer += Time.deltaTime;
        if (fallingTimer >= maximumFallingTime) IsMalfunction = true;
    }

    private void AutoMode()
    {
        //Debug.Log("AutoMode");

        fallingTimer = 0;
        navMeshAgent.SetDestination(AIManager.PlayerTransform.position);

        if (!navMeshAgent.isOnOffMeshLink && !AIManager.IsSameFloor(navMeshAgent))
        {
            IsClimbing = true;
            climbingLookRot = Quaternion.LookRotation((navMeshAgent.navMeshOwner as Component).transform.position, -GravityManager.GravityVector);
        }
        else IsClimbing = false;

        if (IsClimbing) autoTargetRot = climbingLookRot;
        else
        {
            autoTargetDir = (navMeshAgent.steeringTarget - cachedTransform.position).normalized;

            switch (GravityManager.gravityDirection)
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
            if (autoTargetDir == Vector3.zero)
                autoTargetRot = Quaternion.LookRotation(AIManager.PlayerTransform.position, -GravityManager.GravityVector);
            else
                autoTargetRot = Quaternion.LookRotation(autoTargetDir, -GravityManager.GravityVector);
        }
        //cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, autoTargetRot, 0.2f);
        //ㄴ 이동중에 뒤집힐듯 말듯 하는 현상 원인
        cachedTransform.rotation = autoTargetRot;
    }

    public void Dispose()
    {
        IsBatch = false;
        navMeshAgent.enabled = false;
    }
    /*
    private Coroutine navDetect = null;
    private void ManualMode()
    {
        
        Debug.Log("ManualMode");
        if (navDetect == null) navDetect = StartCoroutine(DetectNavMeshOn());
        
        IsAutoMode = false;

        //벽뚫고 옴
        manualTargetDir = (AIManager.CurrentTargetPosition(cachedTransform) - cachedTransform.position).normalized;
        manualTargetRot = Quaternion.LookRotation(manualTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, manualTargetRot, 0.2f);

        if (Vector3.Distance(cachedTransform.position, AIManager.CurrentTargetPosition(cachedTransform)) < stopDistance) return;
        cachedTransform.position += Time.deltaTime * currentSpeed * manualTargetDir;
    }

    
    private IEnumerator DetectNavMeshOn()
    {
        //성능 부하 엄청남
        //의도대로 안움직임
        //-> 위에서 연결되지 않은 땅으로 떨어지는 경우
        //중간중간에 계속 navMeshAgent를 켜서 위치를 강제 이동시켜서 그런 것 같음
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
                    IsAutoMode = true;
                    navDetect = null;
                    yield break;
                }
            }
            navMeshAgent.enabled = false;
            yield return null;
        }
    }
    */

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, castRadius);
    //    Gizmos.DrawSphere(transform.position + transform.up * castHeight, castRadius);
    //}
}
