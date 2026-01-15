using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    private float attackRange = 2f;
    public float rotationSpeed = 5f;
    private float detectionRange = 10f;
    private Transform startPosition;
    private Vector3 randomPatrolTarget;
    public float patrolRadius = 5f;
    
    public float liftHeight = 10f;
    public float liftSpeed = 2f;
    private float currentLiftHeight = 0f;
    private float dropDelay = 2f;
    private bool ispatrolling = true;
    private bool ischasing = false;
    private bool isattacking = false;
    private bool isattacked = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform;
        PickNewRandomPoint(startPosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            ischasing = true;
            ispatrolling = false;
            isattacking = false;
            OnChase();
        }
        else if (distanceToPlayer <= attackRange)
        {
            isattacking = true;
            ischasing = false;
            ispatrolling = false;
            OnAttack();
        }
        else
        {
            ispatrolling = true;
            ischasing = false;
            isattacking = false;
            transform.position = Vector3.MoveTowards(transform.position, startPosition.position, moveSpeed * Time.deltaTime);
            OnFly();
        }
    }
    private void PickNewRandomPoint(Vector3 referencePosition)
    {
        // Random position inside unit sphere, then scaled
        Vector3 rnd = Random.insideUnitSphere * patrolRadius;

        // New point relative to current position
        randomPatrolTarget = referencePosition + rnd;
    }
    private void OnFly()
    {
        Vector3 direction = randomPatrolTarget - transform.position;

        // Move toward the target
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        // Rotate towards movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotationSpeed
            );
        }
        if (Vector3.Distance(transform.position, randomPatrolTarget) < 1f)
        {
            PickNewRandomPoint(startPosition.position);
        }
    }
    private void OnChase()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0; // Keep on same vertical plane
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

    }
    private void OnAttack()
    {
        if (!isattacked)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(20);
            isattacked = true;
        }
        player.SetParent(transform);
        Vector3 liftposition = new Vector3(transform.position.x, transform.position.y + liftHeight, transform.position.z);
        while (currentLiftHeight < liftHeight)
        {
            transform.position = Vector3.MoveTowards(transform.position, liftposition, liftSpeed * Time.deltaTime);
            currentLiftHeight += liftSpeed * Time.deltaTime;
        }
        StartCoroutine(DropPlayerAfterDelay());

    }
    private IEnumerator DropPlayerAfterDelay()
    {
        yield return new WaitForSeconds(dropDelay);
        player.SetParent(null);
        isattacked = false;
        currentLiftHeight = 0f;
    }
}
