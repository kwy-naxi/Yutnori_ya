using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        WaitingForThrow,
        MovingPiece,
        GameOver
    }

    private GameState currentState;
    private int currentPlayerIndex = 0;
    private int currentMoveCount = 0;
    private Piece selectedPiece;

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
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        currentState = GameState.WaitingForThrow;
        currentPlayerIndex = 0;
        Debug.Log($"게임 시작! 플레이어 {currentPlayerIndex + 1}의 차례입니다.");
    }

    public void OnYutThrown(int moveCount)
    {
        if (currentState != GameState.WaitingForThrow) return;

        currentMoveCount = moveCount;
        Debug.Log($"플레이어 {currentPlayerIndex + 1}이(가) {GetYutName(moveCount)}을(를) 던졌습니다!");

        // 말 선택 UI 표시 또는 자동으로 말 선택
        SelectPiece();
    }

    private void SelectPiece()
    {
        var playerPieces = PieceManager.Instance.GetPlayerPieces(currentPlayerIndex);
        if (playerPieces == null || playerPieces.Count == 0)
        {
            Debug.LogError("선택할 수 있는 말이 없습니다!");
            return;
        }

        // 임시로 첫 번째 말을 선택
        selectedPiece = playerPieces[0];
        MoveSelectedPiece();
    }

    private void MoveSelectedPiece()
    {
        if (selectedPiece == null) return;

        currentState = GameState.MovingPiece;
        selectedPiece.Move(currentMoveCount, OnPieceMoveComplete);
    }

    private void OnPieceMoveComplete()
    {
        // 승리 조건 확인
        if (PieceManager.Instance.CheckWinCondition(currentPlayerIndex))
        {
            Debug.Log($"플레이어 {currentPlayerIndex + 1}이(가) 승리했습니다!");
            currentState = GameState.GameOver;
            return;
        }

        // 윷이나 모가 나왔으면 추가 이동
        if (currentMoveCount == 4 || currentMoveCount == 5)
        {
            currentState = GameState.WaitingForThrow;
            Debug.Log($"추가 이동! 플레이어 {currentPlayerIndex + 1}이(가) 다시 윷을 던집니다.");
            YutSystem.Instance.ThrowYut();
        }
        else
        {
            // 다음 플레이어로 턴 넘기기
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
            currentState = GameState.WaitingForThrow;
            Debug.Log($"플레이어 {currentPlayerIndex + 1}의 차례입니다.");
        }
    }

    private string GetYutName(int moveCount)
    {
        switch (moveCount)
        {
            case 1: return "도";
            case 2: return "개";
            case 3: return "걸";
            case 4: return "윷";
            case 5: return "모";
            default: return "알 수 없음";
        }
    }
} 