using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDongle : DongleBase
{
    private bool _canDie;
    TransformSyncSender _syncSender;

    public override void Awake()
    {
        base.Awake();
        _syncSender = GetComponent<TransformSyncSender>();
    }

    private void Start()
    {
        _syncSender.Init(0.33f ,() =>
        {
            CG_SendDonglePool packet = new CG_SendDonglePool();
            DongleInfo info = new DongleInfo();
            info.id = 0;
            info.x = transform.localPosition.x;
            info.y = transform.localPosition.y;
            info.rotation = transform.localRotation.z;
            packet.dongleInfos = new List<DongleInfo> { info };
            packet.playerID = GameManager.Instance.PlayerID;
            ClientPacketHandler.Instance.Send_CG_SendDonglePool(packet);
        });
    }

    public void Init(int level)
    {
        Level = level;
    }

    public void Drop()
    {
        _rigidbody.simulated = true;
        Invoke(nameof(CanDie), 1.5f);
    }

    void CanDie()
    {
        _canDie = true;
    }

    public void Merge(PlayerDongle other)
    {
        if(_level == MAX_LEVEL)
        {
            Destroy(gameObject);
        }
        Destroy(other.gameObject);

        Vector3 newPos = (other.transform.position + transform.position) / 2.0f;
        transform.position = newPos;

        Level += 1;

       // GameMgr.Player1Score += (Level + 1) * (Level + 2) / 2;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_canDie) return;

        _spriteRenderer.color = Color.red;
        _canDie = false;
        GameManager.Instance.Room.GameOver();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Dongle")) return;
        
        PlayerDongle other = collision.gameObject.GetComponent<PlayerDongle>();
        if (other.Level != _level) return;

        if((transform.position.y > other.transform.position.y) || 
            ((transform.position.y == other.transform.position.y) && (transform.position.x > other.transform.position.x)))
        {
            Merge(other);
        }
    }
}
