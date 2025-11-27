using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedAI : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform player;
    public float shootingRange = 10f;
    public float shootingInterval = 2f;
    private float shootingTimer;
    private bool canShoot = true;


    // Start is called before the first frame update
    void Start()
    {
        shootingTimer = shootingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootingRange && canShoot)
        {
            ShootAtPlayer();
            canShoot = false;
            shootingTimer = shootingInterval;
        }

        if (!canShoot)
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer <= 0f)
            {
                canShoot = true;
            }
        }
    }
    void ShootAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(direction));
        transform.LookAt(player);
        // Optionally add force to the projectile if it has a Rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float projectileSpeed = 15f;
            rb.velocity = direction * projectileSpeed;
            
        }
    }

}
