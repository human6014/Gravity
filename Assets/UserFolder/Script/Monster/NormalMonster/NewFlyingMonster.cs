using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFlyingMonster : MonoBehaviour, IMonster
{
    [SerializeField] Transform target;
    [SerializeField] LayerMask obstacleLayer;
    float speed = 3;
    float additionalSpeed = 0;
    float obstacleDistance = 10;
    Vector3 targetVec;
    private void Update()
    {
        if (additionalSpeed > 0) additionalSpeed -= Time.deltaTime;

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
        //if (targetVec == Vector3.zero) targetVec = egoVector;

        transform.SetPositionAndRotation(transform.position + targetVec * (speed) * Time.deltaTime, Quaternion.LookRotation(targetVec));
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * 20;
        return Vector3.ClampMagnitude(v, 5);
    }

    private Vector3 CalculateObstacleVector()
    {
        Vector3 obstacleVec = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDistance, obstacleLayer))
        {
            obstacleVec = hit.normal;
            additionalSpeed = 5;
        }
        return obstacleVec;
    }
}
