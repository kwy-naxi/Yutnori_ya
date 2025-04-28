using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }

    [System.Serializable]
    public class PathPoint
    {
        public Vector3 position;
        public List<int> nextPoints;
        public bool isShortcut;

        public PathPoint(Vector3 pos, bool shortcut = false)
        {
            position = pos;
            nextPoints = new List<int>();
            isShortcut = shortcut;
        }
    }

    public List<PathPoint> PathPoints { get; private set; } = new List<PathPoint>();
    public float pointRadius = 0.2f;
    public Transform[] playerStartPositions;
    public Transform goalPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePathPoints();
    }

    private void InitializePathPoints()
    {
        // 기본 경로 포인트 초기화
        float boardSize = 10f;
        float offset = boardSize * 0.3f;

        // 시작점
        PathPoints.Add(new PathPoint(new Vector3(-offset, 0.5f, -offset)));
        
        // 외곽 경로
        for (int i = 0; i < 4; i++)
        {
            float x = -offset + (i * 2 * offset / 3);
            PathPoints.Add(new PathPoint(new Vector3(x, 0.5f, -offset)));
        }

        // 중앙 경로
        PathPoints.Add(new PathPoint(new Vector3(0, 0.5f, 0), true));

        // 도착점
        PathPoints.Add(new PathPoint(new Vector3(0, 0.5f, offset)));

        // 경로 연결
        for (int i = 0; i < PathPoints.Count - 1; i++)
        {
            PathPoints[i].nextPoints.Add(i + 1);
        }

        // 지름길 연결
        PathPoints[2].nextPoints.Add(4); // 외곽에서 중앙으로
        PathPoints[4].nextPoints.Add(5); // 중앙에서 도착점으로
    }

    public Vector3 GetPathPointPosition(int index)
    {
        if (index >= 0 && index < PathPoints.Count)
        {
            return PathPoints[index].position;
        }
        return Vector3.zero;
    }

    public List<int> GetNextPoints(int currentIndex)
    {
        if (currentIndex >= 0 && currentIndex < PathPoints.Count)
        {
            return PathPoints[currentIndex].nextPoints;
        }
        return new List<int>();
    }

    public bool IsGoalPosition(int index)
    {
        // 현재는 마지막 노드를 도착점으로 간주
        return index == PathPoints.Count - 1;
    }

    private void OnDrawGizmos()
    {
        if (PathPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (var point in PathPoints)
        {
            Gizmos.DrawSphere(point.position, pointRadius);
        }

        Gizmos.color = Color.white;
        for (int i = 0; i < PathPoints.Count; i++)
        {
            foreach (var nextIndex in PathPoints[i].nextPoints)
            {
                if (nextIndex < PathPoints.Count)
                {
                    Gizmos.DrawLine(PathPoints[i].position, PathPoints[nextIndex].position);
                }
            }
        }
    }
} 