using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private int gridWidth = 50;
    [SerializeField] private int gridHeight = 50;
    [SerializeField] private int gridLength = 50;
    [SerializeField] private LayerMask areaMask;
    [SerializeField] private bool IsDrawGizmos;
    public float PointDistance = 3;
    private Vector3 startPoint;

    public int GridWidth { get => gridWidth; private set => gridWidth = value; }
    public int GridHeight { get => gridHeight; private set => gridHeight = value; }
    public int GridLength { get => gridLength; private set => gridLength = value; }
    public static WorldManager Instance { get; private set; }
    public Point[][][] Grid { get; private set; }
    public PointData[][][] GridData { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeGrid();
    }

    private void AddNeighbour(Point p, Vector3Int neighbour)
    {
        if (neighbour.x > -1 && neighbour.x < gridWidth &&
            neighbour.y > -1 && neighbour.y < gridHeight &&
            neighbour.z > -1 && neighbour.z < gridLength)
        {
            p.Neighbours.Add(neighbour);
        }
    }

    private void InitializeGrid()
    {
        startPoint = new Vector3(-gridWidth, -gridHeight, -gridLength) / 2f * PointDistance + transform.position;
        Grid = new Point[gridWidth][][];
        Vector3Int coords;
        Vector3 worldPos;
        bool inValid;
        for (int i = 0; i < gridWidth; i++)
        {
            Grid[i] = new Point[gridHeight][];
            for (int j = 0; j < gridHeight; j++)
            {
                Grid[i][j] = new Point[gridLength];
                for (int k = 0; k < gridLength; k++)
                {
                    coords = new Vector3Int(i, j, k);
                    worldPos = startPoint + new Vector3(i, j, k) * PointDistance;
                    inValid = Physics.CheckBox(worldPos, Vector3.one * PointDistance / 2f, Quaternion.identity, areaMask);
                    Grid[i][j][k] = new Point(coords, worldPos, inValid);

                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            for (int g = -1; g <= 1; g++)
                            {
                                if (i == p && g == q && k == g) continue;
                                AddNeighbour(Grid[i][j][k], new Vector3Int(i + p, j + q, k + g));
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!IsDrawGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWidth, gridHeight, gridLength) * PointDistance);

        if (Grid == null) return;

        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[i].Length; j++)
            {
                for (int k = 0; k < Grid[i][j].Length; k++)
                {
                    if (!Grid[i][j][k].InValid)
                        Gizmos.DrawWireCube(Grid[i][j][k].WorldPosition, Vector3.one * PointDistance / 2f);
                }
            }
        }
    }

    public Point GetClosestPointWorldSpace(Vector3 position)
    {
        float sizeX = PointDistance * gridWidth;
        float sizeY = PointDistance * gridHeight;
        float sizeZ = PointDistance * gridLength;

        Vector3 pos = position - startPoint;
        float percentageX = Mathf.Clamp01(pos.x / sizeX);
        float percentageY = Mathf.Clamp01(pos.y / sizeY);
        float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
        int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * gridWidth), 0, gridWidth - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(percentageY * gridHeight), 0, gridHeight - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * gridLength), 0, gridLength - 1);

        Point result = Grid[x][y][z];
        while (result.InValid)
        {
            int step = 1;
            List<Point> freePoints = new List<Point>();
            for (int p = -step; p <= step; p++)
            {
                for (int q = -step; q <= step; q++)
                {
                    for (int g = -step; g <= step; g++)
                    {
                        if (x == p && y == q && z == g) continue;

                        int i = x + p;
                        int j = y + q;
                        int k = z + g;
                        if (i > -1 && i < gridWidth && j > -1 && j < gridHeight && k > -1 && k < gridLength)
                        {
                            if (!Grid[i][j][k].InValid)
                                freePoints.Add(Grid[i][j][k]);
                        }
                    }
                }
            }
            float distance = Mathf.Infinity;
            for (int i = 0; i < freePoints.Count; i++)
            {
                if ((freePoints[i].WorldPosition - position).sqrMagnitude < distance) result = freePoints[i];
            }
        }
        return result;
    }

    /// <summary>
    /// 랜덤 이동 시 모든 점들을 받아옴
    /// </summary>
    /// <returns>List<Point></returns>
    public List<Point> GetFreePoints()
    {
        List<Point> freePoints = new List<Point>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[i].Length; j++)
            {
                for (int k = 0; k < Grid[i][j].Length; k++)
                {
                    if (!Grid[i][j][k].InValid) freePoints.Add(Grid[i][j][k]);
                }
            }
        }
        return freePoints;
    }
}
