using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player takes " + damage + " damage.");
        Debug.Log("Player health: " + health);
        if (health <= 0)
        {
            Debug.Log("Player is dead.");
        }
    }
}
