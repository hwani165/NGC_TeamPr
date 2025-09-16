using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class MyMovement : Player
{
    [SerializeField] private MovementDataSO _movementData;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private Vector2 groundCheckVecSize = new Vector2(0.5f, 1.05f);
    [SerializeField] private Vector2 groundCheckVec;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.15f;

    [SerializeField] private int maxJumpCount = 2;
    private int _currentJumpCount;

    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec = Vector2.zero;

    private bool _isGrounded;

    #region NetWorkData
    //������ �ߴ°�? (Is Jumping Now? <bool>)
    public bool _usingJump = false;
    //�뽬�� �ϰ� �ִ°�?(Is Dashing Now? <bool>)
    public bool _isDashing = false;
    //�뽬�� ����(Dash Direction<Vec2>)
    public Vector2 _dashDir = Vector2.zero;
    //�뽬�� ����ߴ°�? (Use Dash? <bool>)
    public bool _usingDash = false;
    //������ �뽬�� ����ߴ°�? (Use Down Dash? <bool>)
    public bool _usingDownDash = false;
    //�̵��ϰ� �ִ� ���� (Now Move.X Direction <Sbyte>)
    public sbyte _moveX = 0;
    //�۽� ����
    private byte[] movementBff;
    private byte[] posBff;

    private float _currentTime = 0f;

    private Vector2 _pos;
    #endregion

    private float _dashTimer = 0;

    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
        _currentJumpCount = maxJumpCount;
        groundMask = LayerMask.GetMask("Ground");
        groundCheckVecSize = new Vector2(0.5f, 1.05f);
    }

    private void FixedUpdate()
    {
        //����
        if (!CountDownScript.IsGameStarting) return;
        OnGround();
        GroundDash();
        if (!_isDashing)
        {
            Vector2 velocity = _rbCompo.linearVelocity;
            velocity.x = _moveVec.x * speed;
            _rbCompo.linearVelocityX = velocity.x;
        }
    }

    private void Update()
    {
        //����
        if (!CountDownScript.IsGameStarting) return;
        float x = (float)System.Math.Round(transform.position.x, 3);
        float y = (float)System.Math.Round(transform.position.y, 3);

        _pos = new Vector2(x, y);

        Serialize();

        _currentTime += Time.deltaTime;

        if (Keyboard.current.sKey.wasPressedThisFrame && !_isGrounded)
        {
            _usingDownDash = true;
            _rbCompo.AddForce(Vector2.down * gravity * 1.5f, ForceMode2D.Impulse);
            Send();
        }

        if (_currentTime >= 0.6f)
        {
            _currentTime = 0f;
            Serialize();
            PosSend();
        }
    }
    private void OnValidate()
    {
        speed = _movementData.Speed;
        jumpForce = _movementData.JumpForce;
        gravity = _movementData.Gravity;

        dashForce = _movementData.DashForce;
        dashDuration = _movementData.DashDuration;
    }

    private void OnGround()
    {
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckVec, groundCheckVecSize, 0, groundMask);
        _isGrounded = hit != null;

        if (_isGrounded)
        {
            _currentJumpCount = maxJumpCount;
            _usingJump = false;
            _usingDownDash = false;
            _usingDash = false;
        }
    }
    public void OnMove(InputValue value)
    {
        _moveVec = value.Get<Vector2>();
        if (_moveVec.x >= 0.1f)
        {
            _moveX = 1;
        }
        else if (_moveVec.x <= -0.1f)
        {
            _moveX = -1;
        }
        else
        {
            _moveX = 0;
        }

        Send();
    }
    public void OnJump()
    {
        if (_currentJumpCount > 0)
        {
            _usingJump = true;
            _rbCompo.linearVelocityY = jumpForce;
            _currentJumpCount--;

            Send();
        }
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
    public void OnDash(InputValue value)
    {
        if (_currentJumpCount <= 0) return;

        if (!_usingDash)
        {
            if (_isGrounded)
            {
                _currentJumpCount--;
                _rbCompo.linearVelocityX = 0;
                _usingDash = true;
            }
            else
            {
                _usingDash = false;
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

            Send();

        }
    }
    public void Serialize()
    {
        if (Server.Instance == null) return;
        posBff = Server.Instance.SerializationPlayerPos(_pos);
    }

    public override void Send()
    {
        movementBff = Server.Instance.SerializationPlayerMovementData(_dashDir, _moveX, _usingJump, _usingDash, _isDashing, _usingDownDash);
        Server.Instance.Send(movementBff);
    }
    public void PosSend()
    {
        if (posBff != null)
        {
            Server.Instance.Send(posBff);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}