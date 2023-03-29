using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingMovementController : MonoBehaviour
{
    [Tooltip("타겟과 직선 상 감지 레이어")]
    [SerializeField] private LayerMask playerSeeLayerMask = -1;

    [Tooltip("")]
    [SerializeField] private float maxDistanceRebuildPath = 1;

    [Tooltip("가속도")]
    [SerializeField] private float acceleration = 1;

    [Tooltip("이동 노드 최소 도달 거리")]
    [SerializeField] private float minReachDistance = 2f;

    [Tooltip("최종 노드 도달 거리")]
    [SerializeField] private float minFollowDistance = 4f;

    [Tooltip("")]
    [SerializeField] private float pathPointRadius = 0.2f;

    private Octree octree;
    private Octree.PathRequest oldPath;
    private Octree.PathRequest newPath;

    private Transform target;
    private GameObject playerObject;
    private Rigidbody cachedRigidbody;
    private SphereCollider sphereCollider;

    private Vector3 currentDestination;
    private Vector3 lastDestination;
    private Vector3 randomPos;

    private bool isRun;
    private bool CanSeePlayer()
    {
        if (Physics.Raycast(transform.position, transform.position - target.position, out RaycastHit hit, Vector3.Distance(transform.position, target.position) + 1, playerSeeLayerMask))
            return hit.transform.gameObject == playerObject;
        return false;
    }

    private Octree.PathRequest Path
    {
        get
        {
            if ((newPath == null || newPath.isCalculating) && oldPath != null) return oldPath;
            return newPath;
        }
    }

    public bool CloseToTarget
    {
        get => Path != null && Path.Path.Count > 3;
    }

    public bool HasTarget
    {
        get => Path != null && Path.Path.Count > 0;
    }

    public Vector3 CurrentTargetPosition
    {
        get
        {
            if (Path != null && Path.Path.Count > 0) return currentDestination;
            return target.position;
        }
    }

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        cachedRigidbody = GetComponent<Rigidbody>();
        octree = FindObjectOfType<Octree>();

        playerObject = Manager.AI.AIManager.PlayerTransfrom.gameObject;
        target = Manager.AI.AIManager.PlayerSupportTargetTransform;
    }

    public void Init()
    {
        randomPos = UnityEngine.Random.insideUnitSphere * 3;

        lastDestination = target.position;
        oldPath = newPath;
        newPath = octree.GetPath(transform.position, lastDestination + randomPos);

        isRun = true;
    }

    public void MoveCurrentTarget()
    {
        if (!isRun) return;
        if ((newPath == null || !newPath.isCalculating) && Vector3.SqrMagnitude(target.position - lastDestination) > maxDistanceRebuildPath &&
            (!CanSeePlayer() || Vector3.Distance(target.position, transform.position) > minFollowDistance) && !octree.IsBuilding)
        {
            lastDestination = target.position;

            oldPath = newPath;
            newPath = octree.GetPath(transform.position, lastDestination + randomPos);
        }

        var curPath = Path;

        if (!curPath.isCalculating && curPath != null && curPath.Path.Count > 0)
        {
            if (Vector3.Distance(transform.position, target.position) < minFollowDistance && CanSeePlayer())
                curPath.Reset();

            currentDestination = curPath.Path[0] + Vector3.ClampMagnitude(cachedRigidbody.position - curPath.Path[0], pathPointRadius);

            cachedRigidbody.velocity += acceleration * Time.deltaTime * Vector3.ClampMagnitude(currentDestination - transform.position, 1);
            float sqrMinReachDistance = minReachDistance * minReachDistance;

            Vector3 predictedPosition = cachedRigidbody.position + cachedRigidbody.velocity * Time.deltaTime;
            float shortestPathDistance = Vector3.SqrMagnitude(predictedPosition - currentDestination);
            int shortestPathPoint = 0;

            float sqrDistance;
            float sqrPredictedDistance;
            for (int i = 0; i < curPath.Path.Count; i++)
            {
                sqrDistance = Vector3.SqrMagnitude(cachedRigidbody.position - curPath.Path[i]);
                if (sqrDistance <= sqrMinReachDistance)
                {
                    if (i < curPath.Path.Count) curPath.Path.RemoveRange(0, i + 1);

                    shortestPathPoint = 0;
                    break;
                }

                sqrPredictedDistance = Vector3.SqrMagnitude(predictedPosition - curPath.Path[i]);
                if (sqrPredictedDistance < shortestPathDistance)
                {
                    shortestPathDistance = sqrPredictedDistance;
                    shortestPathPoint = i;
                }
            }

            if (shortestPathPoint > 0) curPath.Path.RemoveRange(0, shortestPathPoint);
        }
        else cachedRigidbody.velocity -= acceleration * Time.deltaTime * cachedRigidbody.velocity;

    }

    public void Dispose()
    {
        isRun = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (cachedRigidbody != null)
        {
            Gizmos.color = Color.blue;
            Vector3 predictedPosition = cachedRigidbody.position + cachedRigidbody.velocity * Time.deltaTime;
            Gizmos.DrawWireSphere(predictedPosition, sphereCollider.radius);
        }

        if (Path != null)
        {
            var path = Path;
            for (int i = 0; i < path.Path.Count - 1; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(path.Path[i], minReachDistance);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(path.Path[i], Vector3.ClampMagnitude(cachedRigidbody.position - path.Path[i], pathPointRadius));
                Gizmos.DrawWireSphere(path.Path[i], pathPointRadius);
                Gizmos.DrawLine(path.path[i], path.Path[i + 1]);
            }
        }
    }
}
