using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static BackendFunctionInGame;

public class OtherMovement : Player
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 9.8f;

    /*[SerializeField]*/ private Vector2 groundCheckVecSize = new Vector2(0.5f, 1.05f);
    [SerializeField] private Vector2 groundCheckVec;
    /*[SerializeField]*/ private LayerMask groundMask;

    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec;

    private bool _isGrounded;
    #region NetWorkData
    //점프를 했는가? (Is Jumping Now? <bool>)
    private bool _isJumping = false;
    //대쉬를 하고 있는가?(Is Dashing Now? <bool>)
    private bool _isRunning;
    //대쉬할 방향(Dash Direction<Vec2>)
    private Vector2 _dashDirection;
    //대쉬를 사용했는가? (Use Dash? <bool>)
    private bool _isDashing = false;
    //이동하고 있는 방향 (Now Move.X Direction <Sbyte>)
    private sbyte nowMoveDir;
    #endregion

    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
        groundMask = LayerMask.GetMask("Ground");
        groundCheckVecSize = new Vector2(0.5f, 1.05f);
    }
    private void Update()
    {
        OnGround();
        //대쉬중이 아니고 달리기 중이 아닐 때 기본적인 움직임 실시
        if (!_isDashing && _isRunning)
        {
            Vector2 velocity = _rbCompo.linearVelocity;
            velocity.x = _moveVec.x * speed;
            _rbCompo.linearVelocityX = velocity.x;
        }
    }
    private void OnGround()
    {
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckVec, groundCheckVecSize, 0, groundMask);
        _isGrounded = hit != null;

        if (_isGrounded)
        {
            _isJumping = false;
            _isDashing = false;
        }
    }
    public void OnJump()
    {
        _rbCompo.linearVelocityY = jumpForce;
        _isJumping = true;
    }
    private void GroundDash()
    {
        _rbCompo.AddForce(new Vector2(_dashDirection.x, 0) * dashForce, ForceMode2D.Impulse); 
    }

    private void AirDash()
    {
        _rbCompo.linearVelocity = _dashDirection * dashForce / 2f;
    }

    public void OnDash()
    {
        if (!_isDashing)
        {
            if (_isGrounded)
            {
                _rbCompo.linearVelocityX = 0;
            }
            else
            {
                _rbCompo.linearVelocity = Vector2.zero;
            }
            if (_isRunning) return;

            Vector2 inputDir = _moveVec.normalized;

            if (_isGrounded)
            {
                if (inputDir == Vector2.zero)
                    inputDir = Vector2.down;
            }
            else
            {
                inputDir = new Vector2(Mathf.Sign(_moveVec.x), 0);
            }

            _dashDirection = inputDir.normalized;
        }
    }
    public override void ApplyByteData(byte byteData)
    {
        //대쉬를 했는가?
        bool isDashing = (byteData & (byte)flagPlayerMovementState.IsDashing) != 0;
        _isDashing = isDashing;
        if (isDashing)
        {
            Debug.Log($"isDash : {isDashing}");
            OnDash();
            GroundDash();
            AirDash();
        }

        //달리고 있는가?
        bool isRunning = (byteData & (byte)flagPlayerMovementState.IsRunning) != 0;
        _isRunning = isRunning;

        //점프를 하고 있는가?
        bool isJumping = (byteData & (byte)flagPlayerMovementState.IsJumping) != 0;
        if (_isGrounded == true && isJumping == true)
        {
            Debug.Log($"isjumping : {isJumping}");
            OnJump();
        }
    }

    public override void ApplySbyteData(sbyte sbyteData)
    {
        Debug.Log("moveX");
        float moveX = sbyteData;
        _moveVec.x = moveX;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}