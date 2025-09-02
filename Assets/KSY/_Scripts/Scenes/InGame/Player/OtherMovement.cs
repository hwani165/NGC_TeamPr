using BackEnd;
using UnityEngine;
using UnityEngine.InputSystem;
using static BackendFunctionInGame;

public class OtherMovement : Player
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private Vector2 groundCheckVecSize = new Vector2(0.5f, 1.05f);
    [SerializeField] private Vector2 groundCheckVec;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec;

    private bool _isGrounded;
    #region NetWorkData
    //점프를 했는가? (Is Jumping Now? <bool>)
    private bool _usingJump = false;
    //대쉬를 하고 있는가?(Is Dashing Now? <bool>)
    private bool _isDashing;
    //대쉬할 방향(Dash Direction<Vec2>)
    private Vector2 _dashDir;
    //대쉬를 사용했는가? (Use Dash? <bool>)
    private bool CanDash = false;
    //이동하고 있는 방향 (Now Move.X Direction <Sbyte>)
    #endregion

    private float _dashTimer;

    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
        groundMask = LayerMask.GetMask("Ground");
        groundCheckVecSize = new Vector2(0.5f, 1.05f);
    }
    private void Update()
    {
        //if (Keyboard.current.sKey.wasPressedThisFrame && _isGrounded)
        //{
        //    _rbCompo.AddForce(Vector2.down * gravity * 1.5f, ForceMode2D.Impulse);
        //}

        OnGround();
        GroundDash();
        AirDash();
        if (!_isDashing)
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
            _usingJump = false;
            CanDash = false;
        }
    }
    public void OnJump()
    {
        _usingJump = true;
        _rbCompo.linearVelocityY = jumpForce;
    }

    private void GroundDash()
    {
        if (_isDashing && _isGrounded)
        {
            _rbCompo.AddForce(new Vector2(_dashDir.x, 0) * dashForce, ForceMode2D.Impulse);
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
            return;
        }
    }

    private void AirDash()
    {
        if (_isDashing && !_isGrounded)
        {
            _rbCompo.linearVelocity = _dashDir * dashForce / 2f;
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            return;
        }
    }

    public void OnDash()
    {
        if (!CanDash)
        {
            if (_isGrounded)
            {
                _rbCompo.linearVelocityX = 0;
                CanDash = true;
            }
            else
            {
                CanDash = false;
                _rbCompo.linearVelocity = Vector2.zero;
            }
            if (_isDashing) return;

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

            _dashDir = inputDir.normalized;
            _isDashing = true;
            _dashTimer = dashDuration;
        }
    }
    public override void ApplyByteData(byte state)
    {
        //대쉬를 했는가?
        bool usingDash = (state & (byte)flagPlayerMovementState.UsingDash) != 0;
        CanDash = usingDash;
        if (!_isDashing && usingDash)
        {
            Debug.Log($"isDash : {usingDash}");
            OnDash();
            GroundDash();
            AirDash();
        }

        //달리고 있는가?
        bool isDashing = (state & (byte)flagPlayerMovementState.IsDashing) != 0;
        _isDashing = isDashing;

        //점프를 하고 있는가?
        bool isJumping = (state & (byte)flagPlayerMovementState.IsJumping) != 0;
        if (_isGrounded && isJumping)
        {
            Debug.Log($"isjumping : {isJumping}");
            OnJump();
        }
    }
    public override void ApplySbyteData(sbyte moveX, sbyte dashX, sbyte dashY)
    {
        float _moveX = moveX;
        _moveVec.x = _moveX;
        _dashDir = new Vector2(dashX, dashY);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}