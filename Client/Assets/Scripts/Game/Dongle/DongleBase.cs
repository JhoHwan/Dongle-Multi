using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DongleBase : MonoBehaviour
{
    protected DongleSpawnerBase _owner;

    // Components
    protected Rigidbody2D _rigidbody;
    protected CircleCollider2D _collider;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public static readonly ushort MAX_LEVEL = 7;

    public bool IsEable { get; set; }
    protected DongleInfo _info = new DongleInfo();

    public ushort Level
    {
        get => _info.level;
        protected set
        {
            _info.level = (ushort)Mathf.Min(value, MAX_LEVEL);

            _animator.SetInteger("Level", _info.level);
            SetScale();
        }
    }

    public virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Init(ushort id, ushort level)
    {
        IsEable = true;
        Level = level;
        _info.id = id;
    }

    public virtual void DeInit()
    {
        IsEable = false;
        transform.parent = null;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _owner.DeSpawn(this);
    }

    public void SetPool(DongleSpawnerBase owner)
    {
        _owner = owner;
    }

    public float GetRadius()
    {
        float radius = transform.localScale.x * _collider.radius;
        return radius;
    }

    protected void SetScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = 0.8f + 0.4f * _info.level;
        scale.y = 0.8f + 0.4f * _info.level;
        transform.localScale = scale;
    }

    public virtual DongleInfo GetDongleInfo()
    {
        _info.x = transform.localPosition.x;
        _info.y = transform.localPosition.y;
        _info.rotation = transform.rotation.eulerAngles.z;
        _info.level = Level;
        _info.isEnable = (byte)(IsEable ? 1 : 0);

        return _info;
    }

    public virtual void SetDongleInfo(DongleInfo info)
    {

    }
}
