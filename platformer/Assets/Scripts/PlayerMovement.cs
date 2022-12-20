using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Btw kato 2d Movement in unity (Tutorial) Brackeyssin kanavalt jos haluut teh� ihan niinku omal spritell�

    //public GameObject Player; 

    //public player_health ph;
    public bool isntinvincible = true;
    public float speed = 8f;
    public float jumpingPower = 16f;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private bool isFacingRight = true;
    private float horizontal;

    /*private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    private float wallJumpingDirection;
    private float wallJumpingCounter;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(12f, 20f);
    public bool wallJumpEnabled = false;*/
    private bool isWallJumping;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBuffertime = 0.2f;
    private float jumpBuffetCounter;
    private bool isJumping;



    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public bool dashUnlocked = false;

    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform wallCheck;
    [Header("Knockback")]
    [SerializeField] private Transform center;
    [SerializeField] private float knockbackVel = 8f;
    [SerializeField] public bool knockbacked = false;
    [SerializeField] private float knockbackedTime = 1f;
      
    public Animator animator;

    // Update is called once per frame

    void Start()
    {


    }
    void Update()
    {        
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        horizontal = Input.GetAxisRaw("Horizontal");
        if(isGrounded)
        {
            //animator.SetBool("isJumping", false);
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            //animator.SetBool("isJumping", true);
            coyoteTimeCounter -= Time.deltaTime;        
        }
        if(Input.GetButtonDown("Jump"))
        {
            jumpBuffetCounter = jumpBuffertime;
        }
        else
        {
            jumpBuffetCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBuffetCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBuffetCounter = 0f;

            StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        if (isDashing)
        {
            return;
        }
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && dashUnlocked)
        {
            StartCoroutine(Dash());
        }
        //WallSlide();
        //WallJump();
        if(!isWallJumping)
        {
            Flip();
        }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            gameObject.transform.SetParent(null);
        }


    }
    void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }
        //Move our character
        //Controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        if(!knockbacked)
        {
            if (!isWallJumping)
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
        }  
        else
        {
            var lerpedXVelocity = Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 3);
            rb.velocity = new Vector2(lerpedXVelocity, rb.velocity.y);
        }    

    }
    /*private bool isWalled()
    {

        
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, whatIsGround);

    }
    private void WallSlide()
    {
        if(isWalled() && !isGrounded && horizontal != 0f && wallJumpEnabled)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            if(wallJumpEnabled)
            {
                isWallSliding = false;
            }

        }
    }
    private void WallJump()
    {
        if(isWallSliding && wallJumpEnabled)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            if(wallJumpEnabled)
            {
                wallJumpingCounter -= Time.deltaTime;
            }
        }
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f && wallJumpEnabled)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if(transform.localScale.x != wallJumpingDirection)
            {
                Vector3 localScale = transform.localScale;
                isFacingRight = !isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    private void StopWallJumping()
    {
        isWallJumping = false;
    }*/
    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
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
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.3f);
        isJumping = false;
    }
    public void UnlockDash()
    {
        dashUnlocked = true;
    }

    public void Knockback(Transform t)
    {   
        if (isntinvincible)
        {
            var dir = transform.position - t.position;
            knockbacked = true;
            rb.velocity = dir.normalized * knockbackVel;
            StartCoroutine(Unknockback());   
        }
     
    }
    private IEnumerator Unknockback()
    {
        yield return new WaitForSeconds(knockbackedTime);
        knockbacked = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
    public void isInvurnelable()
    {
        isntinvincible = false;
    }
    public void isntInvurnelable()
    {
        isntinvincible = true;
    }
}