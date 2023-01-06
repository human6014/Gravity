using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager;

[RequireComponent(typeof(NavMeshAgent))]
public class LastNormal : MonoBehaviour
{
    [SerializeField] private LayerMask climbingDetectLayer;
    NavMeshAgent navMeshAgent;
    NavMeshPath path;
    Transform cachedTransform;
    Rigidbody cachedRigidbody;

    private Quaternion climbingLookRot = Quaternion.identity;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 autoTargetDir;
    private Vector3 manualTargetDir;

    private float currentSpeed;

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
    }

   
    private void Update()
    {
        if (GravitiesManager.IsGravityChange)
        {
            cachedRigidbody.useGravity = true;
            cachedRigidbody.isKinematic = false;
            navMeshAgent.enabled = false;
            IsFolling = true;
            return;
        }
        if (IsFolling)
        {
            DetectCol();
            return;
        }

        path = new NavMeshPath();
        navMeshAgent.CalculatePath(AIManager.PlayerTransfrom.position, path);

        if (path.status == NavMeshPathStatus.PathPartial) ManualMode();
        else if (path.status == NavMeshPathStatus.PathInvalid)
        {

        }
        else AutoMode();
    }

    private void DetectCol()
    {
        Collider[] col = Physics.OverlapSphere(cachedTransform.position, 1.5f, climbingDetectLayer);
        if (col.Length == 0) return;
        for (int i = 0; i < col.Length; i++)
        {
            if (Physics.Raycast(cachedTransform.position, col[i].transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
            {
                if (hit.normal == -GravitiesManager.GravityVector)
                {
                    Debug.Log("Å»Ãâ");
                    IsFolling = false;
                    navMeshAgent.enabled = true;
                    cachedRigidbody.useGravity = false;
                    cachedRigidbody.isKinematic = true;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position,1.5f);
    }

    private void AutoMode()
    {
        Debug.Log("AutoMode");

        IsAutoMode = true;
        navMeshAgent.isStopped = false;

        navMeshAgent.destination = AIManager.PlayerTransfrom.position;

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

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) return;
        Debug.Log("Move");
        transform.position += Time.deltaTime * currentSpeed * manualTargetDir;
    }

    private void OnTriggerStay(Collider other)
    {
        IsClimbing = false;
        if (!navMeshAgent.isActiveAndEnabled || !IsAutoMode) return;
        if (!AIManager.IsSameFloor(navMeshAgent))
        {
            if(Physics.Raycast(cachedTransform.position, other.transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
            {
                IsClimbing = true;
                climbingLookRot = Quaternion.LookRotation(-hit.normal, -GravitiesManager.GravityVector);
            }
        }
    }
}
