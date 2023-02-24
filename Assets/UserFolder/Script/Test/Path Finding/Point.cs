using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public Vector3Int Coords { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public bool InValid { get; private set; }
    public List<Vector3Int> Neighbours { get; private set; }
    public List<MovingData> MovingData { get; private set; }


    public float distanceFactor = 0.5f;

    public Point(Vector3Int coords, Vector3 worldPosition, bool inValid)
    {
        Neighbours = new List<Vector3Int>();
        Coords = coords;
        WorldPosition = worldPosition;
        InValid = inValid;
    }

    public void AddMovingData(AStarAgent obj, float time, bool stationary = false)
    {
        if (MovingData == null) MovingData = new List<MovingData>();

        MovingData existing = MovingData.Find(x => x.MovingObj == obj);
        if (existing == null) MovingData.Add(new MovingData() { MovingObj = obj, TimeToReach = time, TimeStarted = Time.time, Stationary = stationary });
        else
        {
            existing.TimeStarted = Time.time;
            existing.TimeToReach = time;
            existing.Stationary = stationary;
        }
    }

    public void RemoveMovingData(CharacterMoveControl obj) => MovingData.Remove(MovingData.Find(x => x.MovingObj == obj));

    public void CheckForIntersections()
    {
        if (MovingData != null)
        {
            float ttReach;
            float ttReach2;
            float difference;
            List<MovingData> toRemove = new List<MovingData>();
            for (int i = 0; i < MovingData.Count; i++)
            {
                MovingData data = MovingData[i];
                for (int j = 0; j < MovingData.Count; j++)
                {
                    if (i != j)
                    {
                        MovingData data2 = MovingData[j];
                        if (data2.Stationary)
                        {
                            toRemove.Add(data);
                            break;
                        }
                        if (data.MovingObj.Priority < data2.MovingObj.Priority)
                        {
                            ttReach = data.TrueTimeToReach();
                            ttReach2 = data2.TrueTimeToReach();
                            if (ttReach <= 0 || ttReach2 <= 0) continue;

                            difference = Mathf.Abs(ttReach - ttReach2);
                            if (difference < distanceFactor)
                            {
                                toRemove.Add(data);
                                break;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
                MovingData.Remove(toRemove[i]);

            for (int i = 0; i < toRemove.Count; i++)
                toRemove[i].MovingObj.RePath();
        }
    }

    public bool CheckPointAvailability(float timeToReach, int priority)
    {
        bool available = true;
        if (MovingData != null)
        {
            float ttReach;
            float difference;
            List<MovingData> toRemove = new List<MovingData>();
            for (int i = 0; i < MovingData.Count; i++)
            {
                if (MovingData[i].Stationary) return false;
                if (MovingData[i].MovingObj.Priority > priority)
                {
                    ttReach = MovingData[i].TrueTimeToReach();
                    if (ttReach <= 0)
                    {
                        toRemove.Add(MovingData[i]);
                        continue;
                    }
                    difference = Mathf.Abs(ttReach - timeToReach);
                    if (difference < distanceFactor)
                    {
                        available = false;
                        break;
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
                MovingData.Remove(toRemove[i]);
        }
        return available;
    }
}

public class MovingData
{
    public AStarAgent MovingObj;
    public float TimeToReach;
    public float TimeStarted;
    public bool Stationary;

    public float TrueTimeToReach()
    {
        if (TimeToReach == 0) return 0;
        return Mathf.Max(TimeToReach - (Time.time - TimeStarted), 0);
    }
}