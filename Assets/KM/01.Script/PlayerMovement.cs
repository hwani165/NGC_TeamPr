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
    private bool CanDash = true;
    private void FixedUpdate()
    {
        OnGround();
        GroundDash();
        if (!_isDashing)
        {
            GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }
        AirDash();
        if (!_isDashing)
        {
            Vector2 velocity = _rbCompo.linearVelocity;
            velocity.x = _moveVec.x * speed;
            _rbCompo.linearVelocityX = velocity.x;
        }
        if (Keyboard.current.sKey.wasPressedThisFrame && !_isDashing && !_isGrounded)
            {
                _rbCompo.AddForce(Vector2.down * gravity * 2f, ForceMode2D.Impulse);
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
        if (_isGrounded)
        {
            _rbCompo.linearVelocityY = jumpForce;
            CanDash = true;
        }
    }
    private void GroundDash()
    {
        if (_isDashing && _isGrounded)
        {
            _rbCompo.AddForce(new Vector2(_dashDirection.x,0) * dashForce,ForceMode2D.Impulse);
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
            _rbCompo.linearVelocity = _dashDirection * dashForce / 1.25f;
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
            GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
            return;
        }
    }

    public void OnDash(InputValue value)
    {
        if (CanDash)
        {
            if (!_isGrounded)
            {
                //무적 판정 매서드 꼭 넣기
                _rbCompo.linearVelocityX = 0;
                CanDash = false;
            }
            else
            {
                CanDash = true;
                _rbCompo.linearVelocity = Vector2.zero;
            }
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
            Debug.Log(_dashDirection);
            _dashTimer = dashDuration;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckVec, groundCheckVecSize);
    }
#endif
}