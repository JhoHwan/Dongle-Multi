using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDongleSpawner : DongleSpawnerBase
{
    [SerializeField] private Transform _donglePool;
    [SerializeField] private float _speed; 

    // 이동 관련
    private PlayerInputActions _inputActions;
    private float _moveValue;
    private Vector3 _spawnPosition;

    // 동글 관련
    private System.Random _random;
    private bool _canDrop = true;
    public bool CanDrop { get => _canDrop; set => _canDrop = value; }
    private int _nextLevel;

    private WaitForSeconds _sleep = new WaitForSeconds(1.0f);
    private WaitForSeconds _sleep1 = new WaitForSeconds(0.1f);
    CG_SendMoveSpawner packet = new CG_SendMoveSpawner();

    private TransformSyncSender _transformSyncSender;

    public void Awake()
    {
        _transformSyncSender = GetComponent<TransformSyncSender>();
        _inputActions = new PlayerInputActions();
        _spawnPosition = transform.position;
        _random = new System.Random(GameManager.Instance.Seed);
    }

    public override void Start()
    {
        base.Start();

        packet.playerID = GameManager.Instance.PlayerID;
        packet.roomID = 0;
        _transformSyncSender.Init(SendPacketEvent);

        _nextLevel = _random.Next(0, 3);
        InGameUIManager.Instance.NextDongles[0].SetDongleImage(_nextLevel);
    }

    private void OnEnable()
    {
        _inputActions.PlayerAction.Enable();
        _inputActions.PlayerAction.Move.performed += OnMove;
        _inputActions.PlayerAction.Move.canceled += OnMove;

        _inputActions.PlayerAction.Drop.performed += OnDrop;
        _inputActions.PlayerAction.Drop.canceled += OnDrop;
    }

    private void Update()
    {
        Move(_moveValue);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 입력값을 읽어서 moveValue에 저장
        _moveValue = context.ReadValue<float>();
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        DropDongle();
    }

    private void Move(float value)
    {
        if (!CanDrop) return;

        float rightBorder = 5.475f - _dongleRadius;
        
        Vector3 position = transform.localPosition;
        position.x += value * _speed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -rightBorder, rightBorder);
        position.y = _spawnPosition.y;

        transform.localPosition = position;

        packet.x = position.x;
    }

    public void DropDongle()
    {
        if (!CanDrop) return;

        if (_curDongle == null) return;

        TurnOnLine(false);

        _curDongle.Drop();
        _curDongle.transform.parent = _donglePool;
        _curDongle = null;

        StartCoroutine(SpawnDongleRoutine());
    }

    private void OnDisable()
    {
        _inputActions.PlayerAction.Move.performed -= OnMove;
        _inputActions.PlayerAction.Move.canceled -= OnMove;

        _inputActions.PlayerAction.Drop.performed -= OnDrop;
        _inputActions.PlayerAction.Drop.canceled -= OnDrop;

        _inputActions.PlayerAction.Disable();
    }

    IEnumerator SpawnDongleRoutine()
    {
        yield return _sleep;

        _curDongle = SpawnDongle(_nextLevel);
        _nextLevel = _random.Next(0, 3);
        InGameUIManager.Instance.NextDongles[0].SetDongleImage(_nextLevel);

        TurnOnLine(true); 
    }

    private void SendPacketEvent()
    {
        packet.x = transform.localPosition.x;
        ClientPacketHandler.Instance.Send_CG_SendMoveSpawner(packet);
    }
}
