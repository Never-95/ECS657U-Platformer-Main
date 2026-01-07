using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    //default movement speed that is adjustable
    public float baseMoveSpeed = 5f;
    //current movement speed that can be modified by coins etc. (e.g. speed boost)
    private float moveSpeed;
    public bool isGrounded = true;
    public bool jumping = false;
    public float baseAcceleration = 5f;

    //used for accelerating and slowing down
    public Vector2 velocity = new Vector2(0f, 0f);

    private Vector2 moveInput;
    private Rigidbody rb;

    [Header("Perk Settings")]
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    private bool speedBoostActive = false;
    private float speedBoostTimer = 0f;
    private bool isInvisible = false;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float doubleJumpMultiplier = 1.5f;

    [Header("Other Settings")]
    //Contains current checkpoint respawn position
    private Vector3 checkpointpos = new Vector3(0f, 0f, 0f);

    public bool icy = false;
    public float iceAccel = 0.5f;

    //animator
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        moveSpeed = baseMoveSpeed;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        //sets acceleration and movespeed to ice or normal based on icy status
        float accel = baseAcceleration * Time.fixedDeltaTime;
        //move speed used in calculations based off moveSpeed (which can be increased)
        float currentMoveSpeed;

        //change acceleration and movespeed if on ice
        if (icy){
            accel = iceAccel * Time.fixedDeltaTime;
            currentMoveSpeed = moveSpeed * 2f;
        }
        else{
            currentMoveSpeed = moveSpeed;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        animator.SetBool("isGrounded", isGrounded);

        //make acceleration less while in air (to give less control)
        if (!isGrounded){
            accel = accel / 2f;
        }

        //calculate input direction vector for xz plane (horizontal movement)
        Vector3 inputDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        inputDir.y = 0f;

        /*/calculate if magnitude is greater than 1 (which will work for any direction it could be) and normalize (reduce to magnitude of 1)
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();
        */

        Vector3 targetVelocity = inputDir * currentMoveSpeed;
        Vector3 currentVelocity = rb.velocity;

        //take current horizontal velocity (xz plane) only to use for horizontal movement calculation
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        //smooth acceleration from current horizontal velocity to target velocity
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accel);

        //apply final velocity now including unchanged vertical velocity (y component)
        rb.velocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);


        if (isGrounded && jumping){
            //nullify any existing vertical velocity before adding jump force
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumping = false;
            isGrounded = false;
            animator.SetBool("Jumping", false);
            animator.SetBool("isGrounded", false);
            hasDoubleJumped = false;
        }
        else if (canDoubleJump && jumping && !hasDoubleJumped){
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float doubleJumpForce = jumpForce * doubleJumpMultiplier;
            rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            hasDoubleJumped = true;
            jumping = false;
            animator.SetBool("Jumping", false);
        }

        if (speedBoostActive){
            //decrease speed boost timer
            speedBoostTimer -= Time.fixedDeltaTime;

            //if timer runs out, reset move speed back to base value and deactivate speed boost
            if (speedBoostTimer <= 0f){
                moveSpeed = baseMoveSpeed;
                speedBoostActive = false;
            }
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        animator.SetFloat("ForwardMovement", moveInput.y);
        animator.SetFloat("StrafingMovement", moveInput.x);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumping = true;
            animator.SetBool("Jumping", true);
        }
    }

    void OnRespawn(InputValue value)
    {
        if (value.isPressed)
        {
            transform.position = checkpointpos;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        // Check if we're colliding with walls/sides
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) < 0.5f) // Not ground
            {
                // Apply slight repulsion force or adjust position
                Vector3 adjustment = contact.normal * 0.1f;
                rb.position += adjustment;
                break;
            }
        }
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

    public void ActivateInvisibility(float duration)
    {
        StartCoroutine(InvisibilityRoutine(duration));
    }

    private System.Collections.IEnumerator InvisibilityRoutine(float duration)
    {
        isInvisible = true;
        yield return new WaitForSeconds(duration);
        isInvisible = false;
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }

    public void CheckCurrentCheckpoint(Vector3 newcheckpointpos)
    {
        if (newcheckpointpos != checkpointpos)
        {
            checkpointpos = newcheckpointpos;
        }
    }

    public void Death()
    {
        //respawns player
        transform.position = checkpointpos;
    }
}

