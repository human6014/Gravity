using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class NewFlyingMonster : MonoBehaviour, IMonster
{
    [SerializeField] private LayerMask obstacleLayer;

    private Transform target;

    private float speed = 3;
    private float additionalSpeed = 0;
    private float obstacleDistance = 10;
    Vector3 targetVec;
    private void Update()
    {
        target = AIManager.PlayerTransfrom;
        //if (additionalSpeed > 0) additionalSpeed -= Time.deltaTime;

        Vector3 targetForwardVec = Vector3.zero;
        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - transform.position);
            targetForwardVec = SteerTowards(offsetToTarget) * 1;
        }
        Vector3 obstacleVec = CalculateObstacleVector() * 10;

        targetVec = obstacleVec + targetForwardVec;

        // Steer and Move
        targetVec = Vector3.Lerp(transform.forward, targetVec, Time.deltaTime).normalized;

        transform.SetPositionAndRotation(transform.position + speed * Time.deltaTime * targetVec, Quaternion.LookRotation(targetVec));
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * 20;
        return Vector3.ClampMagnitude(v, 5);
    }

    private Vector3 CalculateObstacleVector()
    {
        Vector3 obstacleVec = Vector3.zero;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, obstacleDistance, obstacleLayer))
        {
            obstacleVec = hit.normal;
            //additionalSpeed = 5;
        }
        return obstacleVec;
    }
}
