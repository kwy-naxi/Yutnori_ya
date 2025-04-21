using UnityEngine;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance { get; private set; }

    public GameObject piecePrefab;
    public Material[] playerMaterials;  // 플레이어별 말 재질
    public int piecesPerPlayer = 4;     // 각 플레이어당 말 개수
    [SerializeField] private float pieceSpacing = 1.5f;    // Inspector에서 수정 가능하도록 SerializeField 추가

    private List<Piece>[] playerPieces;
    private Vector3[] startPositions;

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

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        Debug.Log($"PieceManager Start - Piece Spacing: {pieceSpacing}");
        InitializeStartPositions();
        CreatePieces();
    }

    private void InitializeStartPositions()
    {
        float boardSize = 10f;
        float offset = boardSize * 0.3f;

        startPositions = new Vector3[]
        {
            new Vector3(-offset - 1f, 0.5f, -offset - 2f),  // 플레이어 1 시작 위치 (왼쪽 아래)
            new Vector3(offset + 1.5f, 0.5f, offset + 1.5f)     // 플레이어 2 시작 위치 (오른쪽 위)
        };
        Debug.Log($"Start positions initialized - P1: {startPositions[0]}, P2: {startPositions[1]}");
    }

    private void CreatePieces()
    {
        playerPieces = new List<Piece>[2];

        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            playerPieces[playerIndex] = new List<Piece>();
            Vector3 basePosition = startPositions[playerIndex];
            Debug.Log($"Creating pieces for Player {playerIndex + 1} at {basePosition}");

            // 말들을 일렬로 배치
            for (int i = 0; i < piecesPerPlayer; i++)
            {
                float xOffset = i * pieceSpacing - ((piecesPerPlayer - 1) * pieceSpacing / 2);
                Vector3 position = basePosition + new Vector3(xOffset, 0, 0);
                CreatePiece(playerIndex, position);
                Debug.Log($"Created piece {i + 1} for Player {playerIndex + 1} at {position}");
            }
        }
    }

    private void CreatePiece(int playerIndex, Vector3 position)
    {
        if (piecePrefab == null)
        {
            Debug.LogError("말 프리팹이 설정되지 않았습니다!");
            return;
        }

        GameObject pieceObj = Instantiate(piecePrefab, position, Quaternion.identity);
        pieceObj.name = $"Piece_P{playerIndex + 1}_{playerPieces[playerIndex].Count + 1}";
        pieceObj.transform.parent = transform;

        Piece piece = pieceObj.GetComponent<Piece>();
        if (piece != null)
        {
            // 플레이어 Material 설정
            Material playerMaterial = playerIndex < playerMaterials.Length ? playerMaterials[playerIndex] : null;
            piece.Initialize(playerIndex, playerMaterial);
            playerPieces[playerIndex].Add(piece);
        }
    }

    public List<Piece> GetPlayerPieces(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < playerPieces.Length)
        {
            return playerPieces[playerIndex];
        }
        return null;
    }

    public bool CheckWinCondition(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerPieces.Length)
        {
            return false;
        }

        // 모든 말이 도착점에 도달했는지 확인
        foreach (var piece in playerPieces[playerIndex])
        {
            if (!piece.IsAtGoal())
            {
                return false;
            }
        }

        return true;
    }
} 