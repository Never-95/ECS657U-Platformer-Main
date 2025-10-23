using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float moveSpeed;
    public bool isGrounded = true;
    public bool jumping = false;

    //used for accelerating and slowing down while on ice
    private Vector2 velocity = new Vector2(0f, 0f);

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

    [Header("Other Settings")]
    //Contains current checkpoint respawn position
    private Vector3 checkpointpos = new Vector3(0f, 0f, 0f);

    public bool icy = false;
    public float iceaccel = 0.02f;
    public float icedecel = 0.02f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;  // Make sure this is on
        moveSpeed = baseMoveSpeed;
    }

    void FixedUpdate()
    {
        Vector3 move;
        if (icy == true)
        {
            //add inputs to velocity vectors to act as acceleration/deceleration if trying to move in that direction
            if (moveInput.x != 0f)
            {
                velocity.x += (moveInput.x * iceaccel);
                //cap velocity vectors if neccessary
                if (Math.Abs(velocity.x) > 1f)
                {
                    velocity.x -= (moveInput.x * iceaccel);
                }
            }
            //applies deceleration from not moving in that axis
            else if(velocity.x > icedecel)
            {
                Debug.Log("decel");
                velocity.x -= icedecel;
            }
            else if(velocity.x< (-icedecel))
            {
                velocity.x += icedecel;
            }
            else
            {
                velocity.x = 0f;
            }

            //same for y axis
            if (moveInput.y != 0f)
            {
                velocity.y += (moveInput.y * iceaccel);
                if (Math.Abs(velocity.y) > 1f)
                {
                    velocity.y -= (moveInput.y * iceaccel);
                }

            }
            else if (velocity.y > icedecel)
            {
                velocity.y -= icedecel;
            }
            else if (velocity.y < (-icedecel))
            {
                velocity.y += icedecel;
            }
            else
            {
                velocity.y = 0f;
            }

            //applies the velocity onto move vector
            move = transform.right * velocity.x + transform.forward * velocity.y;
        }
        else 
        {
            velocity = moveInput;
            move = transform.right * moveInput.x + transform.forward * moveInput.y;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
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

    void OnRespawn(InputValue value)
    {
        if (value.isPressed)
        {
            transform.position = checkpointpos;
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

