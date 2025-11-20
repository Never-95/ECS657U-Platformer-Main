using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 pointA; // Start position
    public Vector3 pointB; // End position
    public float speed = 2f;
    public bool useLocalPosition = true; // Use local or world coordinates
    
    private Vector3 targetPosition;
    private Vector3 startPosition;

    void Start()
    {
        if (useLocalPosition)
        {
            startPosition = transform.localPosition;
            pointA = startPosition;
            pointB = startPosition + pointB; // Add offset to start position
        }
        else
        {
            startPosition = transform.position;
        }
        
        targetPosition = pointB;
    }

    void FixedUpdate()
    {
        // Move platform
        if (useLocalPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.fixedDeltaTime);
            
            // Switch direction when reached target
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                targetPosition = targetPosition == pointA ? pointB : pointA;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                targetPosition = targetPosition == pointA ? pointB : pointA;
            }
        }
    }

    // Make player move with platform
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // Visualize movement path in editor
    void OnDrawGizmos()
    {
        if (useLocalPosition)
        {
            Vector3 start = Application.isPlaying ? pointA : transform.localPosition;
            Vector3 end = Application.isPlaying ? pointB : transform.localPosition + pointB;
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.parent ? transform.parent.TransformPoint(start) : start, 
                           transform.parent ? transform.parent.TransformPoint(end) : end);
            Gizmos.DrawWireSphere(transform.parent ? transform.parent.TransformPoint(start) : start, 0.3f);
            Gizmos.DrawWireSphere(transform.parent ? transform.parent.TransformPoint(end) : end, 0.3f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawWireSphere(pointA, 0.3f);
            Gizmos.DrawWireSphere(pointB, 0.3f);
        }
    }
}