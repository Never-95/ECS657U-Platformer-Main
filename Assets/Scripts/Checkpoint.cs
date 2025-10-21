using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
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
        Debug.Log($"{collision.gameObject.name} touched {gameObject.name}");

        if (collision.gameObject.name == "Player")
        {
            Debug.Log($"touched touet");
            collision.gameObject.CheckCurrentCheckpoint("boom");
            //controlscript.CheckCurrentCheckpoint("boom");
        }

    }

    void OnCollisionExit(Collision collision)
    {

    }
}
