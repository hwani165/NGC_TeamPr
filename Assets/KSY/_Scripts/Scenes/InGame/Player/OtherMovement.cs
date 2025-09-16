using UnityEngine;
using static BackendFunctionInGame;

public class OtherMovement : Player
{
    [SerializeField] private MovementDataSO _movementData;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private Vector2 groundCheckVecSize = new Vector2(0.5f, 1.05f);
    [SerializeField] private Vector2 groundCheckVec;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D _rbCompo;
    public Vector2 _moveVec;

    private bool _isGrounded;
    #region NetWorkData
    public bool _usingJump = false;
    public bool _downDashing = false;
    #endregion

    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
        groundMask = LayerMask.GetMask("Ground");
        groundCheckVecSize = new Vector2(0.5f, 1.05f);
    }
    private void OnValidate()
    {
        speed = _movementData.Speed;
        jumpForce = _movementData.JumpForce;
        gravity = _movementData.Gravity;

        dashForce = _movementData.DashForce;
        dashDuration = _movementData.DashDuration;
    }
    private void FixedUpdate()
    {
        Vector2 velocity = _rbCompo.linearVelocity;
        velocity.x = _moveVec.x * speed;
        _rbCompo.linearVelocityX = velocity.x;
    }
    private void Update()
    {
        OnGround();


    }
    private void DownDash()
    {
        if(!_isGrounded)
        {
            Debug.Log($"_isGrounded : {_isGrounded}");
            _rbCompo.AddForce(Vector2.down * gravity * 1.5f, ForceMode2D.Impulse);
        }
    }
    private void OnGround()
    {
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckVec, groundCheckVecSize, 0, groundMask);
        _isGrounded = hit != null;

        if (_isGrounded)
        {
            _downDashing = false;
            _usingJump = false;
        }
    }
    public void OnJump()
    {
        _usingJump = true;
        _rbCompo.linearVelocityY = jumpForce;
    }
    public override void ApplyByteData(byte state)
    {
        bool UsingJump = (state & (byte)flagPlayerMovementState.UsingJump) != 0;
        if (UsingJump)
        {
            //Debug.Log($"isjumping : {UsingJump}");
            OnJump();
        }

        bool usingDownDash = (state & (byte)flagPlayerMovementState.UsingDownDash) != 0;

        if (!_downDashing && usingDownDash)
        {
            _downDashing = usingDownDash;
            DownDash();
        }
    }
    public override void ApplyPosData(float x, float y)
    {
        _rbCompo.MovePosition(new Vector3(x, y));
    }
    public override void ApplySbyteData(sbyte moveX, sbyte dashX, sbyte dashY)
    {
        float _moveX = moveX;
        _moveVec.x = _moveX;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}