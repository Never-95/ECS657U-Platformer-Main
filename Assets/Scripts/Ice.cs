using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //checks if player object is touching the ice and applies the icy effect
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().icy = true;
        }

    }

    void OnCollisionExit(Collision collision)
    {
        //removes ice effect off player when they stop touching it
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().icy = false;
        }
    }
}
