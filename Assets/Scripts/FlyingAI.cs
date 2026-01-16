using System.Collections;
using UnityEngine;

public class scr : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private float attackRange = 2f;
    private float detectionRange = 10f;

    private Vector3 startPosition;
    private Vector3 randomPatrolTarget;
    public float patrolRadius = 5f;

    public float liftHeight = 10f;
    public float liftSpeed = 2f;
    private float dropDelay = 2f;

    private bool isAttacking = false;
    private bool isLifting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        PickNewRandomPoint(startPosition);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isAttacking || isLifting)
            return;

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void PickNewRandomPoint(Vector3 referencePosition)
    {
        Vector3 rnd = Random.insideUnitSphere * patrolRadius;
        randomPatrolTarget = referencePosition + rnd;
    }

    private void Patrol()
    {
        Vector3 direction = randomPatrolTarget - transform.position;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, randomPatrolTarget) < 1f)
        {
            PickNewRandomPoint(startPosition);
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Damage once
        player.GetComponent<PlayerHealth>().TakeDamage(20);

        // Attach player to enemy
        player.SetParent(transform);

        Vector3 targetLiftPos = transform.position + Vector3.up * liftHeight;
        isLifting = true;

        // Smooth lift
        while (Vector3.Distance(transform.position, targetLiftPos) > 0.1f)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.zero; // Prevent player from falling during lift
            transform.position = Vector3.MoveTowards(transform.position, targetLiftPos, liftSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(dropDelay);

        // Drop player
        player.SetParent(null);

        isLifting = false;
        isAttacking = false;
    }
}
