using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    private PathCreator m_PathCreator;
    private float distanceTravelled;

    public void Init(PathCreator pathCreator)
    {
        m_PathCreator = pathCreator;
    }

    public void FollowPath(float movementSpeed)
    {
        distanceTravelled += movementSpeed * Time.deltaTime;

        transform.SetPositionAndRotation(
            m_PathCreator.path.GetPointAtDistance(distanceTravelled), 
            m_PathCreator.path.GetRotationAtDistance(distanceTravelled));
    }
}
