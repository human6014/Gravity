using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Detector;
using Manager;
public class NormalMonsterNav : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;

    [SerializeField]
    FloorDetector floorDetector;

    private NavMeshAgent navAgent;

    public Vector3 NewPosition { get; set; } = Vector3.zero;
    /// <summary>
    /// true : navMesh���, false : ����� �̵�
    /// </summary>
    public bool IsFloor { get; set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (GravitiesManager.IsGravityChange)
        {
            IsFloor = false;
            return;
        }
        if (!IsFloor)
        {
            navAgent.Warp(NewPosition);
            if (IsSameFloor()) IsFloor = true;
            return;
        }

        if (IsFloor)
        {
            navAgent.SetDestination(targetTransform.position);

            if (IsSameFloor()) IsClimbing = false;
            else               IsClimbing = true;
        }
    }

    //Debug.Log(ReferenceEquals(navAgent.navMeshOwner, floorDetector.GetNowFloor()));
    //�۵��� �ߵ����� �̸� �񱳴� ���� �񱳹�� �ٲ����
    public bool IsSameFloor() => navAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
}
