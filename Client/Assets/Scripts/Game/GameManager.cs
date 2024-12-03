using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    Queue<Action> jobQueue = new Queue<Action>();
    public ushort PlayerID = 0;

    [SerializeField] private PlayerDongleSpawner _playerSpawner;
    [SerializeField] public DongleSpawner _spawner;

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
        Init();
        Application.targetFrameRate = 60;
        Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
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
        _playerSpawner.CanDrop = false;
    }

    public void IncreaseScore(int score)
    {
        _playerScore += score;
    }

    public void SpawnerMove(GC_BroadCastMoveSpawner packet)
    {
        if (packet.playerID == PlayerID) return;

        Vector2 position = new Vector2(packet.x, 5);
        _spawner.transform.localPosition = position;
    }

    private void OnDestroy()
    {
        DeInit();
    }

    public void CreateJob(Action job)
    {
        jobQueue.Enqueue(job);
    }

    private void Update()
    {
        while (jobQueue.Count > 0)
        {
            Action action = jobQueue.Dequeue();
            action();
        }
    }
}
