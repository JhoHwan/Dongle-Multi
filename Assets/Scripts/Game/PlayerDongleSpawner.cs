using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDongleSpawner : DongleSpawner
{
    [SerializeField] private float _speed; 

    // 이동 관련
    private PlayerInputActions _inputActions;
    private float _moveValue;
    private Vector3 _spawnPosition;

    // 동글 관련
    private Dongle _curDongle;
    private System.Random _random;
    private bool _canDrop = true;
    public bool CanDrop { get => _canDrop; set => _canDrop = value; }
    private int _nextLevel;

    private WaitForSeconds _sleep = new WaitForSeconds(1.0f);


    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _spawnPosition = transform.position;
        _random = new System.Random(GameMgr.Seed);

        TurnOnLine(true);
    }

    private void Start()
    {
        _curDongle = SpawnDongle(0);
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
    }

    public void DropDongle()
    {
        if (!CanDrop) return;

        if (_curDongle == null) return;

        TurnOnLine(false);

        _curDongle.Drop();
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
}
