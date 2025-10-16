using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public float Xaxis = 2f;   // Mouse sensitivity (horizontal)
    public float Yaxis = 2f;   // Mouse sensitivity (vertical)

    private float yRotation = 0f; // Horizontal rotation (player)
    private float xRotation = 0f; // Vertical rotation (camera)

    public Transform target; // The player body or object the camera follows

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Place the camera at the target's position initially
        transform.position = target.position;
    }

    void Update()
    {
        // --- Mouse input ---
        float mouseX = Input.GetAxis("Mouse X") * Xaxis;
        float mouseY = Input.GetAxis("Mouse Y") * Yaxis;
        yRotation += mouseX;
        target.rotation = Quaternion.Euler(0f, yRotation, 0f); // Rotate the body left/right
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical look
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // --- Keep camera on player position ---
        transform.position = target.position;
    }
}
 