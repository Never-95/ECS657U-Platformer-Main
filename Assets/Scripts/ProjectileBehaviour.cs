using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Projectile collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Projectile hit the player");
                playerHealth.TakeDamage(10); // Assuming a TakeDamage method exists
            }
            Destroy(gameObject); // Destroy the projectile after hitting the player
        }
        else if (!other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject,3f); // Destroy the projectile on any other collision except with enemies
        }
    }
}
