using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBehaviour : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    
    private bool isChasing = false;
    private bool isAttacking = false;
    
    public float chaseRange = 10f;
    private float attackRange = 2f;
    
    // Store starting position to return to
    private Vector3 startPosition;

    private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    public float normalSpeed = 3.5f;
    public float retreatSpeed = 6f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Store the enemy's starting position
        startPosition = transform.position;
        agent.speed = normalSpeed;
    }

    void Update()
    {
        // Check if player is invisible
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsInvisible())
        {
            // Return to start position when player is invisible
            ReturnToStart();
            isChasing = false;
            isAttacking = false;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Chase logic
        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            Debug.Log("Chasing the player");
            isChasing = true;
            isAttacking = false;
            agent.speed = normalSpeed;
            OnChase();
        }
        // Attack logic
        else if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Attacking the player");
            isChasing = false;
            isAttacking = true;
            OnAttack();
        }
        // Return to start if player is too far
        else
        {
            Debug.Log("Returning to start position");
            isChasing = false;
            isAttacking = false;
            ReturnToStart();
        }
    }

    void OnChase()
    {
        agent.isStopped = false;
        agent.speed = normalSpeed;
        agent.SetDestination(player.transform.position);
    }

    void OnAttack()
    {
        // Stop moving when attacking
        agent.isStopped = true;
        
        // Look at player
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0; // Keep on same vertical plane
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Deal damage with cooldown
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            playerHealth.TakeDamage(20f);
            
            // Knockback
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDirection = (player.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * 30f, ForceMode.Impulse);
            }
        }
    }

    void ReturnToStart()
    {
        agent.isStopped = false;
        
        // Only move back if not already at start position
        if (Vector3.Distance(transform.position, startPosition) > 0.5f)
        {
            agent.SetDestination(startPosition);
        }
        else
        {
            agent.isStopped = true; // Stop when reached start position
            agent.speed = normalSpeed;
        }
    }
}
