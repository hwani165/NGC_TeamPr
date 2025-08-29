using UnityEngine;
using UnityEngine.InputSystem;
using static BackendFunctionInGame;

public class OtherMovement : MonoBehaviour, IReceivable
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private Vector2 groundCheckVecSize;
    [SerializeField] private Vector2 groundCheckVec;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float dashForce = 20f;

    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec;

    #region NetWorkData
    //점프를 했는가? (Is Jumping Now? <bool>)
    private bool _isGrounded;
    //대쉬를 하고 있는가?(Is Dashing Now? <bool>)
    private bool _isDashing;
    //대쉬할 방향(Dash Direction<Vec2>)
    private Vector2 _dashDirection;
    //대쉬를 사용했는가? (Use Dash? <bool>)
    private bool CanDash = false;
    //이동하고 있는 방향 (Now Move.X Direction <Sbyte>)
    private sbyte nowMoveDirection;
    #endregion
    private void Start()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;

        //클라이언트가 SendDataToInGameRoom 함수로 서버로 보낸 메시지를 게임방에 접속한 모든 클라이언트에게
        //브로드캐스팅 했을 때 호출되는 이벤트입니다.
    }
    private void Update()
    {
        Jump();
        OnGround();
        GroundDash();
        if (!_isDashing)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
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
        _isGrounded = hit == null;

        if (!_isGrounded)
        {
            CanDash = false;
        }
    }
    private void Jump()
    {
        if(!_isGrounded)
        _rbCompo.linearVelocityY = jumpForce;
    }
    private void GroundDash()
    {
        if (_isDashing && !_isGrounded)
        {
            _rbCompo.AddForce(new Vector2(_dashDirection.x, 0) * dashForce, ForceMode2D.Impulse);
            _isDashing = false;
            
            return;
        }
    }
    private void AirDash()
    {
        if (_isDashing && _isGrounded)
        {
            _rbCompo.linearVelocity = _dashDirection * dashForce / 2f;
            _isDashing = false;
            
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            return;
        }
    }
    public void OnDash(InputValue value)
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

            _dashDirection = inputDir.normalized;
            _isDashing = true;
        }
    }
    public void ApplyData(byte byteData)
    {
        Debug.Log($"byteData : {byteData}");

        //대쉬를 했는가?
        bool isDash = (byteData & (byte)flagPlayerMovementState.IsDashing) != 0;

        //대쉬하고 있는가?
        bool isRunning = (byteData & (byte)flagPlayerMovementState.IsRunning) != 0;

        //점프를 하고 있는가?
        bool isJumping = (byteData & (byte)flagPlayerMovementState.IsJumping) != 0;
    }
    public void ApplyData(sbyte sbyteData)
    {
        Debug.Log($"moveDir : {sbyteData}");

        //전달받은 이동방향을 적용
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