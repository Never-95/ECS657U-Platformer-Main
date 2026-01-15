using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;
    
    [Header("Footstep Timing")]
    public float stepDistance = 2f;  // Distance traveled before next step
    
    [Header("Volume")]
    public float footstepVolume = 0.5f;
    
    private PlayerController playerController;
    private Rigidbody rb;
    private Vector3 lastStepPosition;
    private bool wasMoving = false;  // Track if player was moving last frame
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        lastStepPosition = transform.position;
        
        // Create audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }
    
    void Update()
    {
        // Only play footsteps when grounded
        if (playerController != null && playerController.isGrounded)
        {
            // Check if player is actually moving
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            bool isMoving = horizontalVelocity.magnitude > 0.5f;
            
            // Play immediate footstep when starting to move
            if (isMoving && !wasMoving)
            {
                PlayFootstepSound();
                lastStepPosition = transform.position;
            }
            
            // Calculate distance traveled since last step
            if (isMoving)
            {
                float distanceMoved = Vector3.Distance(
                    new Vector3(transform.position.x, 0f, transform.position.z), 
                    new Vector3(lastStepPosition.x, 0f, lastStepPosition.z)
                );
                
                // Play footstep when player has moved enough distance
                if (distanceMoved >= stepDistance)
                {
                    PlayFootstepSound();
                    lastStepPosition = transform.position;
                }
            }
            
            wasMoving = isMoving;
        }
        else
        {
            // Update last position when not grounded (prevents step spam on landing)
            lastStepPosition = transform.position;
            wasMoving = false;
        }
    }
    
    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0 || audioSource == null) return;
        
        // Pick random footstep sound for variety
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        
        // Play sound
        audioSource.PlayOneShot(clip, footstepVolume);
    }
}