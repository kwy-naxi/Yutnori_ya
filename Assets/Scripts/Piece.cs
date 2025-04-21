using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    public int playerIndex;  // 소유 플레이어 인덱스
    public int currentPathIndex = -1;  // 현재 경로 상의 위치
    public float moveSpeed = 5f;  // 이동 속도
    public float moveHeight = 1f;  // 이동 시 높이
    
    private bool isMoving = false;
    private Vector3 startPosition;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Initialize(int ownerIndex, Material playerMaterial)
    {
        playerIndex = ownerIndex;
        startPosition = transform.position;
        currentPathIndex = -1;  // 시작 위치

        // 플레이어 Material 설정
        if (meshRenderer != null && playerMaterial != null)
        {
            meshRenderer.material = playerMaterial;
        }
    }

    public void Move(int steps)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveCoroutine(steps));
        }
    }

    private IEnumerator MoveCoroutine(int steps)
    {
        isMoving = true;
        
        // 시작 위치가 보드 밖이면 첫 번째 위치로 이동
        if (currentPathIndex == -1)
        {
            currentPathIndex = 0;
            Vector3 targetPos = GameBoard.Instance.GetPathPosition(currentPathIndex);
            yield return StartCoroutine(MoveToPosition(targetPos));
            steps--;
        }

        // 남은 스텝만큼 이동
        while (steps > 0)
        {
            int nextIndex = GameBoard.Instance.GetNextPathIndex(currentPathIndex);
            if (nextIndex == -1) // 도착점에 도달
            {
                break;
            }

            currentPathIndex = nextIndex;
            Vector3 targetPos = GameBoard.Instance.GetPathPosition(currentPathIndex);
            yield return StartCoroutine(MoveToPosition(targetPos));
            steps--;
        }

        isMoving = false;
        GameManager.Instance.OnPieceMoveComplete();
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPos = transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPosition);
        float startTime = Time.time;

        while (true)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            if (fractionOfJourney >= 1f)
            {
                transform.position = targetPosition;
                break;
            }

            // 포물선 형태로 이동
            Vector3 currentPos = Vector3.Lerp(startPos, targetPosition, fractionOfJourney);
            float heightOffset = Mathf.Sin(fractionOfJourney * Mathf.PI) * moveHeight;
            currentPos.y += heightOffset;
            
            transform.position = currentPos;
            yield return null;
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        currentPathIndex = -1;
        isMoving = false;
    }

    public bool IsAtGoal()
    {
        return GameBoard.Instance.IsGoalPosition(currentPathIndex);
    }
} 