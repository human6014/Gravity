using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager;

[RequireComponent(typeof(NavMeshAgent))]
public class NormalMonsterNav : MonoBehaviour
{
    [SerializeField] private LayerMask climbingDetectLayer;
    NavMeshAgent navMeshAgent;
    Transform cachedTransform;
    Rigidbody cachedRigidbody;

    private Quaternion climbingLookRot = Quaternion.identity;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 targetPosition;
    private Vector3 autoTargetDir;
    private Vector3 manualTargetDir;

    private float currentSpeed;
    private float remainingDistance;

    public bool IsFolling { get; private set; } = false;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        cachedTransform = GetComponent<Transform>();
        cachedRigidbody = GetComponent<Rigidbody>();

        currentSpeed = navMeshAgent.speed;

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    NavMeshPath path;
    private void Update()
    {
        if (GravitiesManager.IsGravityChange)
        {
            cachedRigidbody.useGravity = true;
            cachedRigidbody.isKinematic = false;
            navMeshAgent.enabled = false;
            IsFolling = true;
        }
        if (IsFolling)
        {
            DetectCol();
            return;
        }

        path = new NavMeshPath();
        navMeshAgent.CalculatePath(AIManager.PlayerTransfrom.position, path);

        if (path.status == NavMeshPathStatus.PathPartial) ManualMode();
        else if(path.status == NavMeshPathStatus.PathInvalid)
        {
            //
        }
        else AutoMode();
    }

    private void DetectCol()
    {
        Collider[] col = Physics.OverlapSphere(cachedTransform.position, 2, climbingDetectLayer);
        if (col.Length == 0) return;
        if (Physics.Raycast(cachedTransform.position, col[0].transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
        {
            if (hit.normal == -GravitiesManager.GravityVector)
            {
                Debug.Log("≈ª√‚");
                IsFolling = false;
                navMeshAgent.enabled = true;
                cachedRigidbody.useGravity = false;
                cachedRigidbody.isKinematic = true;
            }
        }
    }

    private void AutoMode()
    {
        Debug.Log("AutoMode");

        IsAutoMode = true;
        navMeshAgent.isStopped = false;

        navMeshAgent.destination = (AIManager.PlayerTransfrom.position);
        targetPosition = navMeshAgent.steeringTarget;
        
        autoTargetDir = (targetPosition - cachedTransform.position).normalized;
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

        if (IsClimbing) autoTargetRot = climbingLookRot;
        else autoTargetRot = Quaternion.LookRotation(autoTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, autoTargetRot, 0.2f);
    }

    private void ManualMode()
    {
        Debug.Log("ManualMode");

        IsAutoMode = false;
        navMeshAgent.isStopped = true;

        manualTargetDir = (AIManager.CurrentTargetDirection(cachedTransform) - cachedTransform.position).normalized;
        manualTargetRot = Quaternion.LookRotation(manualTargetDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, manualTargetRot, 0.2f);

        remainingDistance = Vector3.Distance(cachedTransform.position, AIManager.PlayerTransfrom.position);

        if (remainingDistance < navMeshAgent.stoppingDistance) return;
        transform.position += Time.deltaTime * currentSpeed * manualTargetDir;
    }

    private void OnTriggerStay(Collider other)
    {
        IsClimbing = false;
        if (!navMeshAgent.isActiveAndEnabled || !IsAutoMode) return;

        IsClimbing = true;
        if (Physics.Raycast(cachedTransform.position, other.transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
            climbingLookRot = Quaternion.LookRotation(-hit.normal, -GravitiesManager.GravityVector);
    }
}
