//using UnityEngine;
//using UnityEngine.InputSystem;

//public class MyMovement : Player, ISender
//{
//    [SerializeField] private float speed = 10f;
//    [SerializeField] private float jumpForce = 12f;
//    [SerializeField] private float gravity = 9.8f;

//    /*[SerializeField]*/ private Vector2 groundCheckVecSize = new Vector2(0.5f, 1.05f);
//    [SerializeField] private Vector2 groundCheckVec;
//    /*[SerializeField]*/ private LayerMask groundMask;

//    [SerializeField] private float dashForce = 20f;
//    [SerializeField] private float dashDuration = 0.2f;

//    [SerializeField] private int maxJumpCount = 3;
//    private int currentJumpCount;

//    private Rigidbody2D _rbCompo;
//    private Vector2 _moveVec;

//    private bool _isGrounded;

//    private float _RunningTimer;

//    #region NetWorkData
//    //점프를 했는가? (Is Jumping Now? <bool>)
//    private bool _isJumping = false;
//    //대쉬를 하고 있는가?(Is Dashing Now? <bool>)
//    //private bool _isRunning;
//    //대쉬할 방향(Dash Direction<Vec2>)
//    private Vector2 _dashDirection;
//    //대쉬를 사용했는가? (Use Dash? <bool>)
//    private bool _isDashing = false;
//    //이동하고 있는 방향 (Now Move.X Direction <Sbyte>)
//    private sbyte _nowMoveDir;
//    #endregion



//    private void Start()
//    {
//        _rbCompo = GetComponent<Rigidbody2D>();
//        _rbCompo.gravityScale = 1f;
//        currentJumpCount = maxJumpCount;
//        groundMask = LayerMask.GetMask("Ground");
//        groundCheckVecSize = new Vector2(0.5f, 1.05f);
//    }

//    private void FixedUpdate()
//    {
//        OnGround();
//        GroundDash();
//        AirDash();
//        //대쉬중이 아니고 달리기 중이 아닐 때 기본적인 움직임 실시
//        if (!_isDashing /*&& _isRunning*/)
//        {
//            Vector2 velocity = _rbCompo.linearVelocity;
//            velocity.x = _moveVec.x * speed;
//            _rbCompo.linearVelocityX = velocity.x;
//        }
//    }

//    private void Update()
//    {
//        //공중에 있고 S 키를 눌렀을 경우
//        if (Keyboard.current.sKey.wasPressedThisFrame && !_isGrounded)
//        {
//            //아래쪽 방향으로 다운
//            _rbCompo.AddForce(Vector2.down * gravity * 1.5f, ForceMode2D.Impulse);
//        }
//    }

//    private void OnGround()
//    {
//        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckVec, groundCheckVecSize, 0, groundMask);
//        _isGrounded = hit != null;

//        if (_isGrounded)
//        {
//            currentJumpCount = maxJumpCount;
//            _isJumping = false;
//            _isDashing = false;
//        }
//    }

//    public void OnMove(InputValue value)
//    {
//        _moveVec = value.Get<Vector2>();
//        if (_moveVec.x >= 0.1f)
//        {
//            _nowMoveDir = 1;
//        }
//        else if (_moveVec.x <= -0.1f)
//        {
//            _nowMoveDir = -1;
//        }
//        else
//        {
//            _nowMoveDir = 0;
//        }

//        SendData();
//    }

//    public void OnJump()
//    {
//        if (currentJumpCount > 0)
//        {
//            _isJumping = true;
//            _rbCompo.linearVelocityY = jumpForce;
//            currentJumpCount--;

//            SendData();
//        }
//    }

//    private void GroundDash()
//    {
//        if (/*!_isRunning*/ && _isGrounded)
//        {
//            _rbCompo.AddForce(new Vector2(_dashDirection.x, 0) * dashForce, ForceMode2D.Impulse);
//            _RunningTimer -= Time.fixedDeltaTime;
//            if (_RunningTimer <= 0f)
//            {
//                //_isRunning = false;
//            }

//            SendData();
//            return;
//        }
//    }

//    private void AirDash()
//    {
//        if (!_isRunning && !_isGrounded)
//        {
//            _rbCompo.linearVelocity = _dashDirection * dashForce / 2f;
//            _RunningTimer -= Time.fixedDeltaTime;
//            if (_RunningTimer <= 0f)
//            {
//                _isRunning = false;
//            }

//            SendData();
//            return;
//        }
//    }

//    public void OnDash(InputValue value)
//    {
//        if (currentJumpCount <= 0) return;

//        if (!_isDashing)
//        {
//            if (_isGrounded)
//            {
//                currentJumpCount--;
//                _rbCompo.linearVelocityX = 0;
//                _isDashing = true;
//            }
//            else
//            {
//                //_isDashing = false;
//                _rbCompo.linearVelocity = Vector2.zero;
//            }
//            if (_isRunning) return;

//            Vector2 inputDir = _moveVec.normalized;

//            if (_isGrounded)
//            {
//                if (inputDir == Vector2.zero)
//                    inputDir = Vector2.down;
//            }
//            else
//            {
//                inputDir = new Vector2(Mathf.Sign(_moveVec.x), 0);
//            }

//            _dashDirection = inputDir.normalized;
//            _isRunning = true;
//            _RunningTimer = dashDuration;

//            SendData();
//        }
//    }
//    public void SendData()
//    {
//        byte[] bff = ServerManager.Instance.SerializationPlayerMovementData(_nowMoveDir, _isJumping, _isDashing, _isRunning);
//        ServerManager.Instance.SnedData(bff);
//    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
//    }
//#endif
//}