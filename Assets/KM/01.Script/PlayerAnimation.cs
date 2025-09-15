using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    private readonly int _blendTreeHash = Animator.StringToHash("DefaultMove");
    private readonly int _isJumpHash = Animator.StringToHash("IsJump");
    private readonly int _isDashHash = Animator.StringToHash("IsDash");

    private MyMovement _myMovement;
    private OtherMovement _otherMovement;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (TryGetComponent<MyMovement>(out MyMovement myMovement))
        {
            myMovement = _myMovement;
        }

        if (TryGetComponent<OtherMovement>(out OtherMovement otherMovement))
        {
            _otherMovement = otherMovement;
        }
    }
    private void ForMyMovement()
    {
        if (_myMovement._moveX != 0)
        {
            _animator.SetFloat(_blendTreeHash, 1);
            if (_myMovement._moveX > 0.1)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        else
        {
            _animator.SetFloat(_blendTreeHash, 0);
        }

        if (_myMovement._usingJump)
        {
            _animator.SetTrigger(_isJumpHash);
        }

        if (_myMovement._usingDash)
        {
            _animator.SetTrigger(_isDashHash);
        }
    }

    private void ForOtherMovement()
    {
        if (_otherMovement._moveVec.x != 0)
        {
            _animator.SetFloat(_blendTreeHash, 1);
            if (_otherMovement._moveVec.x > 0.1)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        else
        {
            _animator.SetFloat(_blendTreeHash, 0);
        }

        if (_otherMovement._usingJump)
        {
            _animator.SetTrigger(_isJumpHash);
        }

        if (_otherMovement._usingDash)
        {
            _animator.SetTrigger(_isDashHash);
        }
    }
    private void Update()
    {
        if(_myMovement != null)
            ForMyMovement();
        else
            ForOtherMovement();
    }
}
