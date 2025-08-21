using BackEnd;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 9.8f;

    [Header("Ground Check")]
    [SerializeField] private Vector2 groundCheckVecSize;
    [SerializeField] private Vector2 groundCheckVec;
    [SerializeField] private LayerMask groundMask;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
//realVer
    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec;
    private bool _isGrounded;
    private bool _isDashing;
    private float _dashTimer;
    private Vector2 _dashDirection;

    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
    }

    private void FixedUpdate()
    {
        if(Backend.Match.IsSuperGamer())
        {
            //슈퍼 게이머 처리
            OnGround();

            if (_isDashing)
            {
                _rbCompo.linearVelocity = _dashDirection * dashForce;
                _dashTimer -= Time.fixedDeltaTime;
                if (_dashTimer <= 0f)
                {
                    _isDashing = false;
                }
                return;
            }

            Vector2 velocity = _rbCompo.linearVelocity;
            velocity.x = _moveVec.x * speed;
            _rbCompo.linearVelocityX = velocity.x;
        }
        else
        {
            //클라이언트 처리
        }

    }

    private void OnGround()
    {
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckVec, groundCheckVecSize, 0, groundMask);
        _isGrounded = hit != null;
    }

    public void OnMove(InputValue value)
    {
        _moveVec = value.Get<Vector2>();
    }

    public void OnJump()
    {
        Debug.Log(_isGrounded);
        if (_isGrounded && !_isDashing)
        {
            _rbCompo.linearVelocityY = jumpForce;
        }
    }

    public void OnDash(InputValue value)
    {
        if (_isDashing) return;

        Vector2 inputDir = _moveVec.normalized;

        if (!_isGrounded)
        {
            if (inputDir == Vector2.zero)
                inputDir = Vector2.down; 
        }
        else
        {
            inputDir = new Vector2(Mathf.Sign(_moveVec.x), 0);
        }

        _dashDirection = inputDir.normalized;
        _isDashing = true;
        _dashTimer = dashDuration;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}