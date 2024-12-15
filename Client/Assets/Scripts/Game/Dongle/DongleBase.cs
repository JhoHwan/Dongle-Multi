using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DongleBase : MonoBehaviour
{
    // Components
    protected Rigidbody2D _rigidbody;
    protected CircleCollider2D _collider;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    protected int _level;
    public static readonly int MAX_LEVEL = 7;

    public int Level
    {
        get => _level;
        protected set
        {
            _level = Mathf.Min(value, MAX_LEVEL);

            _animator.SetInteger("Level", _level);
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

    public float GetRadius()
    {
        float radius = transform.localScale.x * _collider.radius;
        return radius;
    }

    public void SetScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = 0.8f + 0.4f * _level;
        scale.y = 0.8f + 0.4f * _level;
        transform.localScale = scale;
    }
}
