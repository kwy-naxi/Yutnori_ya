using UnityEngine;
using System.Collections.Generic;

public class BoardGenerator : MonoBehaviour
{
    public Material boardMaterial;
    public Material pathMaterial;
    public float boardSize = 10f;
    public float boardHeight = 0.1f;
    public float pathWidth = 0.5f;
    public float pathHeight = 0.2f;

    private List<GameObject> pathSegments = new List<GameObject>();

    private void Awake()
    {
        Debug.Log("보드 생성 시작");
        GenerateBoard();
        GeneratePath();
    }

    private void GenerateBoard()
    {
        Debug.Log("메인 보드 생성 중...");
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Board";
        board.transform.parent = transform;
        board.transform.localScale = new Vector3(boardSize, boardHeight, boardSize);
        board.transform.position = new Vector3(0, 0, 0);

        if (boardMaterial != null)
        {
            board.GetComponent<MeshRenderer>().material = boardMaterial;
            Debug.Log("보드 머티리얼 적용됨");
        }
        else
        {
            Debug.LogWarning("보드 머티리얼이 설정되지 않았습니다!");
        }
    }

    private void GeneratePath()
    {
        Debug.Log("경로 생성 시작");
        float offset = boardSize * 0.4f; // 경로의 위치를 보드 크기의 40%로 설정

        // 외곽 경로 생성
        CreatePathSegment(new Vector3(-offset, boardHeight, -offset), new Vector3(offset, boardHeight, -offset));
        CreatePathSegment(new Vector3(offset, boardHeight, -offset), new Vector3(offset, boardHeight, offset));
        CreatePathSegment(new Vector3(offset, boardHeight, offset), new Vector3(-offset, boardHeight, offset));
        CreatePathSegment(new Vector3(-offset, boardHeight, offset), new Vector3(-offset, boardHeight, -offset));

        // 대각선 경로 생성
        CreatePathSegment(new Vector3(-offset, boardHeight, -offset), new Vector3(offset, boardHeight, offset));
        CreatePathSegment(new Vector3(offset, boardHeight, -offset), new Vector3(-offset, boardHeight, offset));

        Debug.Log($"경로 생성 완료. 총 {pathSegments.Count}개의 경로 세그먼트 생성됨");
    }

    private void CreatePathSegment(Vector3 start, Vector3 end)
    {
        Debug.Log($"경로 세그먼트 생성: {start} -> {end}");

        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = "PathSegment";
        segment.transform.parent = transform;

        // 경로의 길이와 방향 계산
        Vector3 direction = end - start;
        float length = direction.magnitude;
        Vector3 center = (start + end) / 2f;

        // 경로 세그먼트의 크기와 회전 설정
        segment.transform.position = center;
        segment.transform.localScale = new Vector3(pathWidth, pathHeight, length);
        segment.transform.rotation = Quaternion.LookRotation(direction);

        // 머티리얼 할당
        if (pathMaterial != null)
        {
            segment.GetComponent<MeshRenderer>().material = pathMaterial;
            Debug.Log("경로 머티리얼 적용됨");
        }
        else
        {
            Debug.LogWarning("경로 머티리얼이 설정되지 않았습니다!");
        }

        pathSegments.Add(segment);
    }
} 