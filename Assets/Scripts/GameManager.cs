using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        WaitingForThrow,
        SelectingPiece,
        MovingPiece,
        GameOver
    }

    public GameState CurrentState { get; private set; }
    public int CurrentPlayer { get; private set; }
    public int NumberOfPlayers = 2;

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
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
        CurrentPlayer = 0;
        CurrentState = GameState.WaitingForThrow;
        Debug.Log($"게임 시작! 플레이어 {CurrentPlayer + 1}의 턴입니다.");
    }

    public void OnYutThrown(int moveCount)
    {
        if (CurrentState != GameState.WaitingForThrow) return;

        Debug.Log($"윷 결과: {GetYutResultName(moveCount)} ({moveCount}칸)");
        CurrentState = GameState.SelectingPiece;
    }

    public void OnPieceSelected(Piece piece, int moveCount)
    {
        if (CurrentState != GameState.SelectingPiece) return;

        CurrentState = GameState.MovingPiece;
        piece.Move(moveCount);
    }

    public void OnPieceMoveComplete()
    {
        if (CurrentState != GameState.MovingPiece) return;

        // 다음 플레이어로 턴 전환
        CurrentPlayer = (CurrentPlayer + 1) % NumberOfPlayers;
        CurrentState = GameState.WaitingForThrow;
        
        Debug.Log($"플레이어 {CurrentPlayer + 1}의 턴입니다.");
    }

    private string GetYutResultName(int moveCount)
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