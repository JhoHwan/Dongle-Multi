using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerDongleSpawner _spawner;
    private int _playerScore = 0;
    public int Seed { get; private set; }

    private int _player1Score;
    public int Player1Score
    {
        get => _player1Score;
        set
        {
            _player1Score = value;
            InGameUIManager.Instance.ScoreBoard.SetPlayer1Socre(value);
        }
    }
    private int _player2Score;
    public int Player2Score
    {
        get => _player2Score;
        set
        {
            _player2Score = value;
            InGameUIManager.Instance.ScoreBoard.SetPlayer2Socre(value);
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _spawner.GameMgr = this;
        Seed = Random.Range(int.MinValue, int.MaxValue);
    }

    private void Start()
    {
        InGameUIManager.Instance.SetPlayerNameText(1, "Player1");
        InGameUIManager.Instance.SetPlayerNameText(2, "Player2");
        InGameUIManager.Instance.Timer.SetTimer(300);
        InGameUIManager.Instance.Timer.StartTimer(GameOver);
    }

    public void GameOver()
    {
        _spawner.CanDrop = false;
    }

    public void IncreaseScore(int score)
    {
        _playerScore += score;
    }
}
