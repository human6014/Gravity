using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Detector;
using Manager;
public class NormalMonsterNavTest : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private FloorDetector floorDetector;

    private NavMeshAgent navAgent;

    public Vector3 NewPosition { get; set; } = Vector3.zero;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public bool HasPath { get; private set; } = true;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private bool isSameFloor;
    private void FixedUpdate()
    {
        Debug.Log("isAutoMode : " + IsAutoMode);
        
        //Debug.Log("navAgent.pathStatus : " + navAgent.pathStatus);
        //Debug.Log("navAgent.pathPending : " + navAgent.pathPending);
        //Debug.Log("navAgent.navMeshOwner.name : " + navAgent.navMeshOwner.name);
        if (GravitiesManager.IsGravityChange)
        {
            IsAutoMode = false;
            return;
        }

        isSameFloor = IsSameFloor();
        if (IsAutoMode)
        {
            navAgent.updatePosition = true;
            navAgent.SetDestination(targetTransform.position);

            //hasPath로 감지해서 IsAutoMode = false 해주어야 함
            if (isSameFloor) IsClimbing = false;
            else             IsClimbing = true;
        }
        else
        {
            navAgent.Warp(NewPosition);
            if (isSameFloor)
            {
                IsAutoMode = true;
            }
        }
    }

    //Debug.Log(ReferenceEquals(navAgent.navMeshOwner, floorDetector.GetNowFloor()));
    //작동은 잘되지만 이름 비교는 느림 비교방식 바꿔야함
    public bool IsSameFloor() => navAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
}
