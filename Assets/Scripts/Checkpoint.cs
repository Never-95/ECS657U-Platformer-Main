using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //respawn position
    public Vector3 pos; 

    void Start()
    {
        pos = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //checks if player object is touching checkpoint and updates it to that one within the PlayerController script
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().CheckCurrentCheckpoint(pos);
        }

    }

    void OnCollisionExit(Collision collision)
    {

    }
}
