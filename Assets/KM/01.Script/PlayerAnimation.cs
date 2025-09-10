using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private Vector2 _dir;

    private readonly int _blendTreeHash = Animator.StringToHash("DefaultMove");
    private readonly int _isJumpHash = Animator.StringToHash("IsJump");
    private readonly int _isDashHash = Animator.StringToHash("IsDash");
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(_dir.x != 0)
        {
            _animator.SetFloat(_blendTreeHash, 1);
        }
        else
        {
            _animator.SetFloat(_blendTreeHash, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        _dir = value.Get<Vector2>();
        if(_dir.x >= 0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(_dir.x <= -0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void OnJump()
    {
        _animator.SetTrigger(_isJumpHash);
    }

    public void OnDash()
    {
       _animator.SetTrigger(_isDashHash);
    }
}
