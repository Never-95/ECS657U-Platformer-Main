using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float moveSpeed;
    public bool isGrounded = true;
    public bool jumping = false;

    //Contains current checkpoint
    private string checkpoint = "";

    private Vector2 moveInput;
    private Rigidbody rb;

    [Header("Perk Settings")]
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;

    private bool speedBoostActive = false;
    private float speedBoostTimer = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float doubleJumpMultiplier = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;  // Make sure this is on
        moveSpeed = baseMoveSpeed;
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        if (isGrounded)
        {
            // Ground movement - use MovePosition
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Air movement - use velocity for horizontal, preserve vertical
            Vector3 airMove = move * moveSpeed * Time.fixedDeltaTime;
            rb.velocity = new Vector3(airMove.x / Time.fixedDeltaTime, rb.velocity.y, airMove.z / Time.fixedDeltaTime);
        }
        
        if (isGrounded && jumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumping = false;
            isGrounded = false;
            hasDoubleJumped = false;
        }
        else if (canDoubleJump && jumping && !hasDoubleJumped)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float doubleJumpForce = jumpForce * doubleJumpMultiplier;
            rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            hasDoubleJumped = true;
            jumping = false;
        }
        
        if (speedBoostActive)
        {
            speedBoostTimer -= Time.fixedDeltaTime;
            if (speedBoostTimer <= 0f)
            {
                moveSpeed = baseMoveSpeed;
                speedBoostActive = false;
            }
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            jumping = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        moveSpeed = baseMoveSpeed * multiplier;
        speedBoostTimer = duration;
        speedBoostActive = true;
    }

    public void EnableDoubleJump(float duration)
    {
        StartCoroutine(DoubleJumpRoutine(duration));
    }

    private System.Collections.IEnumerator DoubleJumpRoutine(float duration)
    {
        canDoubleJump = true;
        yield return new WaitForSeconds(duration);
        canDoubleJump = false;
    }

    public void CheckCurrentCheckpoint(string checkpoint)
    {
        Debug.Log("check current checkpoint");
    }
}

