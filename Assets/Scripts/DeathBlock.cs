using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //checks if player object is touching death block and calls method to play death sequence and respawn etc.
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Death();
        }

    }

    void OnCollisionExit(Collision collision)
    {

    }
}