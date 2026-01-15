using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;
    public bool useLocalPosition = true;

    private Vector3 targetPosition;
    private Vector3 startPosition;

    private Vector3 lastPlatformPos;

    void Start()
    {
        if (useLocalPosition)
        {
            startPosition = transform.localPosition;
            pointA = startPosition;
            pointB = startPosition + pointB;
            lastPlatformPos = transform.localPosition;
        }
        else
        {
            startPosition = transform.position;
            lastPlatformPos = transform.position;
        }

        targetPosition = pointB;
    }

    void FixedUpdate()
    {
        // Store platform pos BEFORE moving
        Vector3 before = useLocalPosition ? transform.localPosition : transform.position;

        // Move platform
        if (useLocalPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
                targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }

        // Compute platform delta this frame
        Vector3 after = useLocalPosition ? transform.localPosition : transform.position;
        Vector3 platformDelta = after - before;

        // If a player is on it, add delta to player
        if (_playerRb != null)
        {
            _playerRb.MovePosition(_playerRb.position + platformDelta);
        }
    }

    private Rigidbody _playerRb;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _playerRb = collision.rigidbody; // grab rigidbody
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _playerRb = null;
    }
}
