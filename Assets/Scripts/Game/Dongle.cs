using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _collider;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    public GameManager GameMgr { get; set; }

    private bool _canDie;

    private int _level;
    public static readonly int MAX_LEVEL = 7;

    public int Level 
    {   
        get => _level; 
        private set
        {
            _level = Mathf.Min(value, MAX_LEVEL);

            _animator.SetInteger("Level", _level);
            SetScale();
        }
    }

    void Awake()
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

    public void Init(int level)
    {
        Level = level;
    }

    public void SetScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = 0.8f + 0.4f * _level;
        scale.y = 0.8f + 0.4f * _level;
        transform.localScale = scale;
    }

    public void Drop()
    {
        _rigidbody.simulated = true;

        transform.SetParent(null);
        Invoke(nameof(CanDie), 1.5f);
    }

    void CanDie()
    {
        _canDie = true;
    }

    public void Merge(Dongle other)
    {
        if(_level == MAX_LEVEL)
        {
            Destroy(gameObject);
        }
        Destroy(other.gameObject);

        Vector3 newPos = (other.transform.position + transform.position) / 2.0f;
        transform.position = newPos;

        Level += 1;

        GameMgr.Player1Score += (Level + 1) * (Level + 2) / 2;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_canDie) return;

        _spriteRenderer.color = Color.red;
        _canDie = false;
        GameMgr.GameOver();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Dongle")) return;
        
        Dongle other = collision.gameObject.GetComponent<Dongle>();
        if (other.Level != _level) return;

        if((transform.position.y > other.transform.position.y) || 
            ((transform.position.y == other.transform.position.y) && (transform.position.x > other.transform.position.x)))
        {
            Merge(other);
        }
    }
}
