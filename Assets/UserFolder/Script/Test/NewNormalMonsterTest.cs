using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using Manager;
using Detector;

public class NewNormalMonsterTest : PoolableScript, IMonster
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private FloorDetector floorDetector;
    [SerializeField] private NormalMonsterNavTest normalMonsterNav;
    [SerializeField] private LayerMask climbingDetectLayer;

    private Transform cachedTransform;
    private new Rigidbody rigidbody;

    private Quaternion climbingLookRot = Quaternion.identity;
    private Quaternion autoTargetRot;
    private Quaternion manualTargetRot;
    private Vector3 targetPosition;
    private Vector3 manualTargeDir;

    private float speed = 2;
    private float stoppingDistance = 2;
    private float currentDistance;

    private bool isAutoMode;
    void Start()
    {
        cachedTransform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        isAutoMode = normalMonsterNav.IsAutoMode;

        rigidbody.isKinematic = isAutoMode;
        rigidbody.useGravity = !isAutoMode;

        if (isAutoMode) AutoUpdate();
        else            ManualUpdate();
    }

    private void AutoUpdate()
    {
        if (normalMonsterNav.IsClimbing) autoTargetRot = climbingLookRot;
        else                             autoTargetRot = normalMonsterNav.Rotation;

        cachedTransform.SetPositionAndRotation(normalMonsterNav.Position, autoTargetRot);
    }

    private void ManualUpdate()
    {
        targetPosition = targetTransform.position;

        manualTargeDir = (CurrentTargetDirection() - cachedTransform.position).normalized;
        manualTargetRot = Quaternion.LookRotation(manualTargeDir, -GravitiesManager.GravityVector);
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, manualTargetRot, 0.3f);

        currentDistance = Vector3.Distance(cachedTransform.position, targetPosition);
        if (currentDistance < stoppingDistance) return;
        cachedTransform.position += Time.deltaTime * speed * cachedTransform.forward;
        normalMonsterNav.NewPosition = cachedTransform.position;
    }

    private Vector3 CurrentTargetDirection()
    {
        switch (GravitiesManager.gravityDirection)
        {
            case GravityDirection.X: return new(cachedTransform.position.x, targetPosition.y, targetPosition.z);
            case GravityDirection.Y: return new(targetPosition.x, cachedTransform.position.y, targetPosition.z);
            case GravityDirection.Z: return new(targetPosition.x, targetPosition.y, cachedTransform.position.z);
            default: break;
        }
        return Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!normalMonsterNav.IsClimbing) return;

        if (Physics.Raycast(cachedTransform.position, other.transform.position - cachedTransform.position, out RaycastHit hit, climbingDetectLayer))
            climbingLookRot = Quaternion.LookRotation(-hit.normal, GravitiesManager.GravityVector);
    }

    public override void ReturnObject()
    {
        throw new System.NotImplementedException();
    }
}
