using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float movespeed = 8f;
    [SerializeField] private float jumppower = 13f;
    [SerializeField] private int MaxjumpCount = 0;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckerSize;
    [SerializeField] public bool CanJump { get; set; } = true;
    [SerializeField] public bool CanMove { get; set; } = true;
    [SerializeField] public bool isGround { get; protected set; }
    [SerializeField] public bool IsJump { get; protected set; }
    [SerializeField] public bool IsRun { get; protected set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        isGround = CheckGround();
        if (isGround)
        {
            jumpCount = MaxjumpCount;
            IsJump = false;
        }
        else
        {
            IsJump = true;
        }

        float targetSpeed = 0f;
        float acceleration = 30f;
        float groundDeceleration = 15f;
        float airDeceleration = 7.5f;
        if (CanMove)
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                targetSpeed = -movespeed;
            }
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                targetSpeed = movespeed;
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // 둘다 안 누르면 멈춤
        {
            targetSpeed = 0f;
        }

        float deceleration = IsJump ? airDeceleration : groundDeceleration;

        if (targetSpeed != 0f)
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0f, deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(rb.linearVelocityX) < 0.1f && targetSpeed == 0f)
        {
            rb.linearVelocityX = 0f;
        }

        // 👉 W 키로 점프
        if (Input.GetKey(KeyCode.W))
        {
            if (isGround && CanJump)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.up * jumppower * rb.mass, ForceMode2D.Impulse);
            }
            else
            {
                if (jumpCount > 0 && CanJump)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(Vector2.up * jumppower * rb.mass, ForceMode2D.Impulse);
                    jumpCount--;
                }
                else
                {
                    return;
                }
            }
        }

        // 👉 S 키를 특별한 동작(빠른 낙하 같은)으로 쓰고 싶으면 여기 추가 가능
    }

    public bool CheckGround()
    {
        Collider2D collider = Physics2D.OverlapBox(groundChecker.position, groundCheckerSize, 0, groundLayer);
        return collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundChecker.position, groundCheckerSize);
    }

    public void ChangeJumpActive(bool how)
    {
        CanJump = how;
    }

    public void ChangeMoveActive(bool how)
    {
        CanMove = how;
    }
}
