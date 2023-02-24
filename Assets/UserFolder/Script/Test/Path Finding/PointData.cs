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
