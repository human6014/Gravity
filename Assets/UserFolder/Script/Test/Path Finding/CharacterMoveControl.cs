using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointData
{
    public float GScore;
    public float FScore;
    public Vector3Int CameFrom;
    public Vector3Int Coords;
    public float TimeToReach;

    public PointData(Point point)
    {
        GScore = Mathf.Infinity;
        FScore = Mathf.Infinity;
        CameFrom = new Vector3Int(-1, -1, -1);
        Coords = point.Coords;
    }

    public void Reset()
    {
        GScore = Mathf.Infinity;
        FScore = Mathf.Infinity;
        CameFrom = new Vector3Int(-1, -1, -1);
        Coords = Vector3Int.zero;
        TimeToReach = 0;
    }
}

[RequireComponent(typeof(AStarAgent))]
public class CharacterMoveControl : MonoBehaviour
{
    private float timer = 0f;
    private bool isArrive = false;
    private AStarAgent _Agent;
    [SerializeField] private Transform moveToPoint;
    Vector3 oldPos;
    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
        StartCoroutine(Coroutine_MoveToTarget());
    }

    private void Update()
    {
        /*
        timer += Time.deltaTime;
        if (timer>=0.3f)
        {
            if (moveToPoint.position == oldPos) return;
            timer = 0;
            AStarAgentStatus status = _Agent.Pathfinding(moveToPoint.position);
            oldPos = moveToPoint.position;
            Debug.Log(status);
        }
        */
    }

    IEnumerator Coroutine_MoveToTarget()
    {
        while (true)
        {
            _Agent.Pathfinding(moveToPoint.position);
            while (_Agent.Status == AStarAgentStatus.Invalid)
            {
                isArrive = true;
                yield return new WaitForSeconds(0.5f);
                _Agent.Pathfinding(moveToPoint.position);
            }
            isArrive = false;
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                //이동중일때
                yield return null;
            }
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (isArrive)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveToPoint.position, -Manager.GravitiesManager.GravityVector), 0.1f);
        }
    }
}
