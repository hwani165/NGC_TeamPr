using BackEnd;
using BackEnd.Quobject.EngineIoClientDotNet.Parser;
using BackEnd.Tcp;
using Google.FlatBuffers;
using MyGame.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.GPUSort;

public class ClientPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugUI;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 4f; //12
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private float dashForce = 5f; //20
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D _rbCompo;
    private Vector2 _moveVec;
    private bool _isGrounded;
    private bool _isDashing;

    private Vector2 _dashDirection;
    private bool CanDash = true;

    //network
    const byte FLAG_GROUNDED = 1 << 0; // 0000 0001 (1)
    const byte FLAG_DASHING = 1 << 1; // 0000 0010 (2)
    const byte FLAG_CANDASH = 1 << 2; // 0000 0100 (4)

    inputPlayerMovement movement;
    byte[] data;
    Google.FlatBuffers.ByteBuffer buffer;

    string nickName = "null";
    long latency;

    private void Start()
    { 
        Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) =>
        {
            nickName = args.From.NickName;

            data = args.BinaryUserData;
            buffer = new Google.FlatBuffers.ByteBuffer(data);
            movement = inputPlayerMovement.GetRootAsinputPlayerMovement(buffer);
        };

        _rbCompo = GetComponent<Rigidbody2D>();
        _rbCompo.gravityScale = 1f;
    }
    private void Update()
    {
        if (movement.ByteBuffer != null)
        {
            byte stateData = movement.MovementState;
            var vecData = movement.MoveVec.Value;

            _moveVec.x = vecData.X;
            _moveVec.y = vecData.Y;

            CanDash = (stateData & FLAG_CANDASH) != 0;
            _isGrounded = (stateData & FLAG_GROUNDED) != 0;
            _isDashing = (stateData & FLAG_DASHING) != 0;
        }

        //점프 처리
        _rbCompo.linearVelocityY = _moveVec.y * jumpForce;
        GroundDash();
        AirDash();
        if (!_isDashing)
        {
             Vector2 velocity = _rbCompo.linearVelocity;
             velocity.x = _moveVec.x * speed;
             _rbCompo.linearVelocityX = velocity.x;
         }
         _rbCompo.AddForce(Vector2.down * gravity * 2f, ForceMode2D.Impulse);
         //debugUI.text = nickName;
        
    }

    public void OnJump()
    {
        _rbCompo.linearVelocityY = jumpForce;
    }

    private void GroundDash()
    {
        if (_isDashing && _isGrounded)
        {
            _rbCompo.AddForce(new Vector2(_dashDirection.x, 0) * dashForce, ForceMode2D.Impulse);
        }
    }

    private void AirDash()
    {
        if (_isDashing && !_isGrounded)
        {
            _rbCompo.linearVelocity = _dashDirection * dashForce / 2f;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            return;
        }
    }

    public void OnDash(InputValue value)
    {
        if (CanDash)
        {
            if (!_isGrounded)
            {
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
        }
    }
}
