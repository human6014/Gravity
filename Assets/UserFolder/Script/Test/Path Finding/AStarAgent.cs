using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AStarAgentStatus
{
    Invalid,
    InProgress,
    Finished,
    RePath
}

public class AStarAgent : MonoBehaviour
{
    private Point startPoint;
    private Point endPoint;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private List<Point> TotalPath = new List<Point>();
    private List<Point> CornerPoints = new List<Point>();
    private PathCreator PathCreator;
    private readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);
    private Coroutine coroutineRePath = null;
    [HideInInspector] public int Priority { get; private set; }
    [HideInInspector] public AStarAgentStatus Status = AStarAgentStatus.Finished;

    [SerializeField] private PathCreator pathCreatorPrefab;
    [SerializeField] private Color debugPathColor;
    [SerializeField] private float movementSpeed = 1.5f;
    [SerializeField] private float turnSpeed = 0.5f;
    [SerializeField] private float cornerSmooth = 1;
    [SerializeField] private bool curvePath = true;
    [SerializeField] private bool debugPath = false;



    #region 지역변수 맴버로 전환용
    List<Point> totalPath = new List<Point>();
    List<PointData> openSet = new List<PointData>();
    List<Vector3> points = new List<Vector3>();
    #endregion

    public PointData[,,] dataSet;

    private void Awake() => AssignPriority();


    private void Start()
    {
        //dataSet = new PointData[WorldManager.Instance.GridWidth, WorldManager.Instance.GridHeight, WorldManager.Instance.GridLength];
        SetStationaryPoint();
    }

    /// <summary>
    /// 다른 AStart 오브젝트와 우선순위 결정
    /// </summary>
    private void AssignPriority()
    {
        AStarAgent[] agents = FindObjectsOfType<AStarAgent>();
        //Sort by speed
        for (int i = 0; i < agents.Length; i++)
        {
            for (int j = i; j < agents.Length; j++)
            {
                if (agents[i].movementSpeed > agents[j].movementSpeed)
                {
                    AStarAgent pom = agents[i];
                    agents[i] = agents[j];
                    agents[j] = pom;
                }
            }
        }

        for (int i = 0; i < agents.Length; i++)
            agents[i].Priority = i;
    }

    private float HeuristicFunction(Vector3 p1, Vector3 p2) => (p2 - p1).sqrMagnitude;

    private void ResetPath()
    {
        for (int i = 0; i < dataSet.GetLength(0); i++)
        {
            for (int j = 0; j < dataSet.GetLength(1); j++)
            {
                for (int k = 0; k < dataSet.GetLength(2); k++)
                {
                    if (dataSet[i, j, k] == null) continue;
                    dataSet[i, j, k].Reset();
                }
            }
        }

    }

    private List<Point> ReconstructPath(PointData start, PointData current, PointData[,,] dataSet)
    {
        CornerPoints.Clear();
        totalPath.Clear();

        PointData currentPointData = dataSet[current.Coords.x, current.Coords.y, current.Coords.z];
        Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

        currentPoint.AddMovingData(this, currentPointData.TimeToReach);
        totalPath.Add(currentPoint);

        Point cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];

        Vector3 direction = ((Vector3)currentPoint.Coords - cameFromPoint.Coords).normalized;
        //direction = direction.normalized;

        CornerPoints.Add(currentPoint);

        int count = 0;
        while (current.CameFrom.x != -1 && count++ < 10000)
        {
            currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];
            cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
            PointData cameFromPointData = dataSet[current.CameFrom.x, current.CameFrom.y, current.CameFrom.z];

            Vector3 dir = (currentPoint.Coords - cameFromPoint.Coords);
            if (dir != direction)
            {
                CornerPoints.Add(currentPoint);
                direction = dir;
            }

            cameFromPoint.AddMovingData(this, cameFromPointData.TimeToReach);
            totalPath.Add(cameFromPoint);
            current = dataSet[current.CameFrom.x, current.CameFrom.y, current.CameFrom.z];
        }

        currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];
        CornerPoints.Add(currentPoint);

        for (int i = 0; i < totalPath.Count; i++)
            totalPath[i].CheckForIntersections();

        return totalPath;
    }

    #region Heap Control
    private void Heapify(List<PointData> list, int i)
    {
        var parent = (i - 1) / 2;
        while (parent >= 0 && list[i].FScore < list[parent].FScore)
        {
            (list[i], list[parent]) = (list[parent], list[i]);
            i = parent; parent = (i - 1) / 2;
        }
    }

    private void HeapifyDeletion(List<PointData> list, int i)
    {
        int smallest;
        int l, r;
        while (true)
        {
            smallest = i;
            l = 2 * i + 1;
            r = 2 * i + 2;
            if (l < list.Count && list[l].FScore < list[smallest].FScore) smallest = l;
            if (r < list.Count && list[r].FScore < list[smallest].FScore) smallest = r;
            if (smallest == i) break;
            (list[i], list[smallest]) = (list[smallest], list[i]);
            i = smallest;
        }
    }
    #endregion

    private float interpolateValue = 4;
    public AStarAgentStatus Pathfinding(Vector3 goal, bool supressMovement = false)
    {
        //startPosition = transform.position;
        goal += (transform.position - goal).normalized * interpolateValue;

        endPosition = goal;
        startPoint = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
        endPoint = WorldManager.Instance.GetClosestPointWorldSpace(goal);
        if (startPoint == endPoint || startPoint.InValid || endPoint.InValid) return Status = AStarAgentStatus.Invalid;

        if (TotalPath != null)
        {
            for (int i = 0; i < TotalPath.Count; i++)
                TotalPath[i].MovingData.Remove(TotalPath[i].MovingData.Find(x => x.MovingObj == this));
        }

        dataSet = WorldManager.Instance.GridData;//new PointData[WorldManager.Instance.GridWidth, WorldManager.Instance.GridHeight, WorldManager.Instance.GridLength];
        PointData _startPoint = new PointData(startPoint);
        dataSet[startPoint.Coords.x, startPoint.Coords.y, startPoint.Coords.z] = _startPoint;
        _startPoint.GScore = 0;
        _startPoint.TimeToReach = 0;

        openSet.Clear();
        openSet.Add(_startPoint);

        while (openSet.Count > 0)
        {
            PointData current = openSet[0];

            if (current.Coords == endPoint.Coords)
            {
                TotalPath = ReconstructPath(_startPoint, current, dataSet);
                if (!supressMovement)
                {
                    Status = AStarAgentStatus.InProgress;
                    StartMoving();
                }
                return Status;
            }
            Debug.Log("Real Finding");
            openSet.RemoveAt(0);
            HeapifyDeletion(openSet, 0);

            Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

            for (int i = 0; i < currentPoint.Neighbours.Count; i++)
            {
                Vector3Int indexes = currentPoint.Neighbours[i];
                Point neighbour = WorldManager.Instance.Grid[indexes.x][indexes.y][indexes.z];
                PointData neighbourData = dataSet[indexes.x, indexes.y, indexes.z];

                bool neighbourPassed = true;
                if (neighbourData == null)
                {
                    neighbourData = new PointData(neighbour);
                    dataSet[indexes.x, indexes.y, indexes.z] = neighbourData;
                    neighbourPassed = false;
                }

                float distance = (currentPoint.WorldPosition - neighbour.WorldPosition).magnitude;
                float timeToReach = current.TimeToReach + distance / movementSpeed;
                bool neighbourAvailable = neighbour.CheckPointAvailability(timeToReach, Priority);

                if (neighbour == endPoint && !neighbourAvailable) return Status = AStarAgentStatus.Invalid;
                if (!neighbour.InValid && neighbourAvailable)
                {
                    float tenativeScore = current.GScore + WorldManager.Instance.PointDistance;
                    if (tenativeScore < neighbourData.GScore)
                    {
                        neighbourData.CameFrom = current.Coords;
                        neighbourData.GScore = tenativeScore;
                        neighbourData.FScore = neighbourData.GScore + HeuristicFunction(neighbour.WorldPosition, endPoint.WorldPosition);
                        neighbourData.TimeToReach = timeToReach;
                        if (!neighbourPassed)
                        {
                            openSet.Add(neighbourData);
                            Heapify(openSet, openSet.Count - 1);
                        }
                    }
                }
            }
        }
        return Status = AStarAgentStatus.Invalid;
    }

    public void RePath()
    {
        if (Status != AStarAgentStatus.RePath)
        {
            if (coroutineRePath != null) StopCoroutine(coroutineRePath);
            coroutineRePath = StartCoroutine(Coroutine_RePath());
        }
    }

    private IEnumerator Coroutine_RePath()
    {
        Status = AStarAgentStatus.RePath;

        Point p = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
        p.AddMovingData(this, 0, true);

        while (Status == AStarAgentStatus.RePath)
        {
            Status = Pathfinding(endPosition);
            if (Status == AStarAgentStatus.Invalid)
            {
                Status = AStarAgentStatus.RePath;
                yield return waitForSeconds;
            }
        }
    }



    private Coroutine coroutineCharacterFollowPath = null;
    private void StartMoving()
    {
        //StopAllCoroutines();
        if (coroutineCharacterFollowPath != null) StopCoroutine(coroutineCharacterFollowPath);
        coroutineCharacterFollowPath = StartCoroutine(Coroutine_CharacterFollowPath());
        //StartCoroutine(Coroutine_CharacterFollowPathCurve());
    }

    public void StopMovement()
    {
        if (coroutineCharacterFollowPath != null) StopCoroutine(coroutineCharacterFollowPath);
        Status = AStarAgentStatus.Finished;
    }

    /// <summary>
    /// 실질적인 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator Coroutine_CharacterFollowPath()
    {
        Status = AStarAgentStatus.InProgress;
        for (int i = TotalPath.Count - 1; i >= 0; i--)
        {
            SetPathColor();
            float length = (transform.position - TotalPath[i].WorldPosition).magnitude;
            float l = 0;
            while (l < length)
            {
                SetPathColor();
                Vector3 forwardDirection = (TotalPath[i].WorldPosition - transform.position).normalized;
                if (curvePath)
                {
                    transform.position += movementSpeed * Time.deltaTime * transform.forward;
                    transform.forward = Vector3.Lerp(transform.forward, forwardDirection, Time.deltaTime * turnSpeed);
                }
                else
                {
                    transform.forward = forwardDirection;
                    transform.position = Vector3.MoveTowards(transform.position, TotalPath[i].WorldPosition, Time.deltaTime * movementSpeed);
                }
                l += Time.deltaTime * movementSpeed;
                yield return waitForFixedUpdate;
            }
        }
        SetStationaryPoint();
        Status = AStarAgentStatus.Finished;
    }


    #region movement1
    private IEnumerator Coroutine_CharacterFollowPathCurve()
    {
        Status = AStarAgentStatus.InProgress;
        CreateBezierPath();

        float length = PathCreator.path.length;
        float l = 0;

        while (l < length)
        {
            SetPathColor();
            transform.position += movementSpeed * Time.deltaTime * transform.forward;
            Vector3 forwardDirection = (PathCreator.path.GetPointAtDistance(l, EndOfPathInstruction.Stop) - transform.position).normalized;
            transform.forward = Vector3.Slerp(transform.forward, forwardDirection, Time.deltaTime * turnSpeed);
            l += Time.deltaTime * movementSpeed;
            yield return waitForFixedUpdate;
        }

        SetStationaryPoint();
        Status = AStarAgentStatus.Finished;
    }

    /// <summary>
    /// 버지어 곡선 생성
    /// </summary>
    public void CreateBezierPath()
    {
        if (PathCreator == null) PathCreator = Instantiate(pathCreatorPrefab, Vector3.zero, Quaternion.identity);

        points.Clear();

        points.Add(CornerPoints[CornerPoints.Count - 1].WorldPosition);
        for (int i = CornerPoints.Count - 2; i >= 0; i--)
        {
            //Vector3 centerPos = CornerPoints[i + 1].WorldPosition + (CornerPoints[i].WorldPosition - CornerPoints[i + 1].WorldPosition) / 2f;
            points.Add(CornerPoints[i].WorldPosition);
        }
        points.Add(CornerPoints[0].WorldPosition);


        BezierPath bezierPath = new BezierPath(points, false, PathSpace.xyz)
        {
            ControlPointMode = BezierPath.ControlMode.Free
        };
        int cornerIndex = CornerPoints.Count - 1;


        bezierPath.SetPoint(1, CornerPoints[cornerIndex].WorldPosition, true);
        for (int i = 2; i < bezierPath.NumPoints - 2; i += 3)
        {
            Vector3 position = bezierPath.GetPoint(i + 1) + (CornerPoints[cornerIndex].WorldPosition - bezierPath.GetPoint(i + 1)) * cornerSmooth;
            bezierPath.SetPoint(i, position, true);
            if (cornerIndex > 0)
            {
                position = bezierPath.GetPoint(i + 2) + (CornerPoints[cornerIndex - 1].WorldPosition - bezierPath.GetPoint(i + 2)) * cornerSmooth;
                bezierPath.SetPoint(i + 2, position, true);
            }
            cornerIndex--;
        }
        bezierPath.SetPoint(bezierPath.NumPoints - 2, CornerPoints[0].WorldPosition, true);


        bezierPath.NotifyPathModified();
        PathCreator.bezierPath = bezierPath;
    }
    #endregion

    private void SetStationaryPoint()
    {
        Point p = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
        p.AddMovingData(this, 0, true);
        p.CheckForIntersections();
    }

    public void SetPathColor()
    {
        if (debugPath)
        {
            if (TotalPath != null)
            {
                for (int j = TotalPath.Count - 2; j >= 0; j--)
                {
                    Debug.DrawLine(TotalPath[j + 1].WorldPosition, TotalPath[j].WorldPosition, debugPathColor, 1);
                }
            }
        }
    }
}
