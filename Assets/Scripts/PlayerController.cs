using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;

    public float jumpforce = 5f;
    public bool isGrounded=true;
    public bool Jumping = false;
    public InputValue inputValue;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        // Move
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);

        if (isGrounded && Jumping)
        {
            rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            Jumping = false;
            isGrounded = false;
        }
    }
    void OnJump(InputValue inputValue)
    {
        if(inputValue.isPressed && isGrounded)
        {
            Jumping = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move Input: " + moveInput);
    }

}
