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
            navAgent.enabled = false;
            return;
        }
        if (!IsFloor) return;

        if (navAgent.isActiveAndEnabled)
        {
            navAgent.SetDestination(targetTransform.position);

            if (IsSameFloor()) IsClimbing = false;
            else IsClimbing = true;
        }
        else
        {
            navAgent.enabled = true;
            navAgent.Warp(NewPosition);
        }
    }

    //Debug.Log(ReferenceEquals(navAgent.navMeshOwner, floorDetector.GetNowFloor()));
    //작동은 잘되지만 이름 비교는 느림 비교방식 바꿔야함
    public bool IsSameFloor() => navAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
}
