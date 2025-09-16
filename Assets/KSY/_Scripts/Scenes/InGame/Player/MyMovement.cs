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
    public bool _usingJump = false;
    public bool _usingDownDash = false;
    public sbyte _moveX = 0;

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
        OnGround();
        Vector2 velocity = _rbCompo.linearVelocity;
        velocity.x = _moveVec.x * speed;
        _rbCompo.linearVelocityX = velocity.x;
    }

    private void Update()
    {
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
    public void Serialize()
    {
        float x = (float)System.Math.Round(transform.position.x, 3);
        float y = (float)System.Math.Round(transform.position.y, 3);

        _pos = new Vector2(x, y);

        if (Server.Instance == null) return;
        posBff = Server.Instance.SerializationPlayerPos(_pos);
    }

    public override void Send()
    {
        movementBff = Server.Instance.SerializationPlayerMovementData(_moveX, _usingJump, _usingDownDash);
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