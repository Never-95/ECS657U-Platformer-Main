using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public NavMeshAgent agent;
    private bool isChasing = false;
    private bool isAttacking = false;
    private float chaseRange = 10f;
    private float attackRange = 2f;

    //Patroling
    public float walkpointRange;
    private Vector3 walkPoint;
    private bool walkPointSet;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsInvisible())
        {
            if (!walkPointSet || !isChasing)
            {
                OnPatrol();
            }
            isChasing = false;
            isAttacking = false;
            return; 
        }
        if (!isChasing)
        {
            Debug.Log("Patrolling");
            OnPatrol();
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            isChasing = true;
            isAttacking = false;
            //Debug.Log("Chasing");
            OnChase();
        }
        else if (distanceToPlayer <= attackRange)
        {
            isChasing = false;
            isAttacking = true;
            Debug.Log("Attacking");
            OnAttack();
        }
        else
        {
            isChasing = false;
            isAttacking = false;
        }

    }
    
    void OnPatrol()
    {
        if (!walkPointSet) { SearchWalkPoint(); }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            if (Vector3.Distance(transform.position, walkPoint) < 1f)
                walkPointSet = false;
        }
        void SearchWalkPoint()
        {
            //Calculate random point in range
            float randomZ = Random.Range(-walkpointRange, walkpointRange);
            float randomX = Random.Range(-walkpointRange, walkpointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, LayerMask.GetMask("Ground")))
                walkPointSet = true;
        }
        // Logic for patrolling behavior
    }
    void OnChase()
    {

        agent.SetDestination(player.transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            isChasing = false;
            isAttacking = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > chaseRange)
        {
            isChasing = false;
            isAttacking = false;
        }

        // Logic for chasing the player
    }
    void OnAttack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(20f);
        }
        //player.GetComponent<PlayerHealth>().TakeDamage(1);
        player.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position).normalized * 5f, ForceMode.Impulse);
        // Logic for attacking the player
    }

}
