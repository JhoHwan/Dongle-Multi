using UnityEngine;

public class PlayerDongle : DongleBase
{
    private bool _canDie;
    public bool IsDrop { get; set; }

    public override void Awake()
    {
        base.Awake();
    }

    public void Drop()
    {
        _rigidbody.simulated = true;
        IsDrop = true;
        Invoke(nameof(CanDie), 1.5f);
    }

    void CanDie()
    {
        _canDie = true;
    }

    public override void Init(ushort id, ushort level)
    {
        base.Init(id, level);
        _canDie = false;
        _rigidbody.simulated = false;
        IsDrop = false;

    }

    public void Merge(PlayerDongle other)
    {
        if(Level == MAX_LEVEL)
        {
            Destroy(gameObject);
        }

        Vector3 newPos = (other.transform.localPosition + transform.localPosition) / 2.0f;
        transform.localPosition = newPos;
        int score = (Level + 1) * (Level + 2) / 2; ;
        GameManager.Instance.Room.PlayerScore += score;
        Debug.Log("Score : " + score);
        Level += 1;

        CG_MergeDongle packet = new CG_MergeDongle();
        packet.dongleID = other.GetDongleInfo().id;
        packet.roomID = GameManager.Instance.Room.RoomID;
        packet.playerID = GameManager.Instance.PlayerID;

        ClientPacketHandler.Instance.Send_CG_MergeDongle(packet);
        other.DeInit();
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
        if (other.Level != Level) return;

        if((transform.position.y > other.transform.position.y) || 
            ((transform.position.y == other.transform.position.y) && (transform.position.x > other.transform.position.x)))
        {
            Merge(other);
        }
    }


}
