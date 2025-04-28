using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;

    private int playerIndex;
    private Material pieceMaterial;
    private int currentPathIndex = -1;
    private bool isMoving = false;

    public void Initialize(int playerIndex, Material material)
    {
        this.playerIndex = playerIndex;
        pieceMaterial = material;
        GetComponent<Renderer>().material = material;
    }

    public void Move(int steps, System.Action onComplete)
    {
        if (isMoving) return;
        StartCoroutine(MoveCoroutine(steps, onComplete));
    }

    private IEnumerator MoveCoroutine(int steps, System.Action onComplete)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            int nextPathIndex = currentPathIndex + 1;
            if (nextPathIndex >= GameBoard.Instance.PathPoints.Count)
            {
                Debug.Log("말이 도착점에 도달했습니다!");
                break;
            }

            Vector3 startPos = transform.position;
            Vector3 endPos = GameBoard.Instance.GetPathPointPosition(nextPathIndex);
            float journeyLength = Vector3.Distance(startPos, endPos);
            float startTime = Time.time;

            while (transform.position != endPos)
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;

                // 점프 애니메이션
                float height = Mathf.Sin(fractionOfJourney * Mathf.PI) * jumpHeight;
                Vector3 newPos = Vector3.Lerp(startPos, endPos, fractionOfJourney);
                newPos.y = height;

                transform.position = newPos;
                yield return null;
            }

            currentPathIndex = nextPathIndex;
        }

        isMoving = false;
        onComplete?.Invoke();
    }

    public void ResetPosition()
    {
        currentPathIndex = -1;
        transform.position = PieceManager.Instance.GetPlayerPieces(playerIndex)[0].transform.position;
    }

    public bool IsAtGoal()
    {
        return currentPathIndex >= GameBoard.Instance.PathPoints.Count - 1;
    }
} 