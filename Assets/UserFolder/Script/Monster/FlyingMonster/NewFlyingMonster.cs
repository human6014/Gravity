using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
public class NewFlyingMonster : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    private Transform cachedTransform;
    private Transform target;
    private float speed = 3;
    private float additionalSpeed = 0;
    private float obstacleDistance = 10;
    private Vector3 targetVec;

    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
    }
    private void Start()
    {
        target = AIManager.PlayerTransfrom;
    }

    private void Update()
    {
        if (additionalSpeed > 0) additionalSpeed -= Time.deltaTime;

        Vector3 offsetToTarget = AIManager.PlayerTransfrom.position - cachedTransform.position;
        Vector3 targetForwardVec = SteerTowards(offsetToTarget) * 1;
        
        Vector3 obstacleVec = CalculateObstacleVector() * 10;

        targetVec = obstacleVec + targetForwardVec;

        // Steer and Move
        targetVec = Vector3.Lerp(cachedTransform.forward, targetVec, Time.deltaTime).normalized;
        //if (targetVec == Vector3.zero) targetVec = egoVector;

        cachedTransform.SetPositionAndRotation(cachedTransform.position + (speed) * Time.deltaTime * targetVec, Quaternion.LookRotation(targetVec));
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * 20;
        return Vector3.ClampMagnitude(v, 5);
    }

    private Vector3 CalculateObstacleVector()
    {
        Vector3 obstacleVec = Vector3.zero;
        if (Physics.Raycast(cachedTransform.position, cachedTransform.forward, out RaycastHit hit, obstacleDistance, obstacleLayer))
        {
            obstacleVec = hit.normal;
            additionalSpeed = 5;
        }
        return obstacleVec;
    }
}
