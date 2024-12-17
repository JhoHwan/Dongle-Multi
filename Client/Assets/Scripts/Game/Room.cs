using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum RoomState
{
    WAITING,
    INGAME,
    FINISHED,
    INVALID
}

public class Room : MonoBehaviour
{
    [SerializeField] private PlayerDongleSpawner _playerSpawner;
    [SerializeField] private DongleSpawner _enemySpawner;

    private ushort _roomID;
    public ushort RoomID { get => _roomID; }
    private ushort _playerID;
    private ushort _enemyID;
    private RoomState _roomState = RoomState.WAITING;

    private bool _isPlayerReady = false;
    private bool _isEnemyEnter = false;

    public Text readyText;
    public Image readyImage;
    public Text PlayerName;
    public Text EnemyName;
    public IngameTimer _timer;
    public GameObject _gameOver;

    public InGameScoreBoard PlayerBoard;

    private int _playerScore;
    public int PlayerScore
    {
        get => _playerScore;
        set
        {
            _playerScore = value;
            PlayerBoard.SetPlayer1Socre(value);
        }
    }

    private int _enemyrScore;
    public int EnemyScore
    {
        get => _enemyrScore;
        set
        {
            _enemyrScore = value;
            PlayerBoard.SetPlayer2Socre(value);
        }
    }

    public void Start()
    {
        GameManager.Instance.Room = this;
        _roomID = 0;
        _playerID = GameManager.Instance.PlayerID;
    }

    private void Init(RoomInfo roomInfo)
    {
        _roomID = roomInfo.id;

    }

    public void EnterRoom(ushort id)
    {
        if (id == _playerID)
        {
            Debug.Log($"Enter Room Success");
            PlayerName.text = id.ToString();
        }
        else
        {
            _isEnemyEnter = true;
            _enemyID = id;
            Debug.Log($"Player {id} Enter!");
            EnemyName.text = id.ToString();
        }
    }

    public void PlayerReady()
    {
        if (_isPlayerReady || !_isEnemyEnter) return;

        _isPlayerReady = true;
        CG_PlayerReady packet = new CG_PlayerReady();
        packet.playerID = _playerID;
        packet.roomID = _roomID;
        Color color;
        ColorUtility.TryParseHtmlString("#FFD0D0", out color);
        readyImage.color = color;
        ClientPacketHandler.Instance.Send_CG_PlayerReady(packet);
    }

    public void EnemyReady()
    {

        readyText.gameObject.SetActive(true);
    }

    public void ExitRoom(ushort id)
    {
        if(id == _enemyID)
        {
            _isPlayerReady = false;
            _isEnemyEnter = false;

            readyImage.color = Color.white;
            EnemyName.text = "Wait";
            readyText.gameObject.SetActive(false);
        }
    }

    public void UpdateDongle(List<DongleInfo> dongleInfos)
    {
        _enemySpawner.UpdateDongle(dongleInfos);
    }

    public void DeleteDongle(ushort id)
    {
        _enemySpawner.DeleteDongle(id);
    }

    public void GameStart()
    {
        Debug.Log("Game Start!");
        readyText.gameObject.SetActive(false);
        _playerSpawner.StartGame();
        readyImage.gameObject.SetActive(false);
        _timer.SetTimer(300, GameOver);
        _timer.StartTimer();
    }

    public void GameOver()
    {
        _playerSpawner.EndGame();
        _timer.gameObject.SetActive(false);
        _gameOver.SetActive(true);
    }
}
