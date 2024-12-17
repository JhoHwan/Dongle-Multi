using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private PlayerDongleSpawner _playerSpawner;
    [SerializeField] public DongleSpawner _spawner;

    public int player1_ID;
    public int roomID;
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
    public void GameOver()
    {
        _playerSpawner.CanDrop = false;
    }

    private void StartGame()
    {
        InGameUIManager.Instance.SetPlayerNameText(1, "Player1");
        InGameUIManager.Instance.SetPlayerNameText(2, "Player2");
        InGameUIManager.Instance.Timer.SetTimer(300);
        InGameUIManager.Instance.Timer.StartTimer(GameOver);
    }
}
