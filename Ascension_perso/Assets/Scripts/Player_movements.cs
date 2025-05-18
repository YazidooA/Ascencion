using System.Collections;
using UnityEngine;
using System;

public class Player_movements : MonoBehaviour
{
    private float horizontal;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpingPower = 12f;
    private bool isFacingRight = true;

    private bool allowDash => savedDatas.allowDash;
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower = 6f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private storeData savedDatas;

    private bool allowWallSticking => savedDatas.allowWallSticking;
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;


    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(20f, 12f);

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask iceLayer;

    [SerializeField] private Animator animator;

    void Update()
    {
        animator.SetFloat("Speed", Math.Abs(horizontal));
        animator.SetBool("Jump", !IsGrounded() && !IsWalled() && !IsOnIce());
        animator.SetBool("Walled", IsWalled() || IsOnIce());

        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            if (allowDash)
            {
                StartCoroutine(Dash());
            }
        }

        WallSlide();
        if (allowWallSticking)
        {
            WallJump();
        }
        else
        {
            if (!IsOnIce())
            {
                WallJump();
            }
        }

        if (!isWallJumping)
        {
            Flip();
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private bool IsOnIce()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, iceLayer);
    }

    private void WallSlide()
    {
        if (IsOnIce() && !IsGrounded() && horizontal != 0f && !allowWallSticking)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -4f);
        }
        else if (IsWalled() && !IsGrounded() && horizontal != 0f && !allowWallSticking)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else if (IsOnIce() && !IsGrounded() && allowWallSticking)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        else if (IsWalled() && !IsGrounded() && allowWallSticking)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
