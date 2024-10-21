using System.Collections;
using UnityEngine;

// 코요테 타임 + collision으로 grounded 체크

public class PlayerMovement : MonoBehaviour
{
    public bool isPlaying = true;
    private float horizontalInput;
    private float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    public bool isGrounded = false;

    public bool isJumping;

    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    // Jump가 눌렸을 때, coyoteTime - coyoteTimeCounter = (땅에서 떨어지고 점프를 누를 때까지의 시간)

    private Rigidbody2D rb;

    public int death = 0;
    public bool isDead = false;

    public void InitDeath()
    {
        death = 0;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (isPlaying)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (coyoteTime > 0)
            {
                CoyoteJump();
            }
            else
            {
                NormalJump();
            }
        }
        else
        {
            // isPlaying이 아닐 때는 input값이 없도록,
            horizontalInput = 0;
        }
        Flip();
    }

    private void NormalJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void CoyoteJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteTimeCounter > 0f && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                StartCoroutine(JumpCooldown());
                // Debug.Log("점프 성공");
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }


    private void OnCollisionEnter2D(Collision2D Other)
    {
        if (Other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D Other)
    {
        if (Other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TurnOnPlay()
    {
        isPlaying = true;
    }

    public void TurnOffPlay()
    {
        isPlaying = false;
        rb.velocity = Vector2.zero;
    }
}