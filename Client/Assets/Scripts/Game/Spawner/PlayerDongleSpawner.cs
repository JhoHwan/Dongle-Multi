using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDongleSpawner : DongleSpawnerBase
{
    [SerializeField] private float _speed; 

    // �̵� ����
    private PlayerInputActions _inputActions;
    private float _moveValue;
    private Vector3 _spawnPosition;

    // ���� ����
    private System.Random _random;
    private bool _canDrop = true;
    public bool CanDrop { get => _canDrop; set => _canDrop = value; }
    private int _nextLevel;

    private WaitForSeconds _sleep = new WaitForSeconds(1.0f);
    private Dictionary<ushort, DongleInfo> _previousDongleStates = new Dictionary<ushort, DongleInfo>();

    public void Awake()
    {
        _inputActions = new PlayerInputActions();
        _spawnPosition = transform.position;
        _random = new System.Random(GameManager.Instance.Seed);
    }

    public override void Start()
    {
        base.Start();
        _curDongle = SpawnDongle(0, Vector3.zero, 0, transform);
        _dongleMap[_curDongle.GetDongleInfo().id] = _curDongle;

        StartCoroutine(SendDonglePoolRoutine());

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
        // �Է°��� �о moveValue�� ����
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

        PlayerDongle dongle = (PlayerDongle)_curDongle;
        if (dongle == null) return;

        dongle.Drop();
        _dongleMap[dongle.GetDongleInfo().id] = dongle;

        _curDongle.transform.parent = _pool.transform;
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

        _curDongle = SpawnDongle((ushort)_nextLevel, Vector3.zero, 0, transform);
        _dongleMap[_curDongle.GetDongleInfo().id] = _curDongle;

        _nextLevel = _random.Next(0, 3);
        InGameUIManager.Instance.NextDongles[0].SetDongleImage(_nextLevel);

        TurnOnLine(true); 
    }

    IEnumerator SendDonglePoolRoutine()
    {
        WaitForSeconds sleep = new WaitForSeconds(NetWorkManager.Instance.SendInterval);
        CG_SendDonglePool packet = new CG_SendDonglePool();
        packet.playerID = GameManager.Instance.PlayerID;
        packet.dongleInfos = new List<DongleInfo>();
        List<ushort> keysToRemove = new List<ushort>();

        while (true)
        {
            yield return sleep;
            if (_dongleMap.Count == 0) continue;

            foreach (DongleBase dongleBase in _dongleMap.Values)
            {
                PlayerDongle dongle = (PlayerDongle)dongleBase;
                DongleInfo currentInfo = dongle.GetDongleInfo();
                if (dongle.IsDrop == false) currentInfo.x = transform.localPosition.x;

                // ���� ���¿� ��
                if (!_previousDongleStates.ContainsKey(currentInfo.id) ||
                    !DongleInfoEquals(_previousDongleStates[currentInfo.id], currentInfo))
                {
                    // ���°� ����� ��츸 ��Ŷ�� �߰�
                    packet.dongleInfos.Add(currentInfo);

                    // ���� ���¸� ����
                    _previousDongleStates[currentInfo.id] = currentInfo;
                }

                if (currentInfo.isEnable == 0)
                {
                    keysToRemove.Add(currentInfo.id); // ������ Ű�� ����
                }
            }

            // ���� ���� �Ѳ����� ����
            foreach (ushort key in keysToRemove)
            {
                _dongleMap.Remove(key);
            }

            // ���°� ����� ���۸� ������ ����
            if (packet.dongleInfos.Count > 0)
            {
                ClientPacketHandler.Instance.Send_CG_SendDonglePool(packet);
                packet.dongleInfos.Clear();
            }
        }
    }

    // DongleInfo �� �Լ� (�ʵ� �� ��)
    private bool DongleInfoEquals(DongleInfo a, DongleInfo b)
    {
        return a.id == b.id &&
               a.x == b.x &&
               a.y == b.y &&
               a.level == b.level &&
               a.rotation == b.rotation &&
               a.isEnable == b.isEnable;
    }
}
