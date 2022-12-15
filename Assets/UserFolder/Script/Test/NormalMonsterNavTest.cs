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

    private NavMeshAgent navMeshAgent;

    public Vector3 NewPosition { get; set; } = Vector3.zero;
    public bool IsAutoMode { get; private set; } = true;
    public bool IsClimbing { get; private set; } = false;
    public bool HasPath { get; private set; } = true;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private bool startFlag = false;
    
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private bool isSameFloor;
    private void FixedUpdate()
    {
        //Debug.Log("isAutoMode : " + IsAutoMode);
        //Debug.Log(navMeshAgent.hasPath);
        //Debug.Log("navMeshAgent.pathStatus : " + navMeshAgent.pathStatus);
        //Debug.Log("navMeshAgent.pathPending : " + navMeshAgent.pathPending);
        //Debug.Log("navMeshAgent.navMeshOwner.name : " + navMeshAgent.navMeshOwner.name);
        if (GravitiesManager.IsGravityChange)
        {
            IsAutoMode = false;
        }

        //navMeshAgent.updatePosition = IsAutoMode;
        isSameFloor = IsSameFloor();

        if (IsAutoMode) AutoUpdate();
        else            ManualUpdate();
    }

    private void AutoUpdate()
    {
        navMeshAgent.SetDestination(targetTransform.position);

        Debug.Log(navMeshAgent.hasPath);

        Debug.Log(navMeshAgent.CalculatePath(targetTransform.position, navMeshAgent.path));

        if(navMeshAgent.velocity == Vector3.zero && !navMeshAgent.hasPath && startFlag)
        {
            //navMeshAgent.isStopped = true;
            //IsAutoMode = false;
            //return;
        }
        startFlag = true;
        //hasPath로 감지해서 IsAutoMode = false 해주어야 함
        if (isSameFloor) IsClimbing = false;
        else IsClimbing = true;
    }

    private void ManualUpdate()
    {
        navMeshAgent.Warp(NewPosition);
        
        Debug.Log("isSameFloor : " + isSameFloor);
        if (isSameFloor /*&& 뭔가 더... */)
        {
            //navMeshAgent.isStopped = false;
            //IsAutoMode = true;
        }
    }
    //Debug.Log(ReferenceEquals(navAgent.navMeshOwner, floorDetector.GetNowFloor()));
    //작동은 잘되지만 이름 비교는 느림 비교방식 바꿔야함
    public bool IsSameFloor()
    {
        if (!navMeshAgent.isOnNavMesh) return false;
        return navMeshAgent.navMeshOwner.name == floorDetector.GetNowFloor().name;
    }
}
