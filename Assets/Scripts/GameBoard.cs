using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }

    [System.Serializable]
    public class PathNode
    {
        public Vector3 position;
        public List<int> nextNodes = new List<int>();
        public bool isShortcut;
    }

    public List<PathNode> pathNodes = new List<PathNode>();
    public Transform[] playerStartPositions;
    public Transform goalPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePathNodes();
    }

    private void InitializePathNodes()
    {
        float boardSize = 10f; // BoardGenerator의 boardSize와 동일하게 설정
        float offset = boardSize * 0.4f;

        // 경로 노드 생성
        AddPathNode(new Vector3(-offset, 0.2f, -offset), new[] { 1 });  // 0: 시작점
        AddPathNode(new Vector3(0, 0.2f, -offset), new[] { 2 });        // 1
        AddPathNode(new Vector3(offset, 0.2f, -offset), new[] { 3, 5 }); // 2
        AddPathNode(new Vector3(offset, 0.2f, 0), new[] { 4 });         // 3
        AddPathNode(new Vector3(offset, 0.2f, offset), new[] { 7 });    // 4
        AddPathNode(new Vector3(0, 0.2f, 0), new[] { 6 }, true);        // 5: 지름길
        AddPathNode(new Vector3(0, 0.2f, offset), new[] { 7 });         // 6
        AddPathNode(new Vector3(-offset, 0.2f, offset), new[] { 8 });   // 7
        AddPathNode(new Vector3(-offset, 0.2f, 0), new[] { 0 });        // 8
    }

    private void AddPathNode(Vector3 pos, int[] nextIndices, bool isShortcut = false)
    {
        PathNode node = new PathNode
        {
            position = pos,
            isShortcut = isShortcut
        };
        node.nextNodes.AddRange(nextIndices);
        pathNodes.Add(node);
    }

    public Vector3 GetPathPosition(int index)
    {
        if (index >= 0 && index < pathNodes.Count)
        {
            return pathNodes[index].position;
        }
        return Vector3.zero;
    }

    public int GetNextPathIndex(int currentIndex)
    {
        if (currentIndex >= 0 && currentIndex < pathNodes.Count)
        {
            var node = pathNodes[currentIndex];
            if (node.nextNodes.Count > 0)
            {
                // 현재는 첫 번째 다음 노드만 반환
                // 나중에 플레이어가 경로를 선택할 수 있도록 수정 가능
                return node.nextNodes[0];
            }
        }
        return -1;
    }

    public bool IsGoalPosition(int index)
    {
        // 현재는 마지막 노드를 도착점으로 간주
        return index == pathNodes.Count - 1;
    }

    private void OnDrawGizmos()
    {
        // 경로 시각화
        if (pathNodes != null)
        {
            foreach (var node in pathNodes)
            {
                Gizmos.color = node.isShortcut ? Color.red : Color.yellow;
                Gizmos.DrawSphere(node.position, 0.2f);

                foreach (int nextIndex in node.nextNodes)
                {
                    if (nextIndex < pathNodes.Count)
                    {
                        Gizmos.DrawLine(node.position, pathNodes[nextIndex].position);
                    }
                }
            }
        }
    }
} 