using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    struct BouncePadTarget
    {
        public float ContactTime;
    }
    
    float LaunchDelay = 0.1f;
    public float LaunchForce = 10f;
    ForceMode LaunchMode = ForceMode.Impulse;
    
    [Header("Audio")]
    public AudioClip bounceSound;
    public float bounceVolume = 0.8f;
    
    Dictionary<Rigidbody, BouncePadTarget> Targets = new Dictionary<Rigidbody, BouncePadTarget>();
    List<Rigidbody> ClearTargets = new List<Rigidbody>();
    
    private void FixedUpdate()
    {
        //check for targets to bounce
        float thresholdTime = Time.timeSinceLevelLoad - LaunchDelay;
        foreach(var launchtarget in Targets)
        {
            if (launchtarget.Value.ContactTime >= thresholdTime)
            {
                Launch(launchtarget.Key);
                ClearTargets.Add(launchtarget.Key);
            }
        }
        
        foreach(var target in ClearTargets)
        {
            Targets.Remove(target);
        }
        ClearTargets.Clear();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        //Retrieve Rigidbody and add to target list
        Rigidbody rb;
        if (collision.gameObject.TryGetComponent<Rigidbody>(out rb))
        {
            Targets[rb] = new BouncePadTarget() {ContactTime = Time.timeSinceLevelLoad};
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
    }
    
    void Launch(Rigidbody target)
    {
        target.AddForce(transform.up * LaunchForce, ForceMode.Impulse);
        
        // Play bounce sound
        if (bounceSound != null)
        {
            AudioSource.PlayClipAtPoint(bounceSound, transform.position, bounceVolume);
        }
    }
}
